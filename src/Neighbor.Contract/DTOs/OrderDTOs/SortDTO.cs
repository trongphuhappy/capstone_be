using Neighbor.Contract.Enumarations.Order;

namespace Neighbor.Contract.DTOs.OrderDTOs;

public sealed class SortDTO
{
    public SortType SortType { get; set; }
    public bool IsSortASC { get; set; }
}
