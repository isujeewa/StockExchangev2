using Microsoft.AspNetCore.Identity;
using System;

namespace AuthServer.Models
{
    public class AppRoleClaim : IdentityRoleClaim<Guid>
    {
        public virtual AppRole Role { get; set; }
    }
}
