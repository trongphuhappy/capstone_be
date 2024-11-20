using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class ImagesRepository(ApplicationDbContext context) : RepositoryBase<Images, Guid>(context), IImagesRepository
{
}
