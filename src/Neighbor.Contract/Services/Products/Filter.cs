using Neighbor.Contract.Enumarations.Product;

namespace Neighbor.Contract.Services.Products;

public static class Filter
{
    public record ProductFilter(Guid? Id, string? Name, StatusType? StatusType, string? Policies, string? Description, double? Rating, double? Price, double? Value, int? CategoryId, ConfirmStatus? ConfirmStatus, Guid? AccountId);
}
