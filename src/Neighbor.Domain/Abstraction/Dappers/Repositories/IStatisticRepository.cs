using Neighbor.Contract.DTOs.StatisticDTOs;

namespace Neighbor.Domain.Abstraction.Dappers.Repositories;

public interface IStatisticRepository
{
    Task<List<BoxLessorDTO>> GetTotalBoxLessor(Guid lessorId);
}
