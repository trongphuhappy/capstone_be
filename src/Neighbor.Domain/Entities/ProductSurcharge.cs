using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class ProductSurcharge : DomainEntity<Guid>
{
    public ProductSurcharge()
    {

    }

    public ProductSurcharge(double price, Guid productId, Guid surchargeId)
    {
        Price = price;
        ProductId = productId;
        SurchargeId = surchargeId;
    }

    public double Price { get; set; }
    public Guid ProductId { get; private set; }
    public virtual Product Product { get; private set; }

    public Guid SurchargeId { get; private set; }
    public virtual Surcharge Surcharge { get; private set; }
    public static ProductSurcharge CreateProductSurcharge(double price, Guid productId, Guid surcharge)
    {
        return new ProductSurcharge(price, productId, surcharge);
    }
}
