using Brechosfera.Application.Common;
using Brechosfera.Application.Responses;

namespace Brechosfera.Application.Mapper;

public static class AuthUserMapper
{
    public static UserSummary ToSummary(this AuthUser user)
        => new(user.Id, user.UserName, user.Email, user.Roles);
}
