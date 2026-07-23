using Microsoft.EntityFrameworkCore;
using StartupWebAPIs.Data;
using StartupWebAPIs.Interfaces;
using System.Linq.Expressions;

namespace StartupWebAPIs.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}