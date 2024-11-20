using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class LessorRepository(ApplicationDbContext context) : RepositoryBase<Lessor, Guid>(context), ILessorRepository
{
}
