using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Mapper;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;

namespace Brechosfera.Application.Services;

public class PostService : BaseService, IPostService
{
    private readonly IRepository<Post> _repository;
    private readonly IUserCredentialRepository _userRepository;

    public PostService(
        IUnitOfWork unitOfWork,
        INotifier notifier,
        IRepository<Post> repository,
        IUserCredentialRepository userRepository
    ) : base(unitOfWork, notifier) {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<bool> DeletePostAsync(Guid id, CancellationToken cancellationToken)
    {
        var post = await _repository.GetByIdAsync(id);

        if (post is null)
        {
            Notify(
                "PostId",
                "O post informado não foi encontrado."
            );

            return false;
        }

        if (post.DeletedAt.HasValue)
        {
            Notify(
                "DeletePost",
                "O post já foi deletado."
            );

            return false;
        }

        post.MarkAsDeleted();

        var success = await CommitAsync(cancellationToken);

        return success;
    }

    public async Task<PostResponse?> UpdatePostAsync(Guid id, UpdatePostRequest request, CancellationToken cancellationToken)
    {
        var post = await _repository.GetByIdAsync(id, cancellationToken);

        if (post is null)
        {
            Notify(
                "PostId",
                "O post informado não foi encontrado."
            );

            return null;
        }

        post.UpdatePost(
            request.Image,
            request.Description
        );

        post.MarkAsUpdated();

        var success = await CommitAsync(cancellationToken);

        return success ? post.ToResponse() : null;
    }

    public async Task<IReadOnlyList<PostResponse>> ListPostAsync(CancellationToken cancellationToken)
    {
        var posts = await _repository.GetAllAsync(cancellationToken);

        return posts.ToResponseList();
    }

    public async Task<PostResponse?> GetPostByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var post = await _repository.GetByIdAsync(id, cancellationToken);

        if (post is null)
            return null;

        return post.ToResponse();
    }

    public async Task<PostResponse?> CreatePostAsync(CreatePostRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            Notify("UserId", "O usuário informado não foi encontrado.");
            return null;
        }

        var newPost = new Post(
            request.UserId,
            request.Description,
            request.Image
        );

        _repository.Add(newPost);

        var success = await CommitAsync(cancellationToken);

        return success ? newPost.ToResponse() : null;
    }
}
