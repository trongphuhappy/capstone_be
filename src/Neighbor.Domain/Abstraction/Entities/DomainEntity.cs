namespace Neighbor.Domain.Abstraction.Entities;

public class DomainEntity<TKey>
{
    public virtual TKey Id { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool? IsDeleted { get; set; }
    /// <summary>
    /// True if domain entity has an identity
    /// </summary>
    /// <returns></returns>
    public bool IsTransient()
    {
        return Id.Equals(default(TKey));
    }
}
