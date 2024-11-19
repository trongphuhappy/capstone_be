namespace Neighbor.Contract.Services.Categories;

public static class Response
{
    public record CategoryResponse(int Id, string Name, bool IsVehicle);
}
