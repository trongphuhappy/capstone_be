﻿using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class InsuranceRepository(ApplicationDbContext context) : RepositoryBase<Insurance, Guid>(context), IInsuranceRepository
{
}
