using Neighbor.Contract.DTOs.PaymentDTOs;

namespace Neighbor.Contract.Abstractions.Services;

public interface IPaymentService
{
    Task<CreatePaymentResponseDTO> CreatePaymentLink(CreatePaymentDTO paymentData);
    Task<bool> CancelOrder(long orderId);
}
