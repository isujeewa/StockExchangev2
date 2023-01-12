using AuthServer.Data;
using AuthServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using UserService.API.IRepositories;
using UserService.API.IServices;

namespace UserService.API.Repositories
{
    /// <summary>
    /// User repository class
    /// </summary>
    public class UserRepository : GenericRepository<AppUser>, IUserRepository
    {
        private readonly UserDbContext applicationDbContext;

        private readonly IUnitOfWork unitOfWork;

        public UserRepository(UserDbContext _applicationDbContext, IUnitOfWork _unitOfWork) : base(_applicationDbContext)
        {
            applicationDbContext = _applicationDbContext;
            unitOfWork = _unitOfWork;
        }

        public async Task<AppUser> AddUser(AppUser user)
        {
            //user.Created = DateTime.UtcNow;
            //user.CreatedBy = "SYSTEM";
            return await unitOfWork.GetRepository<AppUser>().Add(user);
        }

        public async Task<AppUser> GetByEmail(string email)
        {
            var repository = unitOfWork.GetRepository<AppUser>();
            Expression<Func<AppUser, bool>> FilterByKey(string key)
            {
                return x => x.Email.ToLower() == key.ToLower();
            }
            var item = await repository.Get(FilterByKey(email));
            return item.FirstOrDefault();
        }

        public async Task<AppUser> GetByEmailAndPassword(string email, string password)
        {
            var repository = unitOfWork.GetRepository<AppUser>();
            Expression<Func<AppUser, bool>> GetByEmailAndPassword(string email, string password)
            {
                return x => x.Email.ToLower() == email.ToLower() && x.Password == password;
            }
            var items = await repository.Get(GetByEmailAndPassword(email, password));
            return items.FirstOrDefault();
        }

        public async Task<UserRole?> GetUserRoleByEmail(string email)
        {
            var repository = unitOfWork.GetRepository<AppUser>();
            Expression<Func<AppUser, bool>> GetByEmail(string email)
            {
                return x => x.Email.ToLower() == email.ToLower();
            }
            var items = await repository.Get(GetByEmail(email));

            if (items.Any())
            {
                return items.FirstOrDefault().UserRoleEnum;
            }
            else
            {
                return null;
            }
        }

        public async Task<int> UpdateLoginProvider(LoginProvider loginProvider, Guid userId)
        {
            int updatedCode = 0;
            var repository = unitOfWork.GetRepository<AppUser>();
            var item = await repository.GetById(userId);
            item.LoginProvider = loginProvider;
            item.Modified = DateTime.UtcNow;
            item.ModifiedBy = userId.ToString();
            repository.Update(item);
            updatedCode = await unitOfWork.Commit();
            return updatedCode;
        }

        public async Task<int> UpdatePassword(AppUser user)
        {
            int updatedCode = 0;
            var repository = unitOfWork.GetRepository<AppUser>();

            // set the modified field values
            user.Modified = DateTime.UtcNow;
            user.ModifiedBy = user.Id.ToString();
            user.IsActive = true;
            repository.Update(user);
            updatedCode = await unitOfWork.Commit();
            return updatedCode;
        }

      

     

        public async Task<AppUser> GetUserById(Guid id)
        {
            var repository = unitOfWork.GetRepository<AppUser>();

            var item = await repository.GetById(id);
            return item;
        }

        public async Task<int> ValidateUserActivation(Guid userId)
        {
            var repository = unitOfWork.GetRepository<AppUser>();
            var item = await repository.GetById(userId);

            // check whether the item has been already modified
            // if not activate the user
            if (string.IsNullOrEmpty(item.ModifiedBy))
            {
                await UpdateActivation(userId);
                return (int)ResetUserValidataions.ActivatedUser;
            }
            else
            {
                return (int)ResetUserValidataions.AlreadyActivated;
            }
        }

        public async Task UpdateActivation(Guid userId)
        {
            var repository = unitOfWork.GetRepository<AppUser>();
            var item = await GetUserById(userId);

            item.IsActive = true;
            item.Modified = DateTime.UtcNow;
            item.ModifiedBy = item.Id.ToString();
            repository.Update(item);
            await unitOfWork.Commit();
        }

        public async Task<IEnumerable<AppUser>> GetUsersByRole(UserRole role)
        {
            var repository = unitOfWork.GetRepository<AppUser>();
            //get all users
            Expression<Func<AppUser, AppUser>> GetAllUser()
            {
                return user => user;
            }
            // where by type
            Expression<Func<AppUser, bool>> GetByRole(UserRole role)
            {
                return user => user.UserRoleEnum == role;
            }
            var userList = repository.GetAllOrDefault(GetAllUser(), GetByRole(role));
            return userList;
        }

        public async Task UpdateUserDetailsByAdmin(AppUser user)
        {
            var repository = unitOfWork.GetRepository<AppUser>();
            var existingUser = await repository.GetById(user.Id);
            existingUser.Modified = DateTime.UtcNow;
            existingUser.ModifiedBy = existingUser.Id.ToString();
            existingUser.UserRoleEnum = user.UserRoleEnum;
            existingUser.IsActive = user.IsActive;
            repository.Update(existingUser);
            await unitOfWork.Commit();
        }

        public async Task DeactivateUserByUserId(Guid userId)
        {
            var repository = unitOfWork.GetRepository<AppUser>();
            var existingUser = await repository.GetById(userId);
            existingUser.IsActive = false;
            repository.Update(existingUser);
            await unitOfWork.Commit();
        }

        public IEnumerable<AppUser> GetUsersByUserRoles(int[] userRoles)
        {
            var items = applicationDbContext.AppUser.AsNoTracking().Where(a => userRoles.Contains((int)a.UserRoleEnum));
            items.ToList().ForEach(a => a.Password = null);
            return items;
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersCreatedByAUser(Guid creatorId)
        {
            var repository = unitOfWork.GetRepository<AppUser>();
            Expression<Func<AppUser, bool>> GetByCreatorId(Guid creatorId)
            {
                return x => x.CreatedBy == creatorId.ToString();
            }
            var items = await repository.Get(GetByCreatorId(creatorId));
            return items;
        }

        public async Task<AppUser> GetUserByMobileNo(string mobileNo)
        {
            var repository = unitOfWork.GetRepository<AppUser>();
            Expression<Func<AppUser, bool>> FilterByKey(string key)
            {
                return x => x.PhoneNumber.ToLower() == key.ToLower();
            }
            var item = await repository.Get(FilterByKey(mobileNo));
            return item.FirstOrDefault();
        }

        /// <summary>
        /// Gets the user count
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetAllUserCount()
        {
            return await applicationDbContext.AppUser.Select(a => a.Id).CountAsync();
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersByUserIds(Guid[] userIds)
        {
            var repository = unitOfWork.GetRepository<AppUser>();
            List<AppUser> users = new List<AppUser>();

            foreach (var id in userIds)
            {
                var user = await GetById(id);
                users.Add(user);
            }
            return users;
        }

        public Task<AppUser> GetUserByMobileAndReference(string mobileNo, string reference)
        {
            throw new NotImplementedException();
        }

        public Task<List<AppUser>> GetAllUsers(int rows, int pageNumber)
        {
            throw new NotImplementedException();
        }
    }
}
