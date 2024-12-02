using Microsoft.Extensions.Configuration;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Enumarations.Authentication;
using Neighbor.Contract.Enumarations.PaymentMethod;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.SeedData;

public static class SeedData
{
    public static void Seed(ApplicationDbContext context, IConfiguration configuration, IPasswordHashService passwordHashService)
    {
        Guid userId = Guid.NewGuid();
        if (!context.RoleUsers.Any())
        {
            context.RoleUsers.AddRange(
                new RoleUser
                {
                    Id = RoleType.Admin,
                    RoleName = "Admin",
                },
                new RoleUser
                {
                    Id = RoleType.Member,
                    RoleName = "Member"
                }
            );
        }
        if (!context.Accounts.Any())
        {
            context.Accounts.AddRange(
                Account.CreateAccountAdminLocal(configuration["AccountAdmin:Email"], passwordHashService.HashPassword(configuration["AccountAdmin:Password"]), configuration["UserConfiguration:DefaultMaleAvatar"])
            );
        }

        if (!context.PaymentMethods.Any())
        {
            context.PaymentMethods.AddRange(
                new PaymentMethod
                {
                    Id = PaymentMethodType.Cash,
                    MethodName = "Cash",
                },
                new PaymentMethod
                {
                    Id = PaymentMethodType.Banking,
                    MethodName = "Banking"
                }
            );
        }
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                Category.CreateCategory("Couch", "https://cdn.thewirecutter.com/wp-content/media/2023/05/sofabuyingguide-2048px-benchmademoderncream.jpg", false, false),
                Category.CreateCategory("Table", "https://cdn-images.article.com/products/SKU404/2890x1500/image150364.jpg", false, false),
                Category.CreateCategory("Electronic", "https://img.freepik.com/premium-photo/collection-electronic-gadgets-accessories-including-headphones-cameras-game-controllers-laptop-other-tech-devices_1187703-129882.jpg", false, false),
                Category.CreateCategory("Decorations", "https://i.insider.com/673bf9c3192f525898594860?width=800&format=jpeg&auto=webp", false, false),
                Category.CreateCategory("Bed", "https://i5.walmartimages.com/seo/Queen-Bed-Frame-New-Upgraded-Velvet-Upholstered-Platform-Headboard-Drawer-Modern-Size-Storage-Frame-Bedroom-Furniture-Hold-500lbs-No-Box-Spring-Neede_0991e74f-b3f6-44e7-a7fb-777da4cf795a.a68243c79dd3a430dc2da593e3a30575.jpeg", false, false),
                Category.CreateCategory("Cabinet", "https://media.homeboxstores.com/i/homebox/165270290-165270290-HMBX26012023Q_01-2100.jpg?fmt=auto&$quality-standard$&sm=c&$prodimg-m-sqr-pdp-2x$", false, false),
                Category.CreateCategory("Car", "https://www.kbb.com/wp-content/uploads/2022/08/2022-mercedes-amg-eqs-front-left-3qtr.jpg?w=918", true, false),
                Category.CreateCategory("Motorbike", "https://res.cloudinary.com/adrenalinecomau/image/upload/q_auto,f_auto/v1708303894/adventures/eps_24644.jpg", true, false),
                Category.CreateCategory("Bike", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR5EvpmYgBCKkGeQOyz4xod3ySqq5pp2uGf-dzMBo6LSULBjFHTu7tDANGp4Skcvy9uq-E&usqp=CAU", true, false)
            );
        }
        if (!context.Surcharges.Any())
        {
            context.Surcharges.AddRange(
                Surcharge.CreateSurcharge(Guid.NewGuid(), "Late Fees", "Additional charges for delayed payments", false),
                Surcharge.CreateSurcharge(Guid.NewGuid(), "Sanity Fees", "Charges for ensuring compliance with policies or processes", false),
                Surcharge.CreateSurcharge(Guid.NewGuid(), "Damage Fees", "Charges for repair or replacement due to damages caused", false)
            );
        }
        context.SaveChanges();
    }
}
