using System.Linq.Expressions;

namespace RealEstate.Services.TransactionService.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        Task AddAsync(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<int> SaveChangesAsync();
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entity);
        void Dispose();
    }
}
