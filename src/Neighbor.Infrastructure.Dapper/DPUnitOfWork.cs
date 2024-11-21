using Neighbor.Domain.Abstraction.Dappers;
using Neighbor.Domain.Abstraction.Dappers.Repositories;

namespace Neighbor.Infrastructure.Dapper;
public class DPUnitOfWork : IDPUnitOfWork
{
    public DPUnitOfWork(IAccountRepository accountRepository, ICategoryRepository categoryRepository, ISurchargeRepository surchargeRepository)
    {
        AccountRepositories = accountRepository;
        CategoryRepositories = categoryRepository;
        SurchargeRepositories = surchargeRepository;
    }
    public IAccountRepository AccountRepositories { get; }

    public ICategoryRepository CategoryRepositories { get; }

    public ISurchargeRepository SurchargeRepositories { get; }
    public IProductRepository ProductRepositories { get; }
    public ILessorRepository LessorRepositories { get; }
}
