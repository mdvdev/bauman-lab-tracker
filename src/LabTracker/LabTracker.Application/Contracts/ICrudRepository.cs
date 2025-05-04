namespace LabTracker.Application.Contracts;

public interface ICrudRepository<TEntity, in TKey>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TKey key);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TKey key);
}