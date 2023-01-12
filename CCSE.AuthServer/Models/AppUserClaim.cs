using Microsoft.AspNetCore.Identity;
using System;

namespace AuthServer.Models
{
    public class AppUserClaim : IdentityUserClaim<Guid>
    {
        public virtual AppUser User { get; set; }
    }
}
