﻿using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;

namespace Neighbor.Persistence;
public class EFUnitOfWork : IEFUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public EFUnitOfWork(ApplicationDbContext context, IAccountRepository accountRepository, ISurchargeRepository surchargeRepository, IProductRepository productRepository, IProductSurchargeRepository productSurchargeRepository, IInsuranceRepository insuranceRepository, IImagesRepository imagesRepository, ICategoryRepository categoryRepository, ILessorRepository lessorRepository, IWishlistRepository wishlistRepository, IOrderRepository orderRepository, IWalletRepository walletRepository, ITransactionRepository transactionRepository, IFeedbackRepository feedbackRepository)
    {
        _context = context;
        AccountRepository = accountRepository;
        SurchargeRepository = surchargeRepository;
        ProductRepository = productRepository;
        ProductSurchargeRepository = productSurchargeRepository;
        InsuranceRepository = insuranceRepository;
        ImagesRepository = imagesRepository;
        CategoryRepository = categoryRepository;
        LessorRepository = lessorRepository;
        WishlistRepository = wishlistRepository;
        OrderRepository = orderRepository;
        WalletRepository = walletRepository;
        TransactionRepository = transactionRepository;
        FeedbackRepository = feedbackRepository;
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
    public ICategoryRepository CategoryRepository { get; }
    public ILessorRepository LessorRepository { get; }
    public IWishlistRepository WishlistRepository { get; }
    public IOrderRepository OrderRepository { get; }

    public IWalletRepository WalletRepository { get; }

    public ITransactionRepository TransactionRepository { get; }

    public IFeedbackRepository FeedbackRepository { get; }
}