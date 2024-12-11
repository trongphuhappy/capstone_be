using Neighbor.Domain.Abstraction.Dappers;
using Neighbor.Domain.Abstraction.Dappers.Repositories;

namespace Neighbor.Infrastructure.Dapper;
public class DPUnitOfWork : IDPUnitOfWork
{
    public DPUnitOfWork(IAccountRepository accountRepository, ICategoryRepository categoryRepository, ISurchargeRepository surchargeRepository, IProductRepository productRepository, ILessorRepository lessorRepository, IWishlistRepository wishlistRepositories, IOrderRepository orderRepositories, IFeedbackRepository feedbackRepository, IStatisticRepository statisticRepositories, IWalletRepository walletRepositories)
    {
        AccountRepositories = accountRepository;
        CategoryRepositories = categoryRepository;
        SurchargeRepositories = surchargeRepository;
        ProductRepositories = productRepository;
        LessorRepositories = lessorRepository;
        WishlistRepositories = wishlistRepositories;
        OrderRepositories = orderRepositories;
        FeedbackRepositories = feedbackRepository;
        StatisticRepositories = statisticRepositories;
        WalletRepositories = walletRepositories;
    }
    public IAccountRepository AccountRepositories { get; }

    public ICategoryRepository CategoryRepositories { get; }

    public ISurchargeRepository SurchargeRepositories { get; }
    public IProductRepository ProductRepositories { get; }
    public ILessorRepository LessorRepositories { get; }
    public IWishlistRepository WishlistRepositories { get; }
    public IOrderRepository OrderRepositories { get; }
    public IFeedbackRepository FeedbackRepositories { get; }
    public IStatisticRepository StatisticRepositories { get; }
    public IWalletRepository WalletRepositories { get; }
}
