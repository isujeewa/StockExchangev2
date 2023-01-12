using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UserService.API.IRepositories
{
    /// <summary>
    /// Interface for generic repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IList<TEntity>> GetAll();

        Task<TEntity> GetById(Guid id);

        Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");

        TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
                                          Expression<Func<TEntity, bool>> predicate = null,
                                          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                         Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                          bool disableTracking = true);

        IEnumerable<TResult> GetAllOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
                                      Expression<Func<TEntity, bool>> predicate = null,
                                      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                     Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                      bool disableTracking = true);

        Task<TEntity> Add(TEntity entity);

        void Update(TEntity entity, object id);

        void Update(TEntity entity);


        Task Delete(Guid id);
    }
}
