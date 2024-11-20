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

    public sealed class CategoryNotFoundException : NotFoundException
    {
        public CategoryNotFoundException()
            : base(MessagesList.CategoryNotFoundException.GetMessage().Message,
                   MessagesList.CategoryNotFoundException.GetMessage().Code)
        { }
    }

    public sealed class CategoryMissingInsuranceException : BadRequestException
    {
        public CategoryMissingInsuranceException()
            : base(MessagesList.CategoryMissingInsuranceException.GetMessage().Message,
                   MessagesList.CategoryMissingInsuranceException.GetMessage().Code)
        { }
    }
}
