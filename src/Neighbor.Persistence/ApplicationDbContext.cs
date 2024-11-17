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
}