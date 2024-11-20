using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using static Neighbor.Contract.Services.Surcharges.Filter;

namespace Neighbor.Contract.Services.Products;

public static class Query
{
    public record GetAllProductsQuery(int PageIndex,
        int PageSize,
        SurchargeFilter FilterParams,
        string[] SelectedColumns) : IQuery<Success<PagedResult<Response.ProductResponse>>>;

    public record GetProductByIdQuery(Guid Id) : IQuery<Success<Response.ProductResponse>>;
}
