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

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PostResponse>> UpdatePostAsync(Guid id, UpdatePostRequest request, CancellationToken cancellationToken)
    {
        var result = await _postService.UpdatePostAsync(id, request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PostResponse>>> ListPost(CancellationToken cancellationToken)
    {
        var result = await _postService.ListPostAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostResponse>> GetPostById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _postService.GetPostByIdAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PostResponse>> CreatePost(CreatePostRequest request, CancellationToken cancellationToken)
    {
        var result = await _postService.CreatePostAsync(request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return Created($"/api/post/{result?.Id}", result);
    }
}
