﻿using Neighbor.Contract.Enumarations.Product;
using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Product : DomainEntity<Guid>
{
    public Product()
    {
    }

    public Product(string name, StatusType statusType, string policies, string description, double rating, long price, long value, int maximumRentDays, int categoryId, ConfirmStatus confirmStatus, Guid lessorId)
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
    public long Price { get; private set; }
    public long Value { get; private set; }
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
    public virtual List<Order>? Orders { get; private set; }

    public static Product CreateProduct(string name, string policies, string description, long price, long value, int maximumRentDays, int categoryId, Guid lessorId)
    {
        return new Product(name, StatusType.Available, policies, description, 0, price, value, maximumRentDays,  categoryId, ConfirmStatus.Pending, lessorId);
    }

    public void UpdateProduct(string name, string policies, string description, long price, long value, int maximumRentDays, string rejectReason, StatusType statusType, ConfirmStatus confirmStatus)
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

    public void UpdateCategory(Category category)
    {
        Category = category;
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

    public void UpdateStatusType(StatusType statusType)
    {
        StatusType = statusType;
    }

    public static Product CreateProductForConfirmProductCommandHandlerTest(Lessor lessor)
    {
        return new Product()
        {
            Lessor = lessor
        };
    }

    public static Product CreateProductForAddToWishlistCommandHandlerTest(Lessor lessor)
    {
        return new Product()
        {
            Lessor = lessor
        };
    }

    public static Product CreateProductForCreateOrderBankingCommandHandlerTest(Lessor lessor)
    {
        return new Product()
        {
            Lessor = lessor,
            StatusType = StatusType.Available,
            ConfirmStatus = ConfirmStatus.Approved,
        };
    }

    public static Product CreateProductForOrderSuccessCommandHandlerTest(Guid Id, Lessor lessor, long price, StatusType statusType)
    {
        return new Product()
        {
            Id = Id,
            Lessor = lessor,
            Price = price,
            StatusType = StatusType.Available,
        };
    }

    public static Product CreateProductWithNameAndLessors(string name, Lessor lessor)
    {
        return new Product()
        {
            Name = name,
            Lessor = lessor,
        };
    }

    public static Product CreateProductWithIdAndLessors(Guid id, Lessor lessor)
    {
        return new Product()
        {
            Id = id,
            Lessor = lessor,
        };
    }

    public static Product CreateProductWithIdAndNameAndPrice(Guid id, string name, long price)
    {
        return new Product()
        {
            Id = id,
            Name = name,
            Price = price
        };
    }

    public static Product CreateProductForGetAllTest(Guid id, string name, StatusType statusType, string policies, string description, double rating, long price, long value, int maximumRentDays, ConfirmStatus confirmStatus, DateTime createdDate, List<Wishlist> wishlists, Lessor lessor, Category category, List<Images> images)
    {
        var product = new Product
        {
            Id = id,
            Name = name,
            StatusType = statusType,
            Policies = policies,
            Description = description,
            Rating = rating,
            Price = price,
            Value = value,
            MaximumRentDays = maximumRentDays,
            ConfirmStatus = confirmStatus,
            Wishlists = wishlists,
            CreatedDate = createdDate,
            Lessor = lessor,
            Category = category,
            Images = images
        };
        return product;
    }

    public static Product CreateProductForGetDetailsTest(Guid id, string name, StatusType statusType, string policies, string description, double rating, long price, long value, int maximumRentDays, ConfirmStatus confirmStatus, DateTime createdDate, List<Wishlist> wishlists, List<Insurance> insurances, List<ProductSurcharge> productSurcharges, Lessor lessor, Category category, List<Images> images)
    {
        var product = new Product
        {
            Id = id,
            Name = name,
            StatusType = statusType,
            Policies = policies,
            Description = description,
            Rating = rating,
            Price = price,
            Value = value,
            MaximumRentDays = maximumRentDays,
            ConfirmStatus = confirmStatus,
            Wishlists = wishlists,
            CreatedDate = createdDate,
            Insurances = insurances,
            ProductSurcharges = productSurcharges,
            Lessor = lessor,
            Category = category,
            Images = images
        };
        return product;
    }
}
