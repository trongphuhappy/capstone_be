using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Configurations;
public class RoleUserConfiguration : IEntityTypeConfiguration<RoleUser>
{
    public void Configure(EntityTypeBuilder<RoleUser> builder)
    {
        builder.ToTable("RoleUsers");

        builder.HasKey(p => p.Id);
    }
}
