using Neighbor.Domain.Abstraction.Dappers.Repositories;

namespace Neighbor.Domain.Abstraction.Dappers;

public interface IDPUnitOfWork
{
    IAccountRepository AccountRepositories { get; }
    ICategoryRepository CategoryRepositories { get; }
    ISurchargeRepository SurchargeRepositories { get; }
    IProductRepository ProductRepositories { get; }
    ILessorRepository LessorRepositories { get; }
    IWishlistRepository WishlistRepositories { get; }
    IOrderRepository OrderRepositories { get; }
    IFeedbackRepository FeedbackRepositories { get; }
    IStatisticRepository StatisticRepositories { get; }
    IWalletRepository WalletRepositories { get; }
}
