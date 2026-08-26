using Brechosfera.Domain.Enums;

namespace Brechosfera.Application.Responses;

public sealed record ProductTypeResponse(
    string Key,
    string Label,
    EProductTypeCategory Category
);
