using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using static Neighbor.Contract.Services.Orders.Filter;

namespace Neighbor.Contract.Services.Orders;

public static class Query
{
    public record GetAllOrdersQuery(int PageIndex,
        int PageSize,
        OrderFilter FilterParams,
        string[] SelectedColumns) : IQuery<Success<PagedResult<Response.OrderResponse>>>;

    public record GetOrderByIdQuery(Guid Id) : IQuery<Success<Response.OrderResponse>>;

}
