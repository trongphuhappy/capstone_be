using MediatR;
using Microsoft.CodeAnalysis;
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
        var product = await _efUnitOfWork.ProductRepository.FindByIdAsync(orderObject.ProductId);
        if (product == null)
        {
            throw new ProductException.ProductNotFoundException();
        }
        var orderCreated = Order.CreateOrder(orderObject.AccountId, orderObject.ProductId, orderObject.RentTime, orderObject.ReturnTime, product.Lessor.WareHouseAddress, product.Price, request.OrderId);
        _efUnitOfWork.OrderRepository.Add(orderCreated);
        product.UpdateStatusType(StatusType.Not_Available);
        _efUnitOfWork.ProductRepository.Update(product);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        await _responseCacheService.DeleteCacheResponseAsync($"order_{request.OrderId}");
        // //Send success order email for Lessor
        // await Task.WhenAll(
        //    _publisher.Publish(new DomainEvent.NotiLessorOrderSuccess(orderCreated.Id, product.Lessor.Account.Email, product.Name, product.Lessor.WareHouseAddress, orderObject.RentTime, accountFound.Email), cancellationToken)
        // );
        // //Send success order email for User
        // await Task.WhenAll(
        //    _publisher.Publish(new DomainEvent.NotiUserOrderSuccess(orderCreated.Id, accountFound.Email, productFound.Name, productFound.Lessor.WareHouseAddress, request.RentTime), cancellationToken)
        //);
        var result = new Response.OrderSuccess($"{_clientSetting.Url}{_clientSetting.DonateSuccess}/{request.OrderId}");
        return Result.Success(new Success<Response.OrderSuccess>("", "", result));
    }
}
