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
    public DbSet<Category> Category { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Images> Images { get; set; }
    public DbSet<Lessor> Lessor { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductSurcharge> ProductSurchange { get; set; }
    public DbSet<Surcharge> Surchange { get; set; }
}