using AuthServer.Models;
using UserService.API.IServices;

namespace UserService.API.Services
{
    /// <summary>
    /// User service class implementation
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AppUser> AddUser(AppUser user)
        {
            var item = await _unitOfWork.UserRepository.AddUser(user);
            _unitOfWork.SaveChanges();
            return item;
        }

        public Task DeleteUserById(Guid userId)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<AppUser>> GetAllUsers(string emailToExclude)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppUser>> GetAllUsersByStatus(bool isActive)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppUser>> GetAllUsersByUserRoleAndStatus(UserRole userRole, bool isActive)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AppUser>> GetUsersByUserRole(UserRole userRole)
        {
            return await _unitOfWork.UserRepository.GetUsersByRole(userRole);
        }

        public Task<AppUser> GetByEmailAndPassword(string email, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<AppUser> GetUserByEmail(string email)
        {
            return await _unitOfWork.UserRepository.GetByEmail(email);
        }

        public async Task<AppUser> GetUserById(Guid id)
        {
            return await _unitOfWork.UserRepository.GetById(id);
        }

        public Task<UserRole?> GetUserRoleByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateLoginProviderById(LoginProvider loginProvider, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdatePassword(AppUser user)
        {
            return await _unitOfWork.UserRepository.UpdatePassword(user);
        }

      
        public async Task<IEnumerable<AppUser>> GetAllUsers()
        {
            return await _unitOfWork.UserRepository.GetAll();
        }

        public async Task DeactivateUserByUserId(Guid userId)
        {
            await _unitOfWork.UserRepository.DeactivateUserByUserId(userId);
        }

        public IEnumerable<AppUser> GetUsersByUserRoles(int[] userRoles)
        {
            return _unitOfWork.UserRepository.GetUsersByUserRoles(userRoles);
        }

        public Task<IEnumerable<AppUser>> GetAllUsersCreatedByAUser(Guid creatorId)
        {
            return _unitOfWork.UserRepository.GetAllUsersCreatedByAUser(creatorId);
        }

        public Task<AppUser> GetUserByMobileNo(string mobileNo)
        {
            return _unitOfWork.UserRepository.GetUserByMobileNo(mobileNo);

        }

        public Task<AppUser> GetUserByMobileAndReference(string mobileNo, string reference)
        {
            return _unitOfWork.UserRepository.GetUserByMobileAndReference(mobileNo, reference);
        }

        public async Task<List<AppUser>> GetAllUsers(int rows, int pageNumber)
        {
            return await _unitOfWork.UserRepository.GetAllUsers(rows, pageNumber);
        }
     
        public async Task<int> GetAllUserCount()
        {
            return await _unitOfWork.UserRepository.GetAllUserCount();
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersByUserIds(Guid[] userIds)
        {
            return await _unitOfWork.UserRepository.GetAllUsersByUserIds(userIds);
        }

        public Task<List<AppUser>> GetAllUsersByKey(string key, string value)
        {
            throw new NotImplementedException();
        }

        public Task<int> IsPhoneNumberAndEmailAvailable(string email, string phoneNo)
        {
            throw new NotImplementedException();
        }
    }
}
