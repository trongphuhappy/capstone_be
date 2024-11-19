using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;

namespace Neighbor.Contract.Services.Categories;

public static class Query
{
    public record GetAllCategoriesQuery(Filter.CategoryFilter FilterParams, string[] SelectedColumns) : IQuery<Success<PagedResult<Response.CategoriesResponse>>>;
}
