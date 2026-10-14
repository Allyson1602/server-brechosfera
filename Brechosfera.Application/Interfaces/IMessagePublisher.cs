namespace Brechosfera.Application.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync<TMessage>(string queue, TMessage message, CancellationToken ct = default);
}