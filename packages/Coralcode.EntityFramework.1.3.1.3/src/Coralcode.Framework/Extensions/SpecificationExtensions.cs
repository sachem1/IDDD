using Coralcode.Framework.Data.Specification;

namespace Coralcode.Framework.Extensions
{
    public static class SpecificationExtensions
    {
        public static ISpecification<T> And<T>(this ISpecification<T> leftSide, ISpecification<T> rightSide) where T : class
        {
            return new AndSpecification<T>(leftSide, rightSide);
        }
        public static ISpecification<T> Or<T>(this ISpecification<T> leftSide, ISpecification<T> rightSide) where T : class
        {
            return new OrSpecification<T>(leftSide, rightSide);
        }
        public static ISpecification<T> Not<T>(this ISpecification<T> originalSpecification) where T : class
        {
            return new NotSpecification<T>(originalSpecification);
        }
    }
}
