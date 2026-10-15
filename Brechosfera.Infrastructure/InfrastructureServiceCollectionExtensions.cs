using Brechosfera.Application.Interfaces;
using Brechosfera.Infrastructure.Email;
using Brechosfera.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Brechosfera.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.Section));
        services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.Section));

        services.AddSingleton<IConnection>(sp => CreateConnection(
            sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value,
            sp.GetRequiredService<ILoggerFactory>().CreateLogger("RabbitMq")
        ));

        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
        services.AddSingleton<IEmailSender, SmtpEmailSender>();

        return services;
    }

    public static IServiceCollection AddRabbitMqConsumer<TMessage>(this IServiceCollection services, string queue)
        => services.AddHostedService(sp => new RabbitMqConsumer<TMessage>(
            queue,
            sp.GetRequiredService<IConnection>(),
            sp.GetRequiredService<IServiceScopeFactory>(),
            sp.GetRequiredService<ILogger<RabbitMqConsumer<TMessage>>>()
        ));

    public static IConnection CreateConnection(RabbitMqSettings settings, ILogger logger)
    {
        var factory = new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.User,
            Password = settings.Password,
            VirtualHost = settings.VirtualHost
        };

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex) when (attempt < 10)
            {
                logger.LogWarning(ex, "RabbitMQ indisponível (tentativa {Attempt}/10). Nova tentativa em 3s...", attempt);
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
        }
    }
}
