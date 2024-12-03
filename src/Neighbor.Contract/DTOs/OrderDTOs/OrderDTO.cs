using Microsoft.AspNetCore.Http;
using static Neighbor.Contract.DTOs.ProductDTOs.InsuranceDTO;

namespace Neighbor.Contract.DTOs.ProductDTOs;
public static class OrderDTO
{
    public class OrderRequestDTO
    {
        public Guid ProductId { get; set; }
        public DateTime RentTime { get; set; }
        public DateTime ReturnTime { get; set; }
    }

    public class OrderRequestConfirmDTO
    {
        public Guid OrderId { get; set; }
        public bool IsApproved { get; set; }
        public string? RejectReason { get; set; }
    }

    public class OrderResponseDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
        public double Price { get; set; }
        public int MaximumRentDays { get; set; }
        public string Policies { get; set; }
        public int CategoryId { get; set; }
        public List<string> ProductImagesUrl { get; set; }
        public InsuranceResponseDTO Insurance { get; set; }
        public List<SurchargeDTO.SurchargeResponseDTO> Surcharges { get; set; }
    }


}
