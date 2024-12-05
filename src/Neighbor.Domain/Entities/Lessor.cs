using Neighbor.Contract.Enumarations.Product;
using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Lessor : DomainEntity<Guid>
{
    public Lessor()
    {

    }

    public Lessor(string wareHouseAddress, string? shopName, LocationType locationType, Guid accountId)
    {
        WareHouseAddress = wareHouseAddress;
        ShopName = shopName;
        LocationType = locationType;
        AccountId = accountId;
    }
    public string WareHouseAddress { get; private set; }
    public string? ShopName { get; private set; }
    public LocationType LocationType { get; private set; }
    public Guid AccountId { get; private set; }
    public virtual Account Account { get; private set; }
    public virtual List<Product> Products { get; private set; }
    public virtual Wallet Wallet { get; private set; }

    public static Lessor CreateLessor(string wareHouseAddress, string shopeName, LocationType locationType, Guid accountId)
    {
        // Create object lessor
        var lessor = new Lessor(wareHouseAddress, shopeName, locationType, accountId);
        // Create wallet for lessor
        var wallet = new Wallet(lessor.Id, 0);
        lessor.Wallet = wallet;
        return lessor;
    }
    public void UpdateLessor(string? wareHouseAddress, string? shopeName, LocationType? locationType)
    {
        if (wareHouseAddress != null) WareHouseAddress = wareHouseAddress;
        if (shopeName != null) ShopName = shopeName;
        if (locationType != null) LocationType = (LocationType)locationType;
    }
}
