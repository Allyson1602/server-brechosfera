using Brechosfera.Domain.Abstractions;
using Brechosfera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Brechosfera.Infrastructure.Repositories;

public abstract class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly AppDbContext DbContext;
    protected readonly DbSet<T> DbSet;

    protected Repository(AppDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual void Add(T entity)
    {
        DbSet.Add(entity);
    }

    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }
    public virtual void Delete(T entity)
    {
        DbSet.Remove(entity);
    }
}