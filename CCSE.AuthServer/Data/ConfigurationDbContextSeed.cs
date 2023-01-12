using AuthServer.Config;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Data
{
    /// <summary>
    /// Config DB seed class
    /// </summary>
    public class ConfigurationDbContextSeed
    {
        public async Task SeedAsync(ConfigurationDbContext context, IConfiguration configuration)
        {

            //callbacks urls from config:
            var clientUrls = configuration.GetSection("ServiceClientUrls").GetChildren().ToDictionary(x => x.Key, x => x.Value);
            //clientUrls.Add("Weather", configuration.GetValue<string>("Weather"));

            if (!context.Clients.Any())
            {
                foreach (var client in Configuration.GetClients(clientUrls))
                {
                    context.Clients.Add(client.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Configuration.GetResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var api in Configuration.GetApiResources())
                {
                    context.ApiResources.Add(api.ToEntity());
                }

                await context.SaveChangesAsync();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var api in Configuration.GetApisScopes())
                {
                    context.ApiScopes.Add(api.ToEntity());
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
