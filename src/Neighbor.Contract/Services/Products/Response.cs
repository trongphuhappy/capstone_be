namespace Neighbor.Contract.Services.Products;

public static class Response
{
    public record ProductResponse(Guid Id, string Name, string Description);
}
