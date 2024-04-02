using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDb.Sample.Api.Models.Dtos;
using MongoDb.Sample.Api.Models.Requests.Category;
using Ogu.Dal.Abstractions;

namespace MongoDb.Sample.Api.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> GetAsync(GetCategoryRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryDto>> GetAllAsync(GetAllCategoriesRequest request, CancellationToken cancellationToken = default);
        Task<IPaginated<CategoryDto>> GetAllAsAsyncPaginated(GetAllAsPaginatedCategoryRequest request, CancellationToken cancellationToken = default);
        Task<CategoryDto> AddAsync(AddCategoryRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryDto>> AddRangeAsync(AddCategoriesRequest request, CancellationToken cancellationToken = default);
        Task<CategoryDto> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryDto>> UpdateRangeAsync(UpdateCategoriesRequest request, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(RemoveRequest request, CancellationToken cancellationToken = default);
        Task<long> RemoveRangeAsync(CancellationToken cancellationToken = default);
        Task<bool> IsCategoryExistAsync(string name, CancellationToken cancellationToken);
    }
}