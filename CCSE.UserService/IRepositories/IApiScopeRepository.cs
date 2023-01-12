using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserService.API.IRepositories
{
    public interface IApiScopeRepository
    {
        List<ApiScope> GetApiScopes();

        Task<ApiScope> GetApiScope(Guid apiScopeId);

        Task<int> AddApiScope(ApiScope apiScope);

        Task<int> UpdateApiScope(ApiScope apiScope);

        Task<int> DeleteApiScope(Guid id);
    }
}
