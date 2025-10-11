using System.Linq.Expressions;

namespace Gr8_VehicleManagement.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        // CRUD Operations
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        
        // Add Operations
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        
        // Update Operations
        Task<T> UpdateAsync(T entity);
        Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities);
        
        // Delete Operations
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteAsync(T entity);
        Task<bool> DeleteRangeAsync(IEnumerable<T> entities);
        
        // Count Operations
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        
        // Exists Operations
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        
        // Pagination
        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize);
        
        // Include Operations
        Task<IEnumerable<T>> GetWithIncludesAsync(params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdWithIncludesAsync(Guid id, params Expression<Func<T, object>>[] includes);
    }
}
