using MediatR;
using Neighbor.Contract.Abstractions.Shared;

namespace Neighbor.Contract.Abstractions.Message;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
