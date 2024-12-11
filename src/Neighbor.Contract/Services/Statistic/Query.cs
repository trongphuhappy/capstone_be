using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.StatisticDTOs;

namespace Neighbor.Contract.Services.Statistic;

public static class Query
{
    public record GetTotalBoxLessor(Guid AccountId) : IQuery<Success<List<BoxLessorDTO>>>;
}
