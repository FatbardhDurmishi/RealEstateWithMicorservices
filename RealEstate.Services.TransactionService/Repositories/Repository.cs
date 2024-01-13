using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using RealEstate.Services.TransactionService.Repositories.IRepositories;
using RealEstate.Services.TransactionService.Data;

namespace RealEstate.Services.TransactionService.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        internal DbSet<T> DbSet
        {
            get; set;
        }
        public Repository(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            this.DbSet = _db.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await DbSet.AddAsync(entity).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);

            }
            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return await query.AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query;

            query = DbSet;

            query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return await query.AsNoTracking().FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _db.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Update(T entity)
        {
            try
            {
                DbSet.Update(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateRange(IEnumerable<T> entity)
        {
            try
            {
                DbSet.UpdateRange(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
