using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Domain.Entities;
using static Neighbor.Contract.Services.Categories.Filter;

namespace Neighbor.Domain.Abstraction.Dappers.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<PagedResult<Category>> GetPagedAsync(int pageIndex, int pageSize, CategoryFilter filterParams, string[] selectedColumns);
}
