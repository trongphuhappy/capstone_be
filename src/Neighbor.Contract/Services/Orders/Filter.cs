using Neighbor.Contract.Enumarations.Product;

namespace Neighbor.Contract.Services.Orders;

public static class Filter
{
    public record ProductFilter(Guid? Id, string? Name, StatusType? StatusType, string? Policies, string? Description, double? Rating, double? Price, double? Value, int? MaximumRentDays, int? CategoryId, ConfirmStatus? ConfirmStatus, Guid? AccountUserId, Guid? AccountLessorId);

    public record ProductWishlistFilter(Guid? Id, string? Name, StatusType? StatusType, string? Policies, string? Description, double? Rating, double? Price, double? Value, int? MaximumRentDays, int? CategoryId);
}
