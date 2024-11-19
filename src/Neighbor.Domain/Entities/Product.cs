using Neighbor.Contract.Enumarations.Product;
using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Product : DomainEntity<Guid>
{
    public string Name { get; private set; }
    public StatusType StatusType { get; private set; }
    public string Policies { get; private set; }
    public string Description { get; private set; }
    public string? RejectReason { get; private set; }
    public double Rating { get; private set; }
    public double Price { get; private set; }
    public int CategoryId { get; private set; }
    public bool IsConfirm { get; private set; }
    public Guid LessorId { get; private set; }
    public virtual Lessor Lessor { get; private set; }
    public virtual Category Category { get; private set; }
    public virtual List<Images> Images { get; private set; }
    public virtual List<ProductSurcharge> ProductSurcharges { get; private set; }
}
