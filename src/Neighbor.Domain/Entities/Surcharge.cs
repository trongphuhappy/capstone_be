using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Surcharge : DomainEntity<Guid>
{
    public Surcharge()
    {

    }
    public Surcharge(Guid id, string name, string description, bool? isDeleted)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.IsDeleted = isDeleted;
    }
    public string Name { get;private set; }
    public string Description { get;private set; }
    public virtual List<ProductSurcharge> productSurcharges { get; private set; }

    public static Surcharge CreateSurcharge(Guid Id, string Name, string Description, bool? IsDeleted)
    {
        return new Surcharge(Id, Name, Description, IsDeleted);
    }

    public static Surcharge CreateSurchargeForTest(Guid id, string name, string description)
    {
        return new Surcharge()
        {
            Id = id,
            Name = name,
            Description = description
        };
    }
}
