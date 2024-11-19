using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Surcharge : DomainEntity<Guid>
{
    public string Name { get;private set; }
    public string Description { get;private set; }
    public virtual List<ProductSurcharge> productSurcharges { get; private set; }
}
