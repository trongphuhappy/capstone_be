using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Domain.Entities;
using static Neighbor.Contract.Services.Orders.Filter;

namespace Neighbor.Domain.Abstraction.Dappers.Repositories;

public interface IOrderRepository : IGenericRepository<Domain.Entities.Order>
{
    Task<PagedResult<Order>> GetPagedAsync(int pageIndex, int pageSize, OrderFilter filterParams, string[] selectedColumns);
    Task<Order> GetDetailsAsync(Guid productId);
}
