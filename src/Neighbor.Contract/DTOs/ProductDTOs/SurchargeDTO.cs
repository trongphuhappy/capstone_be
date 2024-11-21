namespace Neighbor.Contract.DTOs.ProductDTOs;
public static class SurchargeDTO
{
    public class SurchargeRequestDTO
    {
        public Guid? SurchargeId { get; set; }
        public double? Price { get; set; }
    }

    public class SurchargeResponseDTO
    {
        public Guid SurchargeId { get; set; }
        public string SurchargeName { get; set; }
        public double Price { get; set; }

    }
}
