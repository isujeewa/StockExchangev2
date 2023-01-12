using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserService.API.IServices
{
    public interface IClientService
    {
        Task<int> AddClient(Client client);

        Task<int> UpdateClient(Client client);

        Task<int> DeleteClient(Guid clientId);

        Task<Client> GetClient(Guid clientId);

        List<Client> GetClients();
    }
}
