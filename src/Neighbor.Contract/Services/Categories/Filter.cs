namespace Neighbor.Contract.Services.Categories;

public static class Filter
{
    public record CategoryFilter(Guid? Id, string? Name, bool? IsVehicle);
}
