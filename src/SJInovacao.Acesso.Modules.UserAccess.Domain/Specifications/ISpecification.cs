namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T entity);
    }
}
