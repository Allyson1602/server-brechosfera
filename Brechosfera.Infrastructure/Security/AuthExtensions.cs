using Brechosfera.Application.Interfaces;
using Brechosfera.Infrastructure.Data;
using Brechosfera.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Brechosfera.Infrastructure.Security;

public static class AuthExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {

        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT configuration was not found.");

        services.AddScoped<ITokenGenerator, JwtProvider>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IIdentityService, IdentityService>();

        services.AddHttpContextAccessor();

        services
            .AddOptions<JwtSettings>()
            .Bind(
                configuration.GetSection(
                    JwtSettings.SectionName
                )
            )
            .Validate(
                settings => !string.IsNullOrWhiteSpace(settings.Issuer),
                "Jwt:Issuer must be configured."
            )
            .Validate(
                settings => !string.IsNullOrWhiteSpace(settings.Audience),
                "Jwt:Audience must be configured."
            )
            .Validate(
                settings => !string.IsNullOrWhiteSpace(settings.Key),
                "Jwt:Key must be configured."
            )
            .Validate(
                settings => settings.ExpirationMinutes > 0,
                "Jwt:ExpirationMinutes must be greater than zero."
            )
            .ValidateOnStart();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)
                    ),

                    ClockSkew = TimeSpan.Zero
                };
            });


        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

            options.SignIn.RequireConfirmedEmail = true;

            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;

            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
        .AddRoles<ApplicationRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        return services;
    }
}
