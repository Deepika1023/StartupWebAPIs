using System.Linq.Expressions;

namespace StartupWebAPIs.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task SaveAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}