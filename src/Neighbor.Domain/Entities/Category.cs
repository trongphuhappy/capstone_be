namespace Neighbor.Domain.Entities;

public class Category
{
    public Category()
    {

    }
    public Category(string name, bool isVehicle)
    {
        this.Name = name;
        this.IsVehicle = isVehicle;
    }
    public int Id { get; private set; }
    public string Name { get; private set; }
    public bool IsVehicle { get; private set; }
    public virtual ICollection<Product> Products { get; set; }

    public static Category CreateCategory(string name, bool isVehicle)
    {
        return new Category(name, isVehicle);
    }
}