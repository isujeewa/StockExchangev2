using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using UserService.API.IRepositories;

namespace UserService.API.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ConfigurationDbContext applicationDbContext;


        public ClientRepository(ConfigurationDbContext _applicationDbContext)
        {
            applicationDbContext = _applicationDbContext;
        }

        public async Task<int> AddClient(Client client)
        {
            var repository = applicationDbContext.Clients;
            await repository.AddAsync(client);
            return await applicationDbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteClient(Guid clientId)
        {
            var repository = applicationDbContext.Clients;
            var apiResource = await repository.FindAsync(clientId);
            repository.Remove(apiResource);
            return await applicationDbContext.SaveChangesAsync();
        }

        public async Task<Client> GetClient(Guid clientId)
        {
            var repository = applicationDbContext.Clients;
            var apiResource = await repository.FindAsync(clientId);
            return apiResource;
        }

        public List<Client> GetClients()
        {
            return applicationDbContext.Clients.ToList();
        }

        public async Task<int> UpdateClient(Client client)
        {
            var repository = applicationDbContext.Clients;
            repository.Update(client);
            return await applicationDbContext.SaveChangesAsync();
        }
    }
}
