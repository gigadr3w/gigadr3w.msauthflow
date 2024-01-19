using System.Linq.Expressions;

namespace gigadr3w.msauthflow.dataaccess.Interfaces
{
    public interface IDataAccess<T>  where T : class
    {
        Task<List<T>> List();
        Task Add(T entity);
        Task Delete(T entity);
        Task Update(T entity);
        Task<T?> Get(int Id);
        //Task<T> Detach(T entity);
        Task<IQueryable<T?>> Where(Expression<Func<T, bool>> predicate);
    }
}