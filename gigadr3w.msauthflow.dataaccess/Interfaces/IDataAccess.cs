using System.Linq.Expressions;

namespace gigadr3w.msauthflow.dataaccess.Interfaces
{
    public interface IDataAccess<T>  where T : class
    {
        Task<List<T>> List();
        Task<List<T>> List(List<Expression<Func<T, object>>> includes);
        Task Add(T entity);
        Task Delete(T entity);
        Task Update(T entity);
        Task<T?> Get(int Id);
        Task<T?> Get(int Id, List<Expression<Func<T, object>>> includes);
        Task<IQueryable<T>> Where(Expression<Func<T, bool>> predicate);
        Task<IQueryable<T>> Where(Expression<Func<T, bool>> predicate, List<Expression<Func<T, object>>> includes);
    }
}