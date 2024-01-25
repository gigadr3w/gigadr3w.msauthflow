using gigadr3w.msauthflow.common.Exceptions;
using gigadr3w.msauthflow.dataaccess.Interfaces;
using gigadr3w.msauthflow.dataaccess.mysql.Contexes;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace gigadr3w.msauthflow.dataaccess.mysql
{
    public class MySQLDataAccess<T> : IDataAccess<T> where T : class
    {
        private readonly DataContext _dataContext;

        public MySQLDataAccess(DataContext dataContext)
            => _dataContext = dataContext;

        /// <summary>
        /// Manually handle the state change (deleted/updated) for the current entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="PropertyNotFoundException">If id key is not found.</exception>
        private async Task Detach(T entity)
        {
            Type entityType = typeof(T);
            if (entityType.GetProperty("Id") == null) throw new PropertyNotFoundException($"Id property not found in {nameof(entityType)} entity");

            int key = (int)entityType.GetProperty("Id").GetValue(entity);

            //to avoid tracking
            var entry = await _dataContext.Set<T>().FindAsync(key);
            if (entry != null)
            {
                _dataContext.Entry(entry).State = EntityState.Detached;
            }
        }

        private async Task ApplyChanges(T entity, EntityState state)
        {
            _dataContext.Entry(entity).State = state;
            await _dataContext.SaveChangesAsync();            
        }

        public Task Add(T entity)
            => ApplyChanges(entity, EntityState.Added);

        public async Task Delete(T entity)
        {
            await Detach(entity);
            await ApplyChanges(entity, EntityState.Deleted);
        }

        public Task<T?> Get(int Id)
            => _dataContext.Set<T>().FindAsync(Id).AsTask();

        public Task<T?> Get(int Id, List<Expression<Func<T, object>>> includes)
        {
            DbSet<T> query = _dataContext.Set<T>();

            foreach (Expression<Func<T, object>> include in includes)
            {
                query.Include(include);
            }

            return query.FindAsync(Id).AsTask();
        }

        public Task<List<T>> List()
            => _dataContext.Set<T>().ToListAsync();

        public Task<List<T>> List(List<Expression<Func<T, object>>> includes)
        {
            DbSet<T> query = _dataContext.Set<T>();

            foreach (Expression<Func<T, object>> include in includes) 
            {
                query.Include(include);
            }

            return query.ToListAsync();
        }

        public async Task Update(T entity)
        {
            await Detach(entity);
            await ApplyChanges(entity, EntityState.Modified);
        }

        public async Task<IQueryable<T>> Where(Expression<Func<T, bool>> predicate)
            => await Task.FromResult(_dataContext.Set<T>().Where(predicate));
        public async Task<IQueryable<T>> Where(Expression<Func<T, bool>> predicate, List<Expression<Func<T, object>>> includes)
        {
            IQueryable<T> query = _dataContext.Set<T>().Where(predicate);

            foreach (Expression<Func<T, object>> include in includes)
            {
                query = query.Include(include);
            }

            return await Task.FromResult(query);
        }
    }
}