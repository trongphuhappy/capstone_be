namespace Neighbor.Contract.Services.Feedbacks;
public static class Filter
{
    public record FeedbackFilter(Guid? ProductId, Guid? AccountId);
}