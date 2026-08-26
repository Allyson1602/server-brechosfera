namespace Brechosfera.Domain.ValueObjects;

public class PostLike
{
    public Guid UserId { get; private set; }
    
    private PostLike() { }

    internal PostLike(Guid userId)
    {
        UserId = userId;
    }
}
