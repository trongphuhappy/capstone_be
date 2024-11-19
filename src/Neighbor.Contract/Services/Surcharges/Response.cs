namespace Neighbor.Contract.Services.Surcharges;

public static class Response
{
    public record SurchargeResponse(Guid Id, string Name, string Description);
}
