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
    private readonly IStoreRepository _storeRepository;
    private readonly ICurrentUser _currentUser;

    public StoreReviewService(
        IUnitOfWork unitOfWork,
        INotifier notifier,
        IStoreReviewRepository repository,
        IStoreRepository storeRepository,
        ICurrentUser currentUser
    ) : base(unitOfWork, notifier) {
        _repository = repository;
        _storeRepository = storeRepository;
        _currentUser = currentUser;
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

        var store = await _storeRepository.GetByIdAsync(request.StoreId, cancellationToken);

        if (store is null)
        {
            Notify(
                "STORE_NOT_FOUND",
                "A loja não foi encontrada.");

            return null;
        }

        if (store.CheckIsOwnedBy(_currentUser.UserId))
        {
            Notify(
                "STORE_REVIEW_OWN_STORE",
                "Você não pode avaliar a sua própria loja.");

            return null;
        }

        var isAlreadyReviewed = await _repository.ExistsByUserAndStoreAsync(
            _currentUser.UserId, store.Id, cancellationToken);

        if (isAlreadyReviewed)
        {
            Notify(
                "STORE_REVIEW_ALREADY_EXISTS",
                "Você já avaliou esta loja.");

            return null;
        }

        var storeReview = new StoreReview(
            _currentUser.UserId,
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
