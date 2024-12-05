using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;
public class Feedback : DomainEntity<Guid>
{

    public Feedback()
    {
    }

    public Feedback(string content, int like, Guid orderId, Guid accountId, Guid productId)
    {
        Content = content;
        Like = like;
        OrderId = orderId;
        AccountId = accountId;
        ProductId = productId;
    }
    public string Content { get; private set; }
    public int Like { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid ProductId { get; private set; }
    public virtual Order Order { get; private set; }
    public virtual Account Account { get; private set; }
    public void UpdateAccount(Account account)
    {
        Account = account;
    }
    public static Feedback CreateFeedback(Guid orderId, Guid accountId, Guid productId, string content)
    {
        return new Feedback(content, 0, orderId, accountId, productId);
    }
}
