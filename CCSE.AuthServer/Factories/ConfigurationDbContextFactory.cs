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
    public class ConfigurationDbContextFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
    {
        public ConfigurationDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddJsonFile($"appsettings.{Environments.Development}.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ConfigurationDbContext>();
            var storeOptions = new ConfigurationStoreOptions();

            string applicationDbContextConnectionString =
                config.GetSection("ConnectionStrings").GetValue<string>("ApplicationDbContext");

            var migrationsAssembly = typeof(ConfigurationDbContextFactory).GetTypeInfo().Assembly.GetName().Name;

            optionsBuilder.UseNpgsql(applicationDbContextConnectionString,
                npgsqlOptionsAction: o => o.MigrationsAssembly(migrationsAssembly));

            return new ConfigurationDbContext(optionsBuilder.Options, storeOptions);
        }
    }
}
