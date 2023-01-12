using AuthServer.Models;
using AuthServer.ViewModels;

namespace UserService.API.IRepositories
{
    public interface IUserRepository : IGenericRepository<AppUser>
    {
        Task<AppUser> AddUser(AppUser user);
        Task<AppUser> GetByEmailAndPassword(string email, string password);
        Task<AppUser> GetByEmail(string email);
        Task<AppUser> GetUserByMobileNo(string mobileNo);
        Task<UserRole?> GetUserRoleByEmail(string email);

        Task<IEnumerable<AppUser>> GetUsersByRole(UserRole role);

        Task<AppUser> GetUserById(Guid id);

        Task<int> UpdatePassword(AppUser user);
        Task DeactivateUserByUserId(Guid userId);

        IEnumerable<AppUser> GetUsersByUserRoles(int[] userRoles);

        Task<IEnumerable<AppUser>> GetAllUsersCreatedByAUser(Guid creatorId);

        Task<AppUser> GetUserByMobileAndReference(string mobileNo, string reference);

        Task<List<AppUser>> GetAllUsers(int rows, int pageNumber);

        Task<int> GetAllUserCount();

        Task<IEnumerable<AppUser>> GetAllUsersByUserIds(Guid[] userIds);
    }
}
