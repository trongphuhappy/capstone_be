
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Feedbacks;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using static Neighbor.Domain.Exceptions.FeedbackException;

namespace Neighbor.Application.UseCases.V2.Commands.Feedbacks;

public sealed class CreateFeedbackCommandHandler : ICommandHandler<Command.CreateFeedbackCommand>
{
    private readonly IEFUnitOfWork _efUnitOfWork;

    public CreateFeedbackCommandHandler(IEFUnitOfWork efUnitOfWork)
    {
        _efUnitOfWork = efUnitOfWork;
    }
    public async Task<Result> Handle(Command.CreateFeedbackCommand request, CancellationToken cancellationToken)
    {
        // Each feedback can only be created once, so check if the user has created feedback.
        var result = await _efUnitOfWork.FeedbackRepository.GetFeedbackByAccountId(request.AccountId);
        if (result != null)
            throw new AccountHasFeedbackException();

        // Create feedback and save database
        var feedback = Feedback.CreateFeedback(request.OrderId, request.AccountId, request.ProductId, request.Content);
        _efUnitOfWork.FeedbackRepository.Add(feedback);
        await _efUnitOfWork.SaveChangesAsync();
        
        return Result.Success(new Success(MessagesList.CreateFeedbackSuccessfully.GetMessage().Code, MessagesList.CreateFeedbackSuccessfully.GetMessage().Message));
    }
}
