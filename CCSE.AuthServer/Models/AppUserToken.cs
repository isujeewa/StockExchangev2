using Microsoft.AspNetCore.Identity;
using System;

namespace AuthServer.Models
{
    public class AppUserToken : IdentityUserToken<Guid>
    {
        public virtual AppUser User { get; set; }
    }

    public class TokenData
    {
        public string AccessToken { get; set; }
        public int AccessTokenExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public int RefreshTokenExpiresIn { get; set; }

        public string FullName { get; set; }
        public Guid UserId { get; set; }
        public string Reference { get; set; }

        public AppUser User { get; set; }


    }

    /// <summary>
    /// Token types
    /// </summary>
    public enum TokenType
    {
        Default = -1,
        ApplicationToken = 0,
        FBToken = 1,
        GoogleToken = 2,
        AppleToken = 3,
        LinkedInToken = 4
    }

    /// <summary>
    /// Refresh token data
    /// </summary>
    public class RefreshTokenObject
    {
        public string RefreshToken { get; set; }
    }
}
