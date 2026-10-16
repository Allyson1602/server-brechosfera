namespace Brechosfera.Application.Common;

public static class Roles
{
    public const string Admin = "Admin";
    public const string StoreOwner = "StoreOwner";

    public static readonly IReadOnlyList<string> All = [Admin, StoreOwner];
}
