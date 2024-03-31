using Ogu.Dal.Abstractions;
using Sql.Sample.Api.Models.Dtos;
using Sql.Sample.Api.Models.Requests;
using Sql.Sample.Api.Models.Requests.Category;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sql.Sample.Api.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> GetAsync(GetCategoryRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryDto>> GetAllAsync(bool includeProducts, CancellationToken cancellationToken = default);
        Task<IPaginated<CategoryDto>> GetAllAsAsyncPaginated(GetAllAsPaginatedCategoryRequest request, CancellationToken cancellationToken = default);
        Task<CategoryDto> AddAsync(AddCategoryRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryDto>> AddRangeAsync(AddCategoriesRequest request, CancellationToken cancellationToken = default);
        Task<CategoryDto> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryDto>> UpdateRangeAsync(UpdateCategoriesRequest request, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(RemoveRequest request, CancellationToken cancellationToken = default);
        Task<int> RemoveAllAsync(CancellationToken cancellationToken = default);
    }
}