using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class ProductSurcharge : DomainEntity<Guid>
{
    public double Price { get; set; }
    public Guid ProductId { get; private set; }
    public virtual Product Product { get; private set; }

    public Guid SurchangeId { get; private set; }
    public virtual Surcharge Surcharge { get; private set; }
}
