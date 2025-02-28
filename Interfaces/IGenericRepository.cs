

namespace CoolFormApi.Interfaces;

public interface IGenericRepository<T> where T : class
{
    public Task<List<T>> GetAllAsync();
    public Task<T?> GetByIdAsync(int id);
    public Task<T> CreateAsync(T entity);
    public Task UpdateAsync(T entity);
    public Task DeleteAsync(T entity);
    public Task DeleteAsync(int id);
    public IQueryable<T> GetQueryable();
    public Task DeleteRangeAsync(IEnumerable<T> entities);
    public Task CreateRangeAsync(IEnumerable<T> entities);
}