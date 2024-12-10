using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Category : DomainEntity<int>
{
    public Category()
    {

    }
    public Category(string name, string? image, bool isVehicle, bool isDeleted)
    {
        Name = name;
        Image = image;
        IsVehicle = isVehicle;
        IsDeleted = isDeleted;
    }
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string? Image { get; private set; }
    public bool IsVehicle { get; private set; }
    public virtual ICollection<Product> Products { get; set; }
    
    public static Category CreateCategory(string name, string? image, bool isVehicle, bool isDeleted)
    {
        return new Category(name, image, isVehicle, isDeleted);
    }

    public static Category CreateCategoryForTest(int id, string name, bool isVehicle, string? image)
    {
        return new Category()
        {
            Id = id,
            Name = name,
            IsVehicle = isVehicle,
            Image = image
        };
    }
}