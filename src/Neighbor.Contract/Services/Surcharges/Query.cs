using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using static Neighbor.Contract.Services.Categories.Filter;
using static Neighbor.Contract.Services.Surcharges.Filter;

namespace Neighbor.Contract.Services.Surcharges;

public static class Query
{
    public record GetAllSurchargesQuery(int PageIndex,
        int PageSize,
        SurchargeFilter FilterParams,
        string[] SelectedColumns) : IQuery<Success<PagedResult<Response.SurchargeResponse>>>;

    public record GetSurchargeByIdQuery(Guid Id) : IQuery<Success<Response.SurchargeResponse>>;
}
