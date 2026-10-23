using Brechosfera.Application.Common;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Brechosfera.Infrastructure.Identity;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager
) : IIdentityService {
    private const string InvalidCredentials = "Usuário ou senha inválidos.";
    private const string InvalidLink = "Link de confirmação inválido ou expirado.";

    public async Task<AuthUser?> GetByIdAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());

        if (user is null)
            return null;

        return await ToAuthUserAsync(user);
    }

    public async Task<(Result Result, AuthUser? User)> DeleteAccountAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());

        if (user is null || user.DeletedAt is not null)
            return (Result.Failure(["Usuário não encontrado."]), null);

        var deletedUser = await ToAuthUserAsync(user);

        user.DeletedAt = DateTimeOffset.UtcNow;
        user.UserName = $"deleted-{user.Id:N}";
        user.Email = $"{user.Id:N}@deleted.brechosfera.local";
        user.EmailConfirmed = false;
        user.PhoneNumber = null;
        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.MaxValue;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return (Result.Failure(result.Errors.Select(e => e.Description)), null);

        return (Result.Success(), deletedUser);
    }

    public async Task<AuthUser?> FindUnconfirmedByEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null || await userManager.IsEmailConfirmedAsync(user))
            return null;

        return await ToAuthUserAsync(user);
    }

    public async Task<(Result Result, AuthUser? User)> RegisterAsync(RegisterUserRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return (Result.Failure(result.Errors.Select(e => e.Description)), null);

        return (Result.Success(), await ToAuthUserAsync(user));
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new InvalidOperationException($"Usuário {userId} não encontrado.");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
    }

    public async Task<(Result Result, AuthUser? NewlyConfirmedUser)> ConfirmEmailAsync(Guid userId, string token)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return (Result.Failure([InvalidLink]), null);

        if (user.EmailConfirmed)
            return (Result.Success(), null);

        string decodedToken;

        try
        {
            decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch (FormatException)
        {
            return (Result.Failure([InvalidLink]), null);
        }

        var result = await userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
            return (Result.Failure([InvalidLink]), null);

        return (Result.Success(), await ToAuthUserAsync(user));
    }

    public async Task<(Result Result, AuthUser? User)> CheckCredentialsAsync(string login, string password)
    {
        var user = await userManager.FindByNameAsync(login)
            ?? await userManager.FindByEmailAsync(login);

        if (user is null)
            return (Result.Failure([InvalidCredentials]), null);

        var result = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (result.IsLockedOut)
            return (Result.Failure(["Conta bloqueada temporariamente. Tente novamente em alguns minutos."]), null);

        if (result.IsNotAllowed)
            return (Result.Failure(["Confirme seu e-mail antes de entrar."]), null);

        if (!result.Succeeded)
            return (Result.Failure([InvalidCredentials]), null);

        return (Result.Success(), await ToAuthUserAsync(user));
    }

    private async Task<AuthUser> ToAuthUserAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        return new AuthUser(user.Id, user.UserName!, user.Email!, roles.ToList());
    }
}
