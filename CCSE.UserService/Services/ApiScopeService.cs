using UserService.API.IRepositories;
using UserService.API.IServices;
using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserService.API.Services
{
    public class ApiScopeService : IApiScopeService
    {
        private IApiScopeRepository _repository;

        public ApiScopeService(IApiScopeRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> AddApiScope(ApiScope apiScope)
        {
            return await _repository.AddApiScope(apiScope);
        }

        public async Task<int> DeleteApiScope(Guid id)
        {
            return await _repository.DeleteApiScope(id);
        }

        public async Task<ApiScope> GetApiScope(Guid apiScopeId)
        {
            return await _repository.GetApiScope(apiScopeId);
        }

        public List<ApiScope> GetApiScopes()
        {
            return _repository.GetApiScopes();
        }

        public async Task<int> UpdateApiScope(ApiScope apiScope)
        {
            return await _repository.UpdateApiScope(apiScope);
        }
    }
}
