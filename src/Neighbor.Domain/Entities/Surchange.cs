using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Surchange : DomainEntity<Guid>
{
    public string Name { get;private set; }
    public string Description { get;private set; }
    public virtual List<ProductSurchange> productSurchanges { get; private set; }
}
