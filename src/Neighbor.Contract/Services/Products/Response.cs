using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Enumarations.Product;

namespace Neighbor.Contract.Services.Products;

public static class Response
{
    public record ProductResponse(Guid Id, string Name, StatusType StatusType, string Policies, string Description, double Rating, double Price, double Value, ConfirmStatus ConfirmStatus, CategoryDTO Category, List<string> ProductImagesUrl, InsuranceDTO.InsuranceResponseDTO Insurance, List<SurchargeDTO.SurchargeResponseDTO> Surcharges);
}
