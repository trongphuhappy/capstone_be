using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;
public class Feedback : DomainEntity<Guid>
{
    public string Content { get; set; }
    public int Like { get; set; }
    public virtual List<Images> Images { get; set; }
}
