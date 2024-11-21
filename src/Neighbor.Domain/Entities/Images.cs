using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Images : DomainEntity<Guid>
{
    public Images()
    {

    }

    public Images(string imageLink, string imageId, Guid? productId, Guid? insuranceId, Guid? feedbackId)
    {
        ImageLink = imageLink;
        ImageId = imageId;
        ProductId = productId;
        InsuranceId = insuranceId;
        FeedBackId = feedbackId;
    }
    
    public string ImageLink { get; private set; }
    public string ImageId { get; private set; }

    public Guid? ProductId { get; private set; }
    public virtual Product? Product { get; private set; }
    public Guid? FeedBackId { get; private set; }
    public virtual Feedback? Feedback { get; private set; }
    public Guid? InsuranceId { get; private set; }
    public virtual Insurance? Insurance { get; private set; }
}
