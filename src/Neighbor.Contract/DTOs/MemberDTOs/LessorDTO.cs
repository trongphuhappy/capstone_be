using Neighbor.Contract.Enumarations.Product;

namespace Neighbor.Contract.DTOs.MemberDTOs;

public sealed class LessorDTO
{
    public Guid AccountId { get; set; }
    public string WareHouseAddress { get; set; }
    public string? ShopName { get; set; }
    public LocationType LocationType { get; set; }
}
