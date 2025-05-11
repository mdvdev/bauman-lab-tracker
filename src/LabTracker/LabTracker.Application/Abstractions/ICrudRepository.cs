namespace LabTracker.Application.Contracts;

public interface ICrudRepository<TEntity, TKey>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TKey key);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TKey> CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TKey key);
}