using Microsoft.AspNetCore.Http;

namespace Neighbor.Contract.DTOs.ProductDTOs;
public static class InsuranceDTO
{
    public class InsuranceRequestDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? IssueDate { get;  set; }
        public DateTime? ExpirationDate { get;  set; }
        public List<IFormFile>? InsuranceImages { get;  set; }
    }

    public class InsuranceResponseDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime IssueDate { get;  set; }
        public DateTime ExpirationDate { get;  set; }
        public List<string> InsuranceImagesUrl { get;  set; }
    }
}
