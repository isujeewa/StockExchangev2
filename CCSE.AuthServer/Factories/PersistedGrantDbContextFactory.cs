using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;

namespace AuthServer
{
    public class PersistedGrantDbContextFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
    {
        public PersistedGrantDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddJsonFile($"appsettings.{Environments.Development}.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
            var operationOptions = new OperationalStoreOptions();

            var migrationsAssembly = typeof(PersistedGrantDbContextFactory).GetTypeInfo().Assembly.GetName().Name;

            string applicationDbContextConnectionString =
                config.GetSection("ConnectionStrings").GetValue<string>("ApplicationDbContext");

            optionsBuilder.UseNpgsql(applicationDbContextConnectionString,
                npgsqlOptionsAction: o => o.MigrationsAssembly(migrationsAssembly));

            return new PersistedGrantDbContext(optionsBuilder.Options, operationOptions);
        }
    }
}
