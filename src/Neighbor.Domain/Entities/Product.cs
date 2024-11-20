using Neighbor.Contract.Enumarations.Product;
using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Product : DomainEntity<Guid>
{
    public Product()
    {

    }
    public Product(string name, StatusType statusType, string policies, string description, double rating, double price, double value, int categoryId, bool isConfirm, Guid lessorId)
    {
        Id = Guid.NewGuid();
        Name = name;
        StatusType = statusType;
        Policies = policies;
        Description = description;
        Rating = rating;
        Price = price;
        Value = value;
        CategoryId = categoryId;
        IsConfirm = isConfirm;
        LessorId = lessorId;
    }

    public string Name { get; private set; }
    public StatusType StatusType { get; private set; }
    public string Policies { get; private set; }
    public string Description { get; private set; }
    public string? RejectReason { get; private set; }
    public double Rating { get; private set; }
    public double Price { get; private set; }
    public double Value { get; private set; }
    public int CategoryId { get; private set; }
    public bool IsConfirm { get; private set; }
    public Guid LessorId { get; private set; }
    public virtual Lessor Lessor { get; private set; }
    public virtual Category Category { get; private set; }
    public virtual List<Images> Images { get; private set; }
    public virtual List<ProductSurcharge> ProductSurcharges { get; private set; }
    public virtual List<Insurance> Insurances { get; private set; }

    public static Product CreateProduct(string name, string policies, string description, double price, double value, int categoryId, Guid lessorId)
    {
        return new Product(name, StatusType.Available, policies, description, 0, price, value, categoryId, false, lessorId);
    }
}
