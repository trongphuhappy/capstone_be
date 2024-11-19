using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Images : DomainEntity<Guid>
{
    public string ImageLink { get; private set; }
    public string ImageId { get; private set; }

    public Guid ProductId { get; private set; }
    public virtual Product Product { get; private set; }
    public Guid FeedBackId { get; private set; }
    public virtual Feedback Feedback { get; private set; }
}
