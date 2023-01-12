using UserService.API.IRepositories;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.API.Repositories
{
    /// <summary>
    /// Api resources related function
    /// </summary>
    public class ApiResourceRepository : IApiResourceRepository
    {
        private readonly ConfigurationDbContext applicationDbContext;

        public ApiResourceRepository(ConfigurationDbContext _applicationDbContext)
        {
            applicationDbContext = _applicationDbContext;
        }

        public async Task<int> AddApiResource(ApiResource apiResource)
        {
            var repository = applicationDbContext.ApiResources;
            await repository.AddAsync(apiResource);
            return await applicationDbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteApiResource(Guid apiResourceId)
        {
            var repository = applicationDbContext.ApiResources;
            var apiResource = await repository.FindAsync(apiResourceId);
            repository.Remove(apiResource);
            return await applicationDbContext.SaveChangesAsync();
        }

        public async Task<ApiResource> GetApiResourceAsync(Guid apiResourceId)
        {
            var repository = applicationDbContext.ApiResources;
            var apiResource = await repository.FindAsync(apiResourceId);
            return apiResource;
        }

        public List<ApiResource> GetApiResources()
        {
            return applicationDbContext.ApiResources.ToList();
        }

        public async Task<int> UpdateApiResourceAsync(ApiResource apiResource)
        {
            var repository = applicationDbContext.ApiResources;
            repository.Update(apiResource);
            return await applicationDbContext.SaveChangesAsync();
        }
    }
}
