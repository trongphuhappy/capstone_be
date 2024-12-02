using AutoMapper;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Abstraction.Dappers;
using static Neighbor.Contract.Services.Orders.Response;

namespace Neighbor.Application.UseCases.V2.Queries.Orders;
public sealed class GetOrderByIdQueryHandler : IQueryHandler<Query.GetOrderByIdQuery, Success<OrderResponse>>
{
    private readonly IDPUnitOfWork _dpUnitOfWork;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(IDPUnitOfWork dpUnitOfWork, IMapper mapper)
    {
        _dpUnitOfWork = dpUnitOfWork;
        _mapper = mapper;
    }

    public Task<Result<Success<OrderResponse>>> Handle(Query.GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
