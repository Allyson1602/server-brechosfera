using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Guards;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Domain.Entities;

public class Post : Entity
{
    private readonly List<PostLike> _likes = new();

    public Guid UserId { get; }
    public string? Image { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public IReadOnlyCollection<PostLike> Likes => _likes.AsReadOnly();

    private Post() { }

    public Post(
        Guid userId,
        string description,
        string? image = null
    ) {
        Guard.AgainstEmptyGuid(userId, "USER_ID_EMPTY", "O usuário não pode ser nulo.");
        Guard.AgainstNullOrWhiteSpace(description, "DESCRIPTION_EMPTY", "A descrição não pode ser nula.");

        UserId = userId;
        Description = description;
        Image = image;
    }

    public void UpdatePost(string image, string description)
    {
        Guard.AgainstNullOrWhiteSpace(description, "DESCRIPTION_EMPTY", "A descrição não pode ser nula.");
        Guard.AgainstNullOrWhiteSpace(image, "IMAGE_EMPTY", "A imagem não pode ser nula.");

        Description = description;
        Image = image;
    }

    public void AddLike(Guid likeUserId)
    {
        Guard.AgainstEmptyGuid(likeUserId, "LIKE_USER_ID_EMPTY", "O usuário não pode ser nulo.");

        if (_likes.Any(x => x.UserId == likeUserId))
            return;

        _likes.Add(new PostLike(likeUserId));
    }

    public void RemoveLike(Guid likeUserId)
    {
        var like = _likes.FirstOrDefault(x => x.UserId == likeUserId);

        if (like is not null)
            _likes.Remove(like);
    }
}
