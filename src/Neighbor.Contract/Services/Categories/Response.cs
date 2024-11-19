namespace Neighbor.Contract.Services.Categories;

public static class Response
{
    public record CategoriesResponse(int Id, string Name, bool IsVehicle);
}
