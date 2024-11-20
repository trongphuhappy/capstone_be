
using Microsoft.AspNetCore.Http;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.DTOs.ProductDTOs;
namespace Neighbor.Contract.Services.Products;
public static class Command
{
    public record CreateProductCommand(string Name, string Description, double Value, double Price, string Policies, int CategoryId, Guid UserId, List<IFormFile> ProductImages, InsuranceDTO.InsuranceRequestDTO? Insurance, List<SurchargeDTO>? Surcharges) : ICommand;
}

