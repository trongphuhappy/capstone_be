namespace Neighbor.Contract.Services.Surcharges;

public static class Response
{
    public record SurchargeResponse(int Id, string Name, bool IsVehicle);
}
