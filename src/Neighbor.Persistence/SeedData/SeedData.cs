﻿using Microsoft.Extensions.Configuration;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Enumarations.Authentication;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.SeedData;

public static class SeedData
{
    public static void Seed(ApplicationDbContext context, IConfiguration configuration, IPasswordHashService passwordHashService)
    {
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
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                Category.CreateCategory("Couch", false),
                Category.CreateCategory("Table", false),
                Category.CreateCategory("Electronic", false),
                Category.CreateCategory("Decorations", false),
                Category.CreateCategory("Bed", false),
                Category.CreateCategory("Cabinet", false),
                Category.CreateCategory("Car", true),
                Category.CreateCategory("Motorbike", true),
                Category.CreateCategory("Bike", true)
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
