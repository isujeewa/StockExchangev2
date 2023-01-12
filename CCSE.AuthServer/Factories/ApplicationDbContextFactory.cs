using AuthServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;

namespace AuthServer
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddJsonFile($"appsettings.{Environments.Development}.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();

            string applicationDbContextConnectionString =
          config.GetSection("ConnectionStrings").GetValue<string>("ApplicationDbContext");

            var migrationsAssembly = typeof(ApplicationDbContextFactory).GetTypeInfo().Assembly.GetName().Name;

            optionsBuilder.UseNpgsql(applicationDbContextConnectionString,
                npgsqlOptionsAction: o => o.MigrationsAssembly(migrationsAssembly));

            return new UserDbContext(optionsBuilder.Options);
        }
    }
}
