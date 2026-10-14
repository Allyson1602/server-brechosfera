using Brechosfera.Application.Handlers;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Brechosfera.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMessageHandler<UserMessage>, UserHandler>();

        return services;
    }
}
