using Neighbor.Contract.Enumarations.Authentication;

namespace Neighbor.Domain.Entities;
public class RoleUser
{
    public RoleUser()
    { }

    public RoleType Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public virtual ICollection<Account> Accounts { get; set; }
}
