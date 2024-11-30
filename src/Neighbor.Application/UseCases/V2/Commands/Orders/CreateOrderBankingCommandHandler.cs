using Microsoft.Extensions.Options;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.PaymentDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Enumarations.Product;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Exceptions;
using PawFund.Contract.Settings;

namespace Neighbor.Application.UseCases.V2.Commands.Orders;

public sealed class CreateOrderBankingCommandHandler : ICommandHandler<Command.CreateOrderBankingCommand>
{
    private readonly IPaymentService _paymentService;
    private readonly PayOSSetting _payOSSetting;
    private readonly IResponseCacheService _responseCacheService;
    private readonly IEFUnitOfWork _efUnitOfWork;

    public CreateOrderBankingCommandHandler(IPaymentService paymentService, IOptions<PayOSSetting> payOSConfiguration, IResponseCacheService responseCacheService, IEFUnitOfWork efUnitOfWork)
    {
        _paymentService = paymentService;
        _payOSSetting = payOSConfiguration.Value;
        _responseCacheService = responseCacheService;
        _efUnitOfWork = efUnitOfWork;
    }

    public async Task<Result> Handle(Command.CreateOrderBankingCommand request, CancellationToken cancellationToken)
    {
        var product = await _efUnitOfWork.ProductRepository.FindByIdAsync(request.ProductId);

        //CreateOrderBankingCommandHandler
        var accountFound = await _efUnitOfWork.AccountRepository.FindByIdAsync(request.AccountId);
        if (accountFound == null)
        {
            throw new AccountException.AccountNotFoundException();
        }
        var productFound = await _efUnitOfWork.ProductRepository.FindByIdAsync(request.ProductId);
        if (productFound == null)
        {
            throw new ProductException.ProductNotFoundException();
        }
        if (productFound.Lessor.AccountId == request.AccountId)
        {
            throw new OrderException.ProductBelongsToUserException();
        }
        var isUserOrderProduct = await _efUnitOfWork.OrderRepository.AnyAsync(order => order.ProductId == request.ProductId && order.AccountId == request.AccountId && order.OrderStatus != OrderStatusType.RejectionValidated && order.OrderStatus != OrderStatusType.RejectionInvalidated);
        if (isUserOrderProduct)
        {
            throw new OrderException.ProductAlreadyOrderedByUserException();
        }
        if (productFound.StatusType != StatusType.Available)
        {
            throw new OrderException.ProductOrderedByAnotherUserException();
        }
        if (productFound.ConfirmStatus != ConfirmStatus.Approved)
        {
            throw new OrderException.ProductNotApprovedByAdminException();
        }

        long orderId = new Random().Next(1, 100000);

        // Create payment dto
        List<ItemDTO> itemDTOs = new List<ItemDTO> { new ItemDTO($"Order_{orderId}", 1, (int)product.Price) };
        var createPaymentDto = new CreatePaymentDTO(orderId, $"Rent {product.Name}", itemDTOs, _payOSSetting.ErrorUrl, _payOSSetting.SuccessUrl + $"?orderId={orderId}");
        var result = await _paymentService.CreatePaymentLink(createPaymentDto);
        // Save memory to when success or fail will know value
        await _responseCacheService.SetCacheResponseAsync($"order_{orderId}", request, TimeSpan.FromMinutes(60));
        
        return Result.Success(new Success<CreatePaymentResponseDTO>("", "", result));
    }
}
