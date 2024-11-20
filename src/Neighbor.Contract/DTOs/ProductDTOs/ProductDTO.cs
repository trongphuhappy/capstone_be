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
        public DateTime? IssueDate { get; private set; }
        public DateTime? ExpirationDate { get; private set; }
        public List<IFormFile>? InsuranceImages { get; private set; }
        public List<SurchargeDTO>? Surcharges { get; set; }
    }
}
