using Neighbor.Contract.Enumarations.Product;
using Neighbor.Domain.Abstraction.Entities;
using System.Runtime.CompilerServices;

namespace Neighbor.Domain.Entities;

public class Product : DomainEntity<Guid>
{
    public Product()
    {

    }
    public Product(string name, StatusType statusType, string policies, string description, double rating, double price, double value, int maximumRentDays, int categoryId, ConfirmStatus confirmStatus, Guid lessorId)
    {
        Id = Guid.NewGuid();
        Name = name;
        StatusType = statusType;
        Policies = policies;
        Description = description;
        Rating = rating;
        Price = price;
        Value = value;
        MaximumRentDays = maximumRentDays;
        CategoryId = categoryId;
        ConfirmStatus = confirmStatus;
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
    public int MaximumRentDays { get; private set; }
    public int CategoryId { get; private set; }
    public ConfirmStatus ConfirmStatus { get; private set; }
    public Guid LessorId { get; private set; }
    public virtual Lessor Lessor { get; private set; }
    public virtual Category Category { get; private set; }
    public virtual List<Images> Images { get; private set; }
    public virtual List<ProductSurcharge> ProductSurcharges { get; private set; }
    public virtual List<Insurance> Insurances { get; private set; }
    public virtual List<Wishlist>? Wishlists { get; private set; }

    public static Product CreateProduct(string name, string policies, string description, double price, double value, int maximumRentDays, int categoryId, Guid lessorId)
    {
        return new Product(name, StatusType.Available, policies, description, 0, price, value, maximumRentDays,  categoryId, ConfirmStatus.Pending, lessorId);
    }

    public void UpdateProduct(string name, string policies, string description, double price, double value, int maximumRentDays, string rejectReason, StatusType statusType, ConfirmStatus confirmStatus)
    {
        Name = name;
        Policies = policies;
        Description = description;
        Price = price;
        Value = value;
        MaximumRentDays = maximumRentDays;
        RejectReason = rejectReason;
        StatusType = statusType;
        ConfirmStatus = confirmStatus;
    }

    public void UpdateImagesProduct(List<Images> images)
    {
        Images = images;
    }

    public void UpdateWishlistsProduct(List<Wishlist> wishlists)
    {
        Wishlists = wishlists;
    }

    public void UpdateLessorProduct(Lessor lessor)
    {
        Lessor = lessor;
    }

    public void UpdateProductSurcharges(List<ProductSurcharge> productSurcharges)
    {
        ProductSurcharges = productSurcharges;
    }

    public void UpdateInsurance(List<Insurance> insurances)
    {
        Insurances = insurances;
    }
}
