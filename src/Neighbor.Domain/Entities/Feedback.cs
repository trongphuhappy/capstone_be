using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;
public class Feedback : DomainEntity<Guid>
{
    public string Content { get; private set; }
    public int Like { get; private set; }
    public virtual List<Images> Images { get; private set; }
    public Guid OrderId { get; private set; }
    public virtual Order Order { get; private set; }
}
