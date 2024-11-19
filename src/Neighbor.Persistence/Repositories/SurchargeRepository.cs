using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class SurchargeRepository(ApplicationDbContext context) : RepositoryBase<Surcharge, Guid>(context), ISurchargeRepository
{
}
