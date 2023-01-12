using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AuthServer.Models
{
    public class AppRole : IdentityRole<Guid>
    {
        public virtual ICollection<AppUserRole> UserRoles { get; set; }
        public virtual ICollection<AppRoleClaim> RoleClaims { get; set; }
    }
}
