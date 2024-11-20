using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;

namespace Neighbor.Persistence;
public class EFUnitOfWork : IEFUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public EFUnitOfWork(ApplicationDbContext context, IAccountRepository accountRepository, ISurchargeRepository surchargeRepository, IProductRepository productRepository, IProductSurchargeRepository productSurchargeRepository, IInsuranceRepository insuranceRepository, IImagesRepository imagesRepository)
    {
        _context = context;
        AccountRepository = accountRepository;
        SurchargeRepository = surchargeRepository;
        ProductRepository = productRepository;
        ProductSurchargeRepository = productSurchargeRepository;
        InsuranceRepository = insuranceRepository;
        ImagesRepository = imagesRepository;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
          => await _context.DisposeAsync();

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    => await _context.SaveChangesAsync();

    public IAccountRepository AccountRepository { get; }

    public ISurchargeRepository SurchargeRepository { get; }

    public IProductRepository ProductRepository {  get; }
    public IProductSurchargeRepository ProductSurchargeRepository { get; }
    public IImagesRepository ImagesRepository { get; }

    public IInsuranceRepository InsuranceRepository { get; }
}