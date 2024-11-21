using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Category : DomainEntity<int>
{
    public Category()
    {

    }
    public Category(string name, bool isVehicle, bool isDeleted)
    {
        this.Name = name;
        this.IsVehicle = isVehicle;
        this.IsDeleted = isDeleted;
    }
    public int Id { get; private set; }
    public string Name { get; private set; }
    public bool IsVehicle { get; private set; }
    public virtual ICollection<Product> Products { get; set; }

    public static Category CreateCategory(string name, bool isVehicle, bool isDeleted)
    {
        return new Category(name, isVehicle, isDeleted);
    }
}