using Neighbor.Contract.Enumarations.MessagesList;

namespace Neighbor.Domain.Exceptions;

public static class CategoryException
{
    public sealed class CategoriesNotFoundException : NotFoundException
    {
        public CategoriesNotFoundException()
            : base(MessagesList.CategoryNotFoundAnyException.GetMessage().Message,
                   MessagesList.CategoryNotFoundAnyException.GetMessage().Code)
        { }
    }
}
