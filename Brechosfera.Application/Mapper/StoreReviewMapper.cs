using Brechosfera.Application.Responses;
using Brechosfera.Domain.Entities;

namespace Brechosfera.Application.Mapper;

public static class StoreReviewMapper
{
    public static StoreReviewResponse ToResponse(this StoreReview storeReview)
    {
        return new StoreReviewResponse(
            storeReview.Id,
            storeReview.UserId,
            storeReview.StoreId,
            storeReview.Rating.Value,
            storeReview.Comment
        );
    }

    public static List<StoreReviewResponse> ToResponseList(this IEnumerable<StoreReview> storeReviews)
    {
        if (storeReviews is null )
            return [];

        return storeReviews.Select(s => s.ToResponse()).ToList();
    }
}
