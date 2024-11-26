using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.Product;
using Neighbor.Contract.Services.Members;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using static Neighbor.Domain.Exceptions.AuthenticationException;

namespace Neighbor.Application.UseCases.V2.Commands.Members;

public sealed class UpdateInfoLessorCommandHandler : ICommandHandler<Command.UpdateInfoLessorCommand>
{
    private readonly IEFUnitOfWork _efUnitOfWork;

    public UpdateInfoLessorCommandHandler(IEFUnitOfWork efUnitOfWork)
    {
        _efUnitOfWork = efUnitOfWork;
    }
    
    public async Task<Result> Handle(Command.UpdateInfoLessorCommand request, CancellationToken cancellationToken)
    {
        var user = await _efUnitOfWork.AccountRepository.FindByIdAsync(request.UserId);
        if (user.IsDeleted == true) throw new AccountBanned();
        var lessor = await _efUnitOfWork.LessorRepository.GetInformationLessorByAccountIdAsync(user.Id);
        if (lessor == null)
        {
            var newLessor = Lessor.CreateLessor(request.WareHouseAddress, request.ShopName, (LocationType)request.LocationType, request.UserId);
            _efUnitOfWork.LessorRepository.Add(newLessor);
        }
        else
            lessor.UpdateLessor(request.WareHouseAddress, request.ShopName, request.LocationType);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
