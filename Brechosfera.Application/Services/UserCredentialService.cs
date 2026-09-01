using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Application.Services;

public class UserCredentialService : BaseService, IUserCredentialService
{
    private readonly IUserCredentialRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _jwtProvider;

    public UserCredentialService(
        IPasswordHasher passwordHasher,
        ITokenGenerator jwtProvider,
        IUserCredentialRepository repository,
        IUnitOfWork unitOfWork,
        INotifier notifier
    ) : base(unitOfWork, notifier)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<UserResponse?> RegisterAsync(CreateRegisterRequest request, CancellationToken cancellationToken = default)
    {
        Password password = Password.Create(request.Password);

        var hasUsername = await _repository.ExistsByUsernameAsync(request.Username, cancellationToken);

        if (hasUsername)
        {
            Notify("Username", "Nome de usuário indisponível.");
            return default;
        }

        var hasEmail = await _repository.ExistsByEmailAsync(request.Email, cancellationToken);

        if (hasEmail)
        {
            Notify("Email", "E-mail já está em uso.");
            return default;
        }

        string passwordHash = _passwordHasher.Hash(password.Value);

        var newUser = new UserCredential(request.Username, request.Email, passwordHash);

        _repository.Add(newUser);

        var success = await CommitAsync(cancellationToken);

        if (!success)
        {
            Notify("Form", "Houve um erro ao persistir os dados no banco.");
            return default;
        }

        return new UserResponse(newUser.Id, newUser.Username, newUser.Email);
    }

    public async Task<LoginResponse?> LoginAsync(CreateLoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByLoginAsync(request.Username, cancellationToken);

        if (user is null)
        {
            Notify("Login", "Usuário ou senha inválidos.");
            return default;
        }

        bool isPasswordValid = _passwordHasher.Verify(user.PasswordHash, request.Password);

        if (!isPasswordValid)
        {
            Notify("Login", "Usuário ou senha inválidos.");
            return default;
        }

        var tokenString = _jwtProvider.Generate(user);

        return new LoginResponse(
            AccessToken: tokenString,
            RefreshToken: Guid.NewGuid().ToString(),
            Expiration: DateTime.UtcNow.AddHours(2)
        );
    }
}
