using RabbitMQ.Client;
using System.Text.Json;

namespace Brechosfera.Infrastructure.Messaging;

internal static class RabbitMqJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}

internal static class RabbitMqTopology
{
    public static async Task DeclareQueueWithDlqAsync(IChannel channel, string queue, CancellationToken cancellationToken)
    {
        var dlq = $"{queue}.dlq";

        await channel.QueueDeclareAsync(
            dlq,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken
        );

        var arguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = string.Empty,
            ["x-dead-letter-routing-key"] = dlq
        };

        await channel.QueueDeclareAsync(
            queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments,
            cancellationToken: cancellationToken
        );
    }
}
