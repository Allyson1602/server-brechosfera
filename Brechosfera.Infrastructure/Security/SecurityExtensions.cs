using Brechosfera.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Brechosfera.Infrastructure.Security;

public static class SecurityExtensions
{
    public static IServiceCollection AddSecurityConfiguration(this IServiceCollection services)
    {
        services.AddScoped<ITokenGenerator, JwtProvider>();

        return services;
    }
}
