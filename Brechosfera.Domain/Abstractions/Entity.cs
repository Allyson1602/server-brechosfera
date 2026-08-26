namespace Brechosfera.Domain.Abstractions;

public abstract class Entity
{
    public Guid Id { get; private set; }

    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; protected set; }
    public DateTimeOffset? DeletedAt { get; protected set; }
    public bool IsDeleted => DeletedAt != null;

    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    public void MarkAsUpdated() => UpdatedAt = DateTimeOffset.UtcNow;
    public void MarkAsDeleted() => DeletedAt = DateTimeOffset.UtcNow;
}