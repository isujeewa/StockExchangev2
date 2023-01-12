using UserService.API.IRepositories;
using System.Threading.Tasks;

namespace UserService.API.IServices
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }

        // common method to get the reposiotory if specific repo is not available
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        Task<int> Commit();

        Task<int> SaveChangesAsync();

        int SaveChanges();
    }
}
