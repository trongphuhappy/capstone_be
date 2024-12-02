using Microsoft.EntityFrameworkCore;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence;
public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder builder)
        => builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

    public DbSet<Account> Accounts { get; set; }
    public DbSet<RoleUser> RoleUsers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Images> Images { get; set; }
    public DbSet<Lessor> Lessors { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductSurcharge> ProductSurcharges { get; set; }
    public DbSet<Surcharge> Surcharges { get; set; }
    public DbSet<Insurance> Insurances { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
}