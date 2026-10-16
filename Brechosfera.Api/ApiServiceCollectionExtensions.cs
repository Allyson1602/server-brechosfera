using Brechosfera.Api.Exceptions;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

namespace Brechosfera.Api;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddFluentValidationAutoValidation(config => config.DisableDataAnnotationsValidation = true);
        services.AddAuthorization(options =>
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build()
        );

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Cole só o token, sem o prefixo 'Bearer'."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme , Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        services.AddExceptionHandler<DomainExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
