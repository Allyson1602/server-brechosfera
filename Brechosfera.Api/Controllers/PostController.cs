using Brechosfera.Application.Common;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brechosfera.Api.Controllers;

[ApiController]
[Route("api/post")]
public class PostController : ApiController
{
    private readonly IPostService _postService;

    public PostController(INotifier notifier, IPostService postService) : base(notifier)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PostResponse>>>> ListPost(CancellationToken cancellationToken)
    {
        var result = await _postService.ListPostAsync(cancellationToken);

        return CustomResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PostResponse>>> GetPostById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _postService.GetPostByIdAsync(id, cancellationToken);

        return CustomResponse(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PostResponse>>> CreatePost(PostRequest request, CancellationToken cancellationToken)
    {
        var result = await _postService.CreatePostAsync(request, cancellationToken);

        return CustomCreate($"/api/post/{result?.Id}", result);
    }
}
