namespace Producer.Core.Interfaces;

public interface IBaseRepository<E, T> where E : BaseEntity
{
    Task<IEnumerable<E>> GetAsync();
    Task<E> GetByIdAsync(T id);
    Task<E> AddAsync(E entity);
    Task<E> UpdateAsync(E entity);
    Task<bool> RemoveAsync(T id);
}
