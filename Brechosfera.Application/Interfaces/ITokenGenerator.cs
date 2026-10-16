using Brechosfera.Application.Common;

namespace Brechosfera.Application.Interfaces;

public interface ITokenGenerator
{
    (string Token, DateTime ExpiresAt) Generate(AuthUser user);
}
