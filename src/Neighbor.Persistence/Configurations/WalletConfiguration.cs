using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(w => w.Id);

        builder.HasOne(w => w.Lessor)
              .WithMany()
              .HasForeignKey(w => w.LessorId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
