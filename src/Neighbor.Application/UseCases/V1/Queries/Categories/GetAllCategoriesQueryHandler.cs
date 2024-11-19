using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Categories;

namespace Neighbor.Application.UseCases.V1.Queries.Categories;

public sealed class GetAllCategoriesQueryHandler : IQueryHandler<Query.GetAllCategoriesQuery, Success<PagedResult<Response.CategoriesResponse>>>
{
    public async Task<Result<Success<PagedResult<Response.CategoriesResponse>>>> Handle(Query.GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
