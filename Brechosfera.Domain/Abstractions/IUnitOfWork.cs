namespace Brechosfera.Domain.Abstractions;

public interface IUnitOfWork : IDisposable
{
    Task<bool> CommitAsync(CancellationToken cancellationToken = default);
    Task<bool> ExecuteInTransactionAsync(Func<Task<bool>> operation, CancellationToken cancellationToken = default);
}
