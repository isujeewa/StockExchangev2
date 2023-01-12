using UserService.API.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UserService.API.Repositories
{
    /// <summary>
    /// Class implementation for generic repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext;

        public GenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            var item = await _dbContext.Set<TEntity>().AddAsync(entity);
            return item.Entity;
        }

        public async Task Delete(Guid id)
        {
            TEntity existingItem = await _dbContext.Set<TEntity>().FindAsync(id);
            if (existingItem != null)
                _dbContext.Set<TEntity>().Remove(existingItem);
        }

        public TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
                                          Expression<Func<TEntity, bool>> predicate = null,
                                          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                         Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                          bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).FirstOrDefault();
            }
            else
            {
                return query.Select(selector).FirstOrDefault();
            }
        }

        public IEnumerable<TResult> GetAllOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
                                         Expression<Func<TEntity, bool>> predicate = null,
                                         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                         bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector);
            }
            else
            {
                return query.Select(selector);
            }
        }

        public async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public async Task<IList<TEntity>> GetAll()
        {
            return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<TEntity> GetById(Guid id)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(id);
            if (entity != null)
            {
                _dbContext.Entry(entity).State = EntityState.Detached;
            }
            return entity;
            //return await _dbContext.Set<TEntity>().AsNoTracking().FindAsync(id);
        }

        public void Update(TEntity entity, object id)
        {
            TEntity existingItem = _dbContext.Set<TEntity>().Find(id);
            if (existingItem != null)
            {
                _dbContext.Entry(existingItem).CurrentValues.SetValues(entity);
            }
            //_dbContext.Attach(entity);
            //_dbContext.Entry(entity).CurrentValues.SetValues(entity);
            //_dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Update(TEntity entity)
        {
            _dbContext.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        //public async Task Add(TEntity entity)
        //{
        //    await _dbContext.Set<TEntity>().e(entity);
        //}

    }
}
