using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class CategoryRepository(ApplicationDbContext context) : RepositoryBase<Category, int>(context), ICategoryRepository
{
}
