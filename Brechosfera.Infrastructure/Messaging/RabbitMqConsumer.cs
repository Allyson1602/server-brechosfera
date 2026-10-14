using Brechosfera.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Brechosfera.Infrastructure.Messaging;

public sealed class RabbitMqConsumer<TMessage>(
    string queue,
    IConnection connection,
    IServiceScopeFactory scopeFactory,
    ILogger<RabbitMqConsumer<TMessage>> logger
) : BackgroundService {
    protected override async Task ExecuteAsync (CancellationToken stoppingToken)
    {
        using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await RabbitMqTopology.DeclareQueueWithDlqAsync(channel, queue, stoppingToken);

        await channel.BasicQosAsync(0, 1, false, stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<TMessage>(ea.Body.Span, RabbitMqJson.Options)
                                ?? throw new InvalidOperationException("Mensagem vazia ou inválida.");

                using var scope = scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<TMessage>>();

                await handler.HandleAsync(message, stoppingToken);

                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha ao processar mensagem da fila {Queue}. Enviando para {Queue}.dlq", queue, queue);

                await channel.BasicNackAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    stoppingToken
                );
            }
        };

        await channel.BasicConsumeAsync(queue, autoAck: false, consumer, stoppingToken);

        logger.LogInformation("Consumer escutando a fila {Queue}", queue);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // desligamento normal
        }
    }
}
