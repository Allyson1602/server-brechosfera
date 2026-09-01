using Brechosfera.Domain.Enums;

namespace Brechosfera.Application.Requests;

public sealed record CreateProductTypeRequest(
    string Key,
    string Label,
    EProductTypeCategory Category
);