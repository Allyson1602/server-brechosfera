using Brechosfera.Application.Common;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Mapper;
using Brechosfera.Application.Messages;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Microsoft.Extensions.Options;

namespace Brechosfera.Application.Services;

public sealed class AuthService(
    IIdentityService identity,
    ITokenGenerator tokenGenerator,
    IMessagePublisher publisher,
    INotifier notifier,
    IOptions<AppUrlSettings> appUrls
) : IAuthService {
    public async Task<Guid?> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var (result, user) = await identity.RegisterAsync(request);

        if (!result.Succeeded || user is null)
        {
            NotifyErrors("Register", result);
            return null;
        }

        var token = await identity.GenerateEmailConfirmationTokenAsync(user.Id);
        var link = $"{appUrls.Value.ConfirmEmail}?userId={user.Id}&token={token}";

        await publisher.PublishAsync(
            Queues.UserCreated,
            new UserMessage(user.Id, user.UserName, user.Email, DateTimeOffset.UtcNow, link),
            cancellationToken
        );

        return user.Id;
    }

    public async Task<bool> ConfirmEmailAsync(Guid userId, string token)
    {
        var result = await identity.ConfirmEmailAsync(userId, token);

        if (!result.Succeeded)
        {
            NotifyErrors("ConfirmEmail", result);
            return false;
        }

        return result.Succeeded;
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

    private void NotifyErrors(string key, Result result)
    {
        foreach (var error in result.Errors)
            notifier.Add(key, error);
    }
}
