using AuthServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Data
{
    /// <summary>
    /// User DB seed class
    /// </summary>
    public class UserDbContextSeed
    {
        private readonly IPasswordHasher<AppUser> _passwordHasher = new PasswordHasher<AppUser>();

        public async Task SeedAsync(
            UserDbContext context,
            IWebHostEnvironment env,
           UserManager<AppUser> userManager,
            ILogger<UserDbContextSeed> logger,
            int? retry = 0)
        {
            int retryForAvaiability = retry.Value;

            try
            {
                var contentRootPath = env.ContentRootPath;
                var webroot = env.WebRootPath;

                if (!context.AppRole.Any())
                {
                    context.AppRole.AddRange(GetDefaultUserRoles());
                    await context.SaveChangesAsync();
                }

                if (!context.Users.Any())
                {
                    foreach (var item in GetDefaultUser())
                    {
                        await userManager.CreateAsync(item);
                        await context.SaveChangesAsync();

                        var appRole =
                            await context.AppRole.AsNoTracking()
                            .FirstOrDefaultAsync(a => a.Name.ToLower().Equals(item.UserRoleValue.ToLower()));
                        AppUserRole appUserRole = new AppUserRole();
                        appUserRole.RoleId = appRole.Id;
                        appUserRole.UserId = item.Id;
                        context.AppUserRole.Add(appUserRole);
                        await context.SaveChangesAsync();
                    }

                }
            }
            catch (Exception ex)
            {
                if (retryForAvaiability < 10)
                {
                    retryForAvaiability++;
                    logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(UserDbContext));
                    await SeedAsync(context, env, userManager, logger, retryForAvaiability);
                }
            }
        }

        private static IEnumerable<AppRole> GetDefaultUserRoles()
        {
            var list = new List<AppRole>();
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                var name = Enum.GetName(typeof(UserRole), role);
                var item = new AppRole()
                {
                    Name = name,
                    NormalizedName = name.ToUpper()
                };

                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// Set the default users
        /// </summary>
        /// <returns></returns>
        private IEnumerable<AppUser> GetDefaultUser()
        {
            var list = new List<AppUser>();
            var user =
           new AppUser()
           {
               Email = "admin@ccse.com",
               FirstName = "ccse",
               LastName = "Admin",
               Created = DateTime.UtcNow,
               CreatedBy = "CCSE",
               IsActive = true,
               UserRoleEnum = UserRole.Admin,
               UserName = "admin@ccse.com",
               NormalizedEmail = "admin@ccse.com",
               NormalizedUserName = "admin@ccse.com",
               SecurityStamp = Guid.NewGuid().ToString("D"),
               EmailConfirmed = true,
               ModifiedBy = ""
           };
            user.PasswordHash = _passwordHasher.HashPassword(user, "admin@1234");
            list.Add(user);

            user =
         new AppUser()
         {
             Email = "poweruser@ccse.com",
             FirstName = "ccse",
             LastName = "Power User",
             Created = DateTime.UtcNow,
             CreatedBy = "CCSE",
             IsActive = true,
             UserRoleEnum = UserRole.PowerUser,
             UserName = "poweruser@ccse.com",
             NormalizedEmail = "poweruser@ccse.com",
             NormalizedUserName = "poweruser@ccse.com",
             SecurityStamp = Guid.NewGuid().ToString("D"),
             EmailConfirmed = true,
             ModifiedBy = ""
         };
            user.PasswordHash = _passwordHasher.HashPassword(user, "poweruser@1234");
            list.Add(user);

            return list;

        }
    }
}
