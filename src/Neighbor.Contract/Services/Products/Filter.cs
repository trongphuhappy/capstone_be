namespace Neighbor.Contract.Services.Products;

public static class Filter
{
    public record ProductFilter(Guid? Id, string? Name, string? Description);
}
