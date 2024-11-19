using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Domain.Entities;
using static Neighbor.Contract.Services.Categories.Filter;
using static Neighbor.Contract.Services.Surcharges.Filter;

namespace Neighbor.Domain.Abstraction.Dappers.Repositories;

public interface ISurchargeRepository : IGenericRepository<Surcharge>
{
    Task<PagedResult<Surcharge>> GetPagedAsync(int pageIndex, int pageSize, SurchargeFilter filterParams, string[] selectedColumns);

}
