using AuthServer.Models;

namespace UserService.API.IServices
{
    /// <summary>
    /// Interface for user service
    /// </summary>
    public interface IUserService
    {
        Task<AppUser> AddUser(AppUser user);

        Task<AppUser> GetByEmailAndPassword(string email, string password);

        Task<AppUser> GetUserByEmail(string email);

        Task<AppUser> GetUserByMobileNo(string mobileNo);

        Task<UserRole?> GetUserRoleByEmail(string email);

        Task<int> UpdatePassword(AppUser user);

        Task<AppUser> GetUserById(Guid id);

        Task<IEnumerable<AppUser>> GetAllUsersByStatus(bool isActive);

        Task<IEnumerable<AppUser>> GetAllUsersByUserRoleAndStatus(UserRole userRole, bool isActive);

        Task<IEnumerable<AppUser>> GetUsersByUserRole(UserRole userRole);

        Task<IEnumerable<AppUser>> GetAllUsers(string emailToExclude);

        Task<IEnumerable<AppUser>> GetAllUsers();

        Task<int> GetAllUserCount();

        Task<List<AppUser>> GetAllUsers(int rows, int pageNumber);

        Task DeleteUserById(Guid userId);

        Task<bool> UpdateLoginProviderById(LoginProvider loginProvider, Guid userId);

        IEnumerable<AppUser> GetUsersByUserRoles(int[] userRoles);

        Task<IEnumerable<AppUser>> GetAllUsersCreatedByAUser(Guid creatorId);

        Task<List<AppUser>> GetAllUsersByKey(string key, string value);

        Task<IEnumerable<AppUser>> GetAllUsersByUserIds(Guid[] userIds);

        Task<int> IsPhoneNumberAndEmailAvailable(string email, string phoneNo);
    }
}
