using Brechosfera.Domain.Enums;

namespace Brechosfera.Application.Requests;

public sealed record ReplaceStoreProductTypesRequest(
    IReadOnlyCollection<Guid> productTypeIds
);
