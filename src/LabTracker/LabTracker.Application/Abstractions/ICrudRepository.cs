namespace LabTracker.Application.Abstractions;

public interface ICrudRepository<TEntity, in TKey>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TKey key);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> CreateAsync(TEntity course);
    Task<TEntity> UpdateAsync(TEntity course);
    Task DeleteAsync(TKey key);
}