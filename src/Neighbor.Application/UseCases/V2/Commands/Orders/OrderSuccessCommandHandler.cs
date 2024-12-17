using MediatR;
using Microsoft.Extensions.Options;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.Product;
using Neighbor.Contract.Services.Orders;
using Neighbor.Contract.Settings;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;
using Newtonsoft.Json;

namespace Neighbor.Application.UseCases.V2.Commands.Orders;

public sealed class OrderSuccessCommandHandler : ICommandHandler<Command.OrderSuccessCommand, Success<Response.OrderSuccess>>
{
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IPublisher _publisher;
    private readonly IResponseCacheService _responseCacheService;
    private readonly ClientSetting _clientSetting;

    public OrderSuccessCommandHandler(IEFUnitOfWork efUnitOfWork, IPublisher publisher, IResponseCacheService responseCacheService, IOptions<ClientSetting> clientConfiguration)
    {
        _efUnitOfWork = efUnitOfWork;
        _publisher = publisher;
        _responseCacheService = responseCacheService;
        _clientSetting = clientConfiguration.Value;
    }
    public async Task<Result<Success<Response.OrderSuccess>>> Handle(Command.OrderSuccessCommand request, CancellationToken cancellationToken)
    {
        // Get infomation saved in memory
        var orderMemory = await _responseCacheService.GetCacheResponseAsync($"order_{request.OrderId}");
        // Conver JSON to object
        var orderObject = JsonConvert.DeserializeObject<Command.CreateOrderBankingCommand>(orderMemory);

        // Find product
        var product = await _efUnitOfWork.ProductRepository.FindByIdAsync(orderObject.ProductId) ?? throw new ProductException.ProductNotFoundException();

        // Find lessor
        //var lessor = await _efUnitOfWork.LessorRepository.FindByIdAsync(product.LessorId, cancellationToken);
        var account = await _efUnitOfWork.AccountRepository.FindByIdAsync(orderObject.AccountId);
        //Find wallet
        var wallet = await _efUnitOfWork.WalletRepository.GetWalletByLessorId(product.LessorId);
        
        var orderCreated = Order.CreateOrder(orderObject.AccountId, orderObject.ProductId, product.LessorId, orderObject.RentTime, orderObject.ReturnTime, product.Lessor.WareHouseAddress, product.Price, request.OrderId);
        _efUnitOfWork.OrderRepository.Add(orderCreated);
        // Change status type product
        product.UpdateStatusType(StatusType.Not_Available);
        _efUnitOfWork.ProductRepository.Update(product);
        // Update wallet
        long orderValue = (long)((orderObject.ReturnTime - orderObject.RentTime).TotalDays) * (long)product.Price;
        wallet.AddMoney((long)(orderValue * 0.3), $"Member {orderObject.AccountId} rent {product.Id}");
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        // Delete cache order
        await _responseCacheService.DeleteCacheResponseAsync($"order_{request.OrderId}");
        // //Send success order email for Lessor
        await Task.WhenAll(
           _publisher.Publish(new DomainEvent.NotiLessorOrderSuccess(orderCreated.Id, product.Lessor.Account.Email, product.Name, product.Lessor.WareHouseAddress, orderObject.RentTime, account.Email), cancellationToken)
        );
        //Send success order email for User
        await Task.WhenAll(
           _publisher.Publish(new DomainEvent.NotiUserOrderSuccess(orderCreated.Id, product.Lessor.Account.Email, product.Name, product.Lessor.WareHouseAddress, orderObject.RentTime), cancellationToken)
       );
        var result = new Response.OrderSuccess($"{_clientSetting.Url}{_clientSetting.OrderSuccess}");
        return Result.Success(new Success<Response.OrderSuccess>("", "", result));
    }
}
