using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Responses;
using Brechosfera.Application.Mapper;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Repositories;
using Brechosfera.Application.Requests;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Brechosfera.Application.Services;

public class StoreReviewService : BaseService, IStoreReviewService
{
    private readonly IStoreReviewRepository _repository;
    private readonly IStoreRepository _storeRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IDistributedCache _cache;

    public StoreReviewService(
        IUnitOfWork unitOfWork,
        INotifier notifier,
        IStoreReviewRepository repository,
        IStoreRepository storeRepository,
        ICurrentUser currentUser,
        IDistributedCache cache
    ) : base(unitOfWork, notifier) {
        _repository = repository;
        _storeRepository = storeRepository;
        _currentUser = currentUser;
        _cache = cache;
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
        var cacheKey = $"store-review:{id}";
        var cachedStoreReview = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedStoreReview))
            return JsonSerializer.Deserialize<StoreReviewResponse>(cachedStoreReview);

        var storeReview = await _repository.GetByIdAsync(id, cancellationToken);

        if (storeReview is null)
            return null;

        var response = storeReview.ToResponse();

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);

        return response;
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

        var success = await CommitAsync(cancellationToken);

        if (!success)
            return null;

        return storeReview.ToResponse();
    }
}
