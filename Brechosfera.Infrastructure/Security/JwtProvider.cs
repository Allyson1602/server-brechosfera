using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Brechosfera.Application.Interfaces;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Brechosfera.Application.Common;

namespace Brechosfera.Infrastructure.Security;

public sealed class JwtProvider(IOptions<JwtSettings> options) : ITokenGenerator
{
    private readonly JwtSettings _settings = options.Value;

    public (string Token, DateTime ExpiresAt) Generate(AuthUser user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key)),
                SecurityAlgorithms.HmacSha256
            )
        };

        var handler = new JwtSecurityTokenHandler();

        return (handler.WriteToken(handler.CreateToken(descriptor)), expiresAt);
    }
}
