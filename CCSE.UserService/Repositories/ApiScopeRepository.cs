using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using UserService.API.IRepositories;

namespace UserService.API.Repositories
{
    public class ApiScopeRepository : IApiScopeRepository
    {
        private readonly ConfigurationDbContext applicationDbContext;

        public ApiScopeRepository(ConfigurationDbContext _applicationDbContext)
        {
            applicationDbContext = _applicationDbContext;
        }

        public async Task<int> AddApiScope(ApiScope apiScope)
        {
            var repository = applicationDbContext.ApiScopes;
            await repository.AddAsync(apiScope);
            return await applicationDbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteApiScope(Guid id)
        {
            var repository = applicationDbContext.ApiScopes;
            var apiResource = await repository.FindAsync(id);
            repository.Remove(apiResource);
            return await applicationDbContext.SaveChangesAsync();
        }

        public async Task<ApiScope> GetApiScope(Guid apiScopeId)
        {
            var repository = applicationDbContext.ApiScopes;
            var apiResource = await repository.FindAsync(apiScopeId);
            return apiResource;
        }

        public List<ApiScope> GetApiScopes()
        {
            return applicationDbContext.ApiScopes.ToList();
        }

        public async Task<int> UpdateApiScope(ApiScope apiScope)
        {
            var repository = applicationDbContext.ApiScopes;
            repository.Update(apiScope);
            return await applicationDbContext.SaveChangesAsync();
        }
    }
}
