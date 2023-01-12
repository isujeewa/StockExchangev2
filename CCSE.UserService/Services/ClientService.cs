using IdentityServer4.EntityFramework.Entities;
using UserService.API.IRepositories;
using UserService.API.IServices;

namespace UserService.API.Services
{
    public class ClientService : IClientService
    {
        private IClientRepository _repository;

        public ClientService(IClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> AddClient(Client client)
        {
            return await _repository.AddClient(client);
        }

        public async Task<int> DeleteClient(Guid clientId)
        {
            return await _repository.DeleteClient(clientId);
        }

        public async Task<Client> GetClient(Guid clientId)
        {
            return await _repository.GetClient(clientId);
        }

        public List<Client> GetClients()
        {
            return _repository.GetClients();
        }

        public async Task<int> UpdateClient(Client client)
        {
            return await _repository.UpdateClient(client);
        }
    }
}
