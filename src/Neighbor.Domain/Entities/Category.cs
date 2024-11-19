namespace Neighbor.Domain.Entities;

public class Category
{
    public Category()
    {
    }
    public int Id { get; set; }
    public string Name { get; private set; }
    public bool IsVehicle { get; private set; }
    public virtual ICollection<Product> Products { get; set; }
}