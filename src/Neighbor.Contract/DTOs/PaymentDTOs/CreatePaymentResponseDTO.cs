namespace Neighbor.Contract.DTOs.PaymentDTOs;

public sealed class CreatePaymentResponseDTO
{
    public bool Success { get; set; }
    public string PaymentUrl { get; set; }
    public string Message { get; set; }

    public CreatePaymentResponseDTO(bool success, string paymentUrl, string message)
    {
        Success = success;
        PaymentUrl = paymentUrl;
        Message = message;
    }
}
