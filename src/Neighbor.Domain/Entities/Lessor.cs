using Neighbor.Contract.Enumarations.Product;
using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Lessor : DomainEntity<Guid>
{
    public string WareHouseAddress { get; private set; }
    public string? Description { get; private set; }
    public string? ShopName { get; private set; }
    public int? ResponseRate { get; private set; }
    public int? ResponseTime { get;private set; }
    public int? AggreementRate { get; private set; }
    public TimeUnitType TimeUnitType { get;private set; }
    public LocationType LocationType { get; private set; }
    public Guid AccountId { get; private set; }
    public virtual Account Account { get; private set; }
    public virtual List<Product> Products { get; private set; }
}
