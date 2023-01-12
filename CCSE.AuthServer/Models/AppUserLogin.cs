using Microsoft.AspNetCore.Identity;
using System;

namespace AuthServer.Models
{
    public class AppUserLogin : IdentityUserLogin<Guid>
    {
        public virtual AppUser User { get; set; }
    }
}
