namespace Neighbor.Contract.Services.Surcharges;

public static class Filter
{
    public record SurchargeFilter(Guid? Id, string? Name, string? Description);
}
