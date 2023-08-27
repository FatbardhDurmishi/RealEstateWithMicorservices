using Microsoft.EntityFrameworkCore;
using RealEstate.Services.AuthAPI.Data;
using RealEstate.Services.AuthAPI.Repositories.IRepository;
using System.Linq.Expressions;

namespace RealEstate.Services.AuthAPI.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        internal DbSet<T> dbSet { get; set; }
        public Repository(AppDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public async Task Add(T entity)
        {
           await dbSet.AddAsync(entity);
           await _db.SaveChangesAsync();
        }

        //includeProp - "Category, CoverType"
        public async  Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
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
            return await query.ToListAsync();
        }

        public async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query;

            query = dbSet;

            query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task Remove(T entity)
        {
            dbSet.Remove(entity);
           await _db.SaveChangesAsync();
        }

        public async Task RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
            await _db.SaveChangesAsync();
        }

        public async Task<int> SaveChanges()
        {
            try
            {
                return await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task Update(T entity)
        {
            try
            {
                _db.Set<T>().Update(entity);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task UpdateRange(IEnumerable<T> entity)
        {
            try
            {
                _db.Set<T>().UpdateRange(entity);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
