using System.Linq.Expressions;

namespace RealEstate.Services.TransactionService.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        Task Add(T entity);
        Task Remove(T entity);
        Task RemoveRange(IEnumerable<T> entities);
        Task<int> SaveChanges();
        Task Update(T entity);
        Task UpdateRange(IEnumerable<T> entity);
    }
}
