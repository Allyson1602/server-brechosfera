using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Responses;
using Brechosfera.Application.Mapper;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Repositories;
using Brechosfera.Application.Requests;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Application.Services;

public class StoreReviewService : BaseService, IStoreReviewService
{
    private readonly IStoreReviewRepository _repository;

    public StoreReviewService(
        IUnitOfWork unitOfWork,
        INotifier notifier,
        IStoreReviewRepository repository
    ): base(unitOfWork, notifier) {
        _repository = repository;
    }

    public async Task<List<StoreReviewResponse>> ListStoreReviewByStoreIdAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var storeReviews = await _repository.ListStoreReviewByStoreIdAsync(storeId, cancellationToken);

        if (storeReviews is null)
        {
            Notify("StoreReviews", "Não foi possível encontrar as avaliações.");
            return [];
        }

        return storeReviews.ToResponseList();
    }

    public async Task<StoreReviewResponse?> GetStoreReviewByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var storeReview = await _repository.GetByIdAsync(id, cancellationToken);

        if (storeReview is null)
            return null;

        return storeReview.ToResponse();
    }

    public async Task<StoreReviewResponse?> AddStoreReviewAsync(CreateStoreReviewRequest request, CancellationToken cancellationToken)
    {
        var rating = Rating.Create(request.Rating);

        var storeReview = new StoreReview(
            request.UserId,
            request.StoreId,
            rating,
            request.Comment
        );

        _repository.Add(storeReview);

        var result = await CommitAsync(cancellationToken);

        if (!result)
            return null;

        return storeReview.ToResponse();
    }
}
