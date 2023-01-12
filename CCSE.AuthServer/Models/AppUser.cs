using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthServer.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        [NotMapped]
        public string Password { get; set; }

        public bool IsActive { get; set; }
        public LoginProvider LoginProvider { get; set; }
        public UserRole UserRoleEnum { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? IsVerified { get; set; }

        [NotMapped]
        private DateTime _DateOfBirth;
        public DateTime DateOfBirth
        {
            get
            {
                return _DateOfBirth.ToUniversalTime();
            }
            set
            {
                _DateOfBirth = value;
                _DateOfBirth = DateTime.SpecifyKind(_DateOfBirth, DateTimeKind.Utc);
            }
        }

        public string CreatedBy { get; set; }

        [NotMapped]
        private DateTime _CreatedDate;
        public DateTime Created
        {
            get
            {
                return _CreatedDate.ToUniversalTime();
            }
            set
            {
                _CreatedDate = value;
                _CreatedDate = DateTime.SpecifyKind(_CreatedDate, DateTimeKind.Utc);
            }
        }
        public string ModifiedBy { get; set; }
        [NotMapped]
        private DateTime? _ModifiedDate;
        public DateTime? Modified
        {
            get
            {
                if (_ModifiedDate.HasValue)
                {
                    return _ModifiedDate.GetValueOrDefault().ToUniversalTime();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    _ModifiedDate = value;
                    _ModifiedDate = DateTime.SpecifyKind(_ModifiedDate.GetValueOrDefault(), DateTimeKind.Utc);
                }
                else
                {
                    _ModifiedDate = null;
                }
            }
        }

        [NotMapped]
        public string LoginProviderValue
        {
            get
            {
                if (LoginProvider == LoginProvider.Email)
                {
                    return "Email";
                }
                else if (LoginProvider == LoginProvider.Facebook)
                {
                    return "Facebook";
                }
                else if (LoginProvider == LoginProvider.Gmail)
                {
                    return "Google";
                }
                else if (LoginProvider == LoginProvider.LinkedIn)
                {
                    return "LinkedIn";
                }
                else if (LoginProvider == LoginProvider.Mobile)
                {
                    return "Mobile";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        [NotMapped]
        public string UserRoleValue
        {
            get
            {
                if (UserRoleEnum == UserRole.Admin)
                {
                    return "Admin";
                }
                else if (UserRoleEnum == UserRole.PowerUser)
                {
                    return "PowerUser";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public virtual ICollection<AppUserClaim> Claims { get; set; }
        public virtual ICollection<AppUserLogin> Logins { get; set; }
        public virtual ICollection<AppUserToken> Tokens { get; set; }
        public virtual ICollection<AppUserRole> UserRoles { get; set; }

    }
    /// <summary>
    /// Login provider values
    /// </summary>
    public enum LoginProvider
    {
        Gmail = 0,
        Facebook = 1,
        Email = 2,
        Apple = 3,
        LinkedIn = 4,
        Mobile = 5
    }

    /// <summary>
    /// User roles
    /// </summary>
    public enum UserRole
    {
        Admin = 0,
        PowerUser = 1,
    }

    /// <summary>
    /// Change password view model
    /// </summary>
    public class ChangePasswordViewModel
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }

    }

    public enum ResetUserValidataions
    {
        AlreadyActivated = 0,
        ActivatedUser = 1
    }

    public class ForgotPwdViewModel
    {
        public string UserName { get; set; }
        public string RecaptchaToken { get; set; }
    }

    public class ChangePasswordViaForgotModel
    {
        public string password { get; set; }
    }
}
