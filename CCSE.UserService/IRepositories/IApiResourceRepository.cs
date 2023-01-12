using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserService.API.IRepositories
{
    public interface IApiResourceRepository
    {
        Task<int> AddApiResource(ApiResource apiResource);

        Task<int> UpdateApiResourceAsync(ApiResource apiResource);

        Task<ApiResource> GetApiResourceAsync(Guid apiResourceId);

        Task<int> DeleteApiResource(Guid apiResourceId);

        List<ApiResource> GetApiResources();

    }
}
