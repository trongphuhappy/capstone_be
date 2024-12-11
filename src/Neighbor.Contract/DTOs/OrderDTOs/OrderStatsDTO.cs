namespace Neighbor.Contract.DTOs.OrderDTOs;

public class OrderStatsDTO
{
    public int TotalOrders { get; set; }
    public double PercentOrderSuccess { get; set; }
    public double PercentOrderFail { get; set; }
}

