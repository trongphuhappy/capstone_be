using Neighbor.Contract.Enumarations.Product;
using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Insurance : DomainEntity<Guid>
{
    public Insurance()
    {

    }

    public Insurance(string name, string description, DateTime issueDate, DateTime expirationDate, Guid productId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        IssueDate = issueDate;
        ExpirationDate = expirationDate;
        ProductId = productId;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime ExpirationDate { get; private set; }
    public Guid ProductId { get; private set; }
    public virtual Product Product { get; private set; }
    public virtual List<Images> Images { get; private set; }
    public static Insurance CreateInsurance(string name, string description, DateTime issueDate, DateTime expirationDate, Guid productId)
    {
        return new Insurance(name, description, issueDate, expirationDate, productId);
    }
}
