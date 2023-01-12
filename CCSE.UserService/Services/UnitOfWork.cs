using AuthServer.Data;
using AuthServer.Utilities;
using Microsoft.EntityFrameworkCore;
using UserService.API.IRepositories;
using UserService.API.IServices;
using UserService.API.Repositories;

namespace UserService.API.Services
{
    /// <summary>
    /// Implementation for unit of work
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserDbContext _dbContext;

        private Dictionary<Type, object> repositories;

        private IUserRepository _userRepository;

        public UnitOfWork(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IUserRepository UserRepository
        {
            get
            {
                _userRepository = new UserRepository(_dbContext, this);
                return _userRepository;
            }
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (repositories == null)
            {
                repositories = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);
            if (!repositories.ContainsKey(type))
            {
                repositories[type] = new GenericRepository<TEntity>(_dbContext);
            }

            return (IGenericRepository<TEntity>)repositories[type];
        }

        public async Task<int> Commit()
        {
            int returnCode = 0;
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // saves the changes to the database
                        await _dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        returnCode = Constants.TransactionSuccessfullyCompleted;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        returnCode = Constants.TransactionFailedAndRollBackOccured;

                    }
                }
            });

            return returnCode;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}
