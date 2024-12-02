using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using static Neighbor.Contract.Services.Orders.Filter;

namespace Neighbor.Contract.Services.Orders;

public static class Query
{
    public record GetAllProductsQuery(int PageIndex,
        int PageSize,
        ProductFilter FilterParams,
        string[] SelectedColumns) : IQuery<Success<PagedResult<Response.ProductResponse>>>;

    public record GetProductByIdQuery(Guid Id, Guid? AccountId) : IQuery<Success<Response.ProductResponse>>;
    public record GetAllProductsInWishlistQuery(Guid AccountId, int PageIndex,
        int PageSize,
        ProductWishlistFilter FilterParams,
        string[] SelectedColumns) : IQuery<Success<PagedResult<Response.ProductResponse>>>;

}
