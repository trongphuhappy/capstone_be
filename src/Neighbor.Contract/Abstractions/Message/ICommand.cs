using MediatR;
using Neighbor.Contract.Abstractions.Shared;

namespace Neighbor.Contract.Abstractions.Message;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}