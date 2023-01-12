using AuthServer.Models;
using Newtonsoft.Json;

namespace AuthServer.ViewModels
{
    public class AppUserViewModel
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public LoginProvider LoginProvider { get; set; }
        public UserRole UserRoleEnum { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public bool EmailConfirmed { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

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

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

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

        public string[]? CampaignGroupId { get; set; }
    }

    public class TokenViewModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("user_id")]
        public Guid UserId { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

    }

    /// <summary>
    /// Used for Communication service
    /// </summary>
    public class AppUserViewModelComms
    {
        public Guid Id { get; set; }
        public UserRole UserRoleEnum { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class AppUserByRoleViewModel
    {
        public Guid Id { get; set; }
    }

    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
