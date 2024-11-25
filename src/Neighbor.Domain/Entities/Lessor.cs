using Neighbor.Contract.Enumarations.Product;
using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Lessor : DomainEntity<Guid>
{
    public Lessor()
    {

    }

    public Lessor(string wareHouseAddress, string? description, string? shopName, int? responseRate, int? responseTime, int? aggreementRate, LocationType locationType, Guid accountId)
    {
        WareHouseAddress = wareHouseAddress;
        Description = description;
        ShopName = shopName;
        ResponseRate = responseRate;
        ResponseTime = responseTime;
        AggreementRate = aggreementRate;
        LocationType = locationType;
        AccountId = accountId;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = DateTime.UtcNow;
    }

    public string WareHouseAddress { get; private set; }
    public string? Description { get; private set; }
    public string? ShopName { get; private set; }
    public int? ResponseRate { get; private set; }
    public int? ResponseTime { get;private set; }
    public int? AggreementRate { get; private set; }
    public LocationType LocationType { get; private set; }
    public Guid AccountId { get; private set; }
    public virtual Account Account { get; private set; }
    public virtual List<Product> Products { get; private set; }
}
