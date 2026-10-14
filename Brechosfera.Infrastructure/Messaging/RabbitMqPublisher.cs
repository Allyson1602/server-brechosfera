using Brechosfera.Application.Interfaces;
using RabbitMQ.Client;
using System.Text.Json;

namespace Brechosfera.Infrastructure.Messaging;

public sealed class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConnection _connection;

    public RabbitMqPublisher(IConnection connection)
    {
        _connection = connection;
    }

    public async Task PublishAsync<TMessage>(string queue, TMessage message, CancellationToken cancellationToken = default)
    {
        using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await RabbitMqTopology.DeclareQueueWithDlqAsync(channel, queue, cancellationToken);

        var body = JsonSerializer.SerializeToUtf8Bytes(message, RabbitMqJson.Options);
        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queue,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken
        );
    }
}
