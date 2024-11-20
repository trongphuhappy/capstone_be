using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;

namespace Neighbor.Domain.Abstraction.EntitiyFramework;

public interface IEFUnitOfWork : IAsyncDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    IAccountRepository AccountRepository { get; }
    ISurchargeRepository SurchargeRepository { get; }
    IProductRepository ProductRepository { get; }
    IProductSurchargeRepository ProductSurchargeRepository { get; }
    IInsuranceRepository InsuranceRepository { get; }
    IImagesRepository ImagesRepository { get; }
}
