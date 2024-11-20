using Microsoft.AspNetCore.Http;
using static Neighbor.Contract.DTOs.ProductDTOs.InsuranceDTO;

namespace Neighbor.Contract.DTOs.ProductDTOs;
public static class ProductDTO
{
    public class ProductRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
        public double Price { get; set; }
        public string Policies { get; set; }
        public int CategoryId { get; set; }
        public List<IFormFile> ProductImages { get; set; }
        public string? InsuranceName { get; set; }
        public string? InsuranceDescription { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public List<IFormFile>? InsuranceImages { get; set; }
        public Guid SurchargeId { get; set; }
        public double SurchargePrice { get; set; }
    }

    public class ProductResponseDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
        public double Price { get; set; }
        public string Policies { get; set; }
        public int CategoryId { get; set; }
        public List<string> ProductImagesUrl { get; set; }
        public InsuranceResponseDTO Insurance { get; set; }
        public List<SurchargeDTO.SurchargeResponseDTO> Surcharges { get; set; }
    }


}
