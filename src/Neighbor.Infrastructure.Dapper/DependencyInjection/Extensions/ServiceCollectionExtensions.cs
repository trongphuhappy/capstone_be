﻿using Microsoft.Extensions.DependencyInjection;
using Neighbor.Domain.Abstraction.Dappers;
using Neighbor.Domain.Abstraction.Dappers.Repositories;
using Neighbor.Infrastructure.Dapper.Repositories;

namespace Neighbor.Infrastructure.Dapper.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureDapper(this IServiceCollection services)
        => services.AddTransient<IDPUnitOfWork, DPUnitOfWork>()
                   .AddTransient<IAccountRepository, AccountRepository>()
                   .AddTransient<ICategoryRepository, CategoryRepository>()
                   .AddTransient<ISurchargeRepository, SurchargeRepository>()
                   .AddTransient<IProductRepository, ProductRepository>()
                   .AddTransient<ILessorRepository, LessorRepository>()
                   .AddTransient<ICategoryRepository, CategoryRepository>()
                   .AddTransient<IWishlistRepository, WishlistRepository>()
                   .AddTransient<IOrderRepository, OrderRepository>()
                   .AddTransient<IFeedbackRepository, FeedbackRepository>()
                   .AddTransient<IStatisticRepository, StatisticRepository>()
                   .AddTransient<IWalletRepository, WalletRepository>();

}
