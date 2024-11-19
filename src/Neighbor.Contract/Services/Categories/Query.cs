using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using static Neighbor.Contract.Services.Categories.Filter;

namespace Neighbor.Contract.Services.Categories;

public static class Query
{
    public record GetAllCategoriesQuery(int PageIndex,
        int PageSize,
        CategoryFilter FilterParams,
        string[] SelectedColumns) : IQuery<Success<PagedResult<Response.CategoryResponse>>>;
}
