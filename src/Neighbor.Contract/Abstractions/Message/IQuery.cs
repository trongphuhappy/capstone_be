using MediatR;
using Neighbor.Contract.Abstractions.Shared;

namespace Neighbor.Contract.Abstractions.Message;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}