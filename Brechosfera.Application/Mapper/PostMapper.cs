using Brechosfera.Application.Responses;
using Brechosfera.Domain.Entities;

namespace Brechosfera.Application.Mapper;

public static class PostMapper
{
    public static PostResponse ToResponse(this Post post)
    {
        return new PostResponse(
            post.Id,
            post.UserId,
            post.Image,
            post.Description,
            post.Likes
        );
    }

    public static IReadOnlyList<PostResponse> ToResponseList(this IReadOnlyList<Post> posts)
    {
        if (posts is null)
            return new List<PostResponse>();

        return posts.Select(p => p.ToResponse()).ToList();
    }
}
