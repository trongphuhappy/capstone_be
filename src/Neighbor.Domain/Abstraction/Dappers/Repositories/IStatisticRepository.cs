using Neighbor.Contract.DTOs.OrderDTOs;
using Neighbor.Contract.DTOs.StatisticDTOs;

namespace Neighbor.Domain.Abstraction.Dappers.Repositories;

public interface IStatisticRepository
{
    Task<List<BoxLessorDTO>> GetTotalBoxLessorAsync(Guid lessorId);
    Task<IEnumerable<OrderStatisticsDTO>> GetMonthlyOrderStatisticsByLessorAsync(Guid lessorId);
    Task<OrderStatsDTO> GetPercentTotalOrder(Guid lessorId);
}
