using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserService.API.IServices
{
    public interface IApiResourceService
    {
        Task<int> AddApiResource(ApiResource apiResource);

        Task<int> UpdateApiResourceAsync(ApiResource apiResource);

        Task<ApiResource> GetApiResourceAsync(Guid apiResourceId);

        Task<int> DeleteApiResource(Guid apiResourceId);

        List<ApiResource> GetAll();

    }
}
