using Brechosfera.Domain.Enums;

namespace Brechosfera.Application.Requests;

public sealed record ProductTypeRequest(
    string Key,
    string Label,
    EProductTypeCategory Category
);