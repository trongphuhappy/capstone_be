﻿using Microsoft.Extensions.Configuration;
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
                Account.CreateAccountAdminLocal(configuration["AccountAdmin:Email"], configuration["AccountAdmin:Password"], configuration["UserConfiguration:DefaultMaleAvatar"])
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
                Category.CreateCategory("Couch", false, false),
                Category.CreateCategory("Table", false, false),
                Category.CreateCategory("Electronic", false, false),
                Category.CreateCategory("Decorations", false, false),
                Category.CreateCategory("Bed", false, false),
                Category.CreateCategory("Cabinet", false, false),
                Category.CreateCategory("Car", true, false),
                Category.CreateCategory("Motorbike", true, false),
                Category.CreateCategory("Bike", true, false)
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
