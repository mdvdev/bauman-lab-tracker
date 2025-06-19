namespace LabTracker.Shared.Contracts;

public interface ICrudRepository<TEntity, in TKey>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TKey labId);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> CreateAsync(TEntity course);
    Task<TEntity> UpdateAsync(TEntity course);
    Task DeleteAsync(TKey id);
}