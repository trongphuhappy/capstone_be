using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.OrderDTOs;
using Neighbor.Contract.DTOs.StatisticDTOs;

namespace Neighbor.Contract.Services.Statistic;

public static class Query
{
    public record GetTotalBoxLessorQuery(Guid AccountId) : IQuery<Success<List<BoxLessorDTO>>>;
    public record GetCountOrderByLessorQuery(Guid AccountId) : IQuery<Success<IEnumerable<OrderStatisticsDTO>>>;
    public record GetPercentByLessorQuery(Guid AccountId) : IQuery<Success<OrderStatsDTO>>;
}
