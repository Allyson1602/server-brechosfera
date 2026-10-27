using Brechosfera.Application.Common;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Mapper;
using Brechosfera.Application.Messages;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace Brechosfera.Application.Services;

public sealed class AuthService(
    StoreDeletion storeDeletion,
    IStoreRepository storeRepository,
    IIdentityService identity,
    ITokenGenerator tokenGenerator,
    IMessagePublisher publisher,
    INotifier notifier,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IOptions<AppUrlSettings> appUrls
) : IAuthService {
    public async Task DeleteAccountAsync(CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        AuthUser? deletedUser = null;
        Result? failure = null;

        var committed = await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var stores = await storeRepository.ListByOwnerAsync(userId, cancellationToken);

            await storeDeletion.MarkAsDeletedAsync(stores, cancellationToken);

            await unitOfWork.CommitAsync(cancellationToken);

            var (result, user) = await identity.DeleteAccountAsync(userId);

            if (!result.Succeeded || user is null)
            {
                failure = result;
                return false;
            }

            deletedUser = user;
            return true;
        }, cancellationToken);

        if (!committed || deletedUser is null)
        {
            if (failure is not null)
                NotifyErrors("DeleteAccount", failure);

            return;
        }

        await publisher.PublishAsync(
            Queues.UserDeleted,
            new UserDeletedMessage(deletedUser.Id, deletedUser.UserName, deletedUser.Email),
            cancellationToken
        );
    }

    public async Task ResendConfirmationAsync(ResendConfirmationRequest request, CancellationToken cancellationToken)
    {
        var user = await identity.FindUnconfirmedByEmailAsync(request.Email);

        if (user is null)
            return;

        await SendConfirmationEmailAsync(user, cancellationToken);
    }

    public async Task<Guid?> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var (result, user) = await identity.RegisterAsync(request);

        if (!result.Succeeded || user is null)
        {
            NotifyErrors("Register", result);
            return null;
        }

        await SendConfirmationEmailAsync(user, cancellationToken);

        return user.Id;
    }

    public async Task<bool> ConfirmEmailAsync(Guid userId, string token, CancellationToken cancellationToken)
    {
        var (result, newlyConfirmedUser) = await identity.ConfirmEmailAsync(userId, token);

        if (!result.Succeeded)
        {
            NotifyErrors("ConfirmEmail", result);
            return false;
        }

        if (newlyConfirmedUser is not null)
        {
            await publisher.PublishAsync(
                Queues.UserCreated,
                new UserMessage(newlyConfirmedUser.Id, newlyConfirmedUser.UserName, newlyConfirmedUser.Email, DateTimeOffset.UtcNow),
                cancellationToken
            );
        }

        return true;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var (result, user) = await identity.CheckCredentialsAsync(request.Login, request.Password);

        if (!result.Succeeded || user is null)
        {
            NotifyErrors("Login", result);
            return null;
        }

        var (accessToken, expiresAt) = tokenGenerator.Generate(user);

        return new LoginResponse(accessToken, expiresAt, user.ToSummary());
    }

    private async Task SendConfirmationEmailAsync(AuthUser user, CancellationToken cancellationToken)
    {
        var token = await identity.GenerateEmailConfirmationTokenAsync(user.Id);
        var link = $"{appUrls.Value.ConfirmEmail}?userId={user.Id}&token={token}";

        await publisher.PublishAsync(
            Queues.EmailConfirmationRequested,
            new EmailConfirmationMessage(user.Id, user.UserName, user.Email, link),
            cancellationToken
        );
    }

    private void NotifyErrors(string key, Result result)
    {
        foreach (var error in result.Errors)
            notifier.Add(key, error);
    }
}
