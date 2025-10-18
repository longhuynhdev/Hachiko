using System.Linq.Expressions;

namespace Hachiko.DataAccess.Repository.IRepository
{
    //Generic <T>
    public interface IRepository <T> where T : class
    {
        // T 
        IEnumerable<T> GetAll(Expression<Func<T,bool>>? filter= null, string? includeProperties = null, bool IsTracked = false);
        T Get(Expression<Func<T,bool>> filter, string? includeProperties = null, bool IsTracked = false);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
         
    }
}
