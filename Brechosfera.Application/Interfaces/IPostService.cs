using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;

namespace Brechosfera.Application.Interfaces;

public interface IPostService
{
    public Task<IReadOnlyList<PostResponse>> ListPostAsync(CancellationToken cancellationToken);
    public Task<PostResponse?> GetPostByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<PostResponse?> CreatePostAsync(CreatePostRequest request, CancellationToken cancellationToken);
}
