using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Enums;

namespace Brechosfera.Domain.Entities;

public class ProductType : Entity
{
    public string Key { get; private set; } = string.Empty;
    public string Label { get; private set; } = string.Empty;
    public EProductTypeCategory Category { get; private set; }

    private ProductType() { }
}
