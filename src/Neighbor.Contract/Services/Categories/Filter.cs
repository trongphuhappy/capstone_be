namespace Neighbor.Contract.Services.Categories;

public static class Filter
{
    public record CategoryFilter(int? Id, string? Name, bool? IsVehicle);
}
