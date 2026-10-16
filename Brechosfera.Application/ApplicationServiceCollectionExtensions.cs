using Brechosfera.Application.Handlers;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Messages;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Brechosfera.Application.Common;
using Microsoft.Extensions.Configuration;
using FluentValidation;

namespace Brechosfera.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppUrlSettings>(configuration.GetSection(AppUrlSettings.SectionName));

        services.AddScoped<INotifier, Notifier>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IStoreReviewService, StoreReviewService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IPostService, PostService>();

        services.AddValidatorsFromAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly);

        return services;
    }

    public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
    {
        services.AddScoped<IMessageHandler<UserMessage>, UserHandler>();
        return services;
    }
}
