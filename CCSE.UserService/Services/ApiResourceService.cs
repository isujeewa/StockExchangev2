using IdentityServer4.EntityFramework.Entities;
using UserService.API.IRepositories;
using UserService.API.IServices;

namespace UserService.API.Services
{
    public class ApiResourceService : IApiResourceService
    {
        private IApiResourceRepository _repository;

        public ApiResourceService(IApiResourceRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> AddApiResource(ApiResource apiResource)
        {
            return await _repository.AddApiResource(apiResource);
        }

        public async Task<int> DeleteApiResource(Guid apiResourceId)
        {
            return await _repository.DeleteApiResource(apiResourceId);
        }

        public List<ApiResource> GetAll()
        {
            return _repository.GetApiResources();
        }

        public async Task<ApiResource> GetApiResourceAsync(Guid apiResourceId)
        {
            return await _repository.GetApiResourceAsync(apiResourceId);
        }

        public async Task<int> UpdateApiResourceAsync(ApiResource apiResource)
        {
            return await _repository.UpdateApiResourceAsync(apiResource);
        }
    }
}
