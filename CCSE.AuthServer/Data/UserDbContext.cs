using AuthServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace AuthServer.Data
{
    /// <summary>
    /// User DB Context
    /// </summary>
    public class UserDbContext : IdentityDbContext<
        AppUser, AppRole, Guid,
        AppUserClaim, AppUserRole, AppUserLogin,
        AppRoleClaim, AppUserToken>
    {
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<AppRole> AppRole { get; set; }
        public DbSet<AppUserClaim> AppUserClaim { get; set; }
        public DbSet<AppUserRole> AppUserRole { get; set; }
        public DbSet<AppUserLogin> AppUserLogin { get; set; }
        public DbSet<AppRoleClaim> AppRoleClaim { get; set; }
        public DbSet<AppUserToken> AppUserToken { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppRoleClaim>(builder =>
            {
                builder.HasOne(roleClaim => roleClaim.Role).WithMany(role => role.RoleClaims).HasForeignKey(roleClaim => roleClaim.RoleId);
                builder.ToTable("AppRoleClaim");
            });
            modelBuilder.Entity<AppRole>(builder =>
            {
                builder.ToTable("AppRole");
            });
            modelBuilder.Entity<AppUserClaim>(builder =>
            {
                builder.HasOne(userClaim => userClaim.User).WithMany(user => user.Claims).HasForeignKey(userClaim => userClaim.UserId);
                builder.ToTable("AppUserClaim");
            });
            modelBuilder.Entity<AppUserLogin>(builder =>
            {
                builder.HasOne(userLogin => userLogin.User).WithMany(user => user.Logins).HasForeignKey(userLogin => userLogin.UserId);
                builder.ToTable("AppUserLogin");
            });
            modelBuilder.Entity<AppUser>(builder =>
            {
                builder.ToTable("AppUser");
            });
            modelBuilder.Entity<AppUserRole>(builder =>
            {
                builder.HasOne(userRole => userRole.Role).WithMany(role => role.UserRoles).HasForeignKey(userRole => userRole.RoleId);
                builder.HasOne(userRole => userRole.User).WithMany(user => user.UserRoles).HasForeignKey(userRole => userRole.UserId);
                builder.ToTable("AppUserRole");
            });
            modelBuilder.Entity<AppUserToken>(builder =>
            {
                builder.HasOne(userToken => userToken.User).WithMany(user => user.Tokens).HasForeignKey(userToken => userToken.UserId);
                builder.ToTable("AppUserToken");
            });
        }
    }
}
