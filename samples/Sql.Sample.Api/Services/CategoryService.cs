using Microsoft.EntityFrameworkCore;
using Ogu.Dal.Abstractions;
using Ogu.Dal.Sql.Entities;
using Sql.Sample.Api.Domain.Entities;
using Sql.Sample.Api.Domain.Repositories.Interfaces;
using Sql.Sample.Api.Models.Dtos;
using Sql.Sample.Api.Models.Requests.Category;
using Sql.Sample.Api.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sql.Sample.Api.Services
{
    internal sealed class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryDto> GetAsync(GetCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var includeProperties = request.IncludeProducts ? nameof(Category.Products) : null;

            var entity = await _categoryRepository.GetAsync(TrackingActivityEnum.Inactive, c => c.Id == request.Id, includeProperties, querySplittingBehavior: QuerySplittingBehavior.SplitQuery, cancellationToken: cancellationToken);

            return entity?.ToDto();
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync(GetAllCategoriesRequest request, CancellationToken cancellationToken = default)
        {
            var includeProperties = request.IncludeProducts ? nameof(Category.Products) : null;

            var entities= await _categoryRepository.GetAllAsAsyncEnumerable(TrackingActivityEnum.Inactive, includeProperties: includeProperties, querySplittingBehavior: QuerySplittingBehavior.SplitQuery, cancellationToken: cancellationToken);

            return entities.ToDto();
        }

        public async Task<IPaginated<CategoryDto>> GetAllAsAsyncPaginated(GetAllAsPaginatedCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var includeProperties = request.IncludeProducts ? nameof(Category.Products) : null;

            var entities = await _categoryRepository.GetAllAsAsyncPaginated(TrackingActivityEnum.Inactive, includeProperties: includeProperties, pageIndex: request.PageIndex, itemsPerPage: request.ItemsPerPage, querySplittingBehavior: QuerySplittingBehavior.SplitQuery, cancellationToken: cancellationToken);
            
            return entities.ToPaginatedDto(entities.Items.ToDto());
        }

        public async Task<CategoryDto> AddAsync(AddCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var entity = new Category
            {
                Name = request.Name,
            };

            await _categoryRepository.InstantAddAsync(entity, cancellationToken);

            return entity.ToDto();
        }

        public async Task<IEnumerable<CategoryDto>> AddRangeAsync(AddCategoriesRequest request, CancellationToken cancellationToken = default)
        {
            var entities = await _categoryRepository.InstantAddRangeAsync(request.Categories.Select(c => new Category { Name = c }), cancellationToken);

            return entities.ToDto();
        }

        public async Task<CategoryDto> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default)
        {
            Category entity;

            if (request.FetchFromDb)
            {
                // First fetch db then update (Two calls - Slower)

                entity = await _categoryRepository.GetByIdAsync(TrackingActivityEnum.Active, request.Id, cancellationToken);

                if (entity == null)
                    return null;

                entity.Name = request.Body.Name;

                await _categoryRepository.SaveChangesWithDateAsync(cancellationToken);
            }
            else
            {
                // To Update entity without fetching db (One call - Faster) - will return default value if it couldn't find!

                entity = await _categoryRepository.InstantUpdateAsync(TrackingActivityEnum.Inactive, request.Id, c => c.Name = request.Body.Name, cancellationToken);
            }
            
            return entity?.ToDto();
        }

        public async Task<IEnumerable<CategoryDto>> UpdateRangeAsync(UpdateCategoriesRequest request, CancellationToken cancellationToken = default)
        {
            IEnumerable<Category> entities;

            if (request.FetchFromDb)
            {
                // First fetch db then update (Two calls - Slower)
                entities = await _categoryRepository.GetAllAsAsyncArray(TrackingActivityEnum.Active, c => request.Body.Ids.Contains(c.Id), cancellationToken: cancellationToken);

                await _categoryRepository.InstantUpdateRangeAsync(TrackingActivityEnum.Active, entities,
                    c => c.Name = request.Body.Name, cancellationToken: cancellationToken);

                await _categoryRepository.SaveChangesWithDateAsync(cancellationToken);
            }
            else
            {
                // To Update entity without fetching db (One call - Faster) - will return default value if it couldn't find!
                entities = await _categoryRepository.InstantUpdateRangeAsync(TrackingActivityEnum.Inactive, request.Body.Ids, c => c.Name = request.Body.Name, cancellationToken: cancellationToken);
            }

            return entities.ToDto();
        }

        public async Task<bool> RemoveAsync(RemoveRequest request, CancellationToken cancellationToken = default)
        {
            if (request.FirstFetchFromDb)
            {
                // First fetch db then update (Two calls - Slower)

                var entity = await _categoryRepository.GetByIdAsync(TrackingActivityEnum.Active, request.Id, cancellationToken);

                if (entity == null)
                    return false;

                await _categoryRepository.InstantRemoveAsync(entity, cancellationToken);
            }
            else
            {
                // To Update entity without fetching db (One call - Faster) - will return default value if it couldn't find!
                await _categoryRepository.InstantRemoveAsync(request.Id, cancellationToken);
            }

            return true;
        }

        public Task<int> RemoveRangeAsync(CancellationToken cancellationToken = default)
        {
            return _categoryRepository.InstantRemoveRangeAsync(cancellationToken);
        }

        public Task<bool> IsCategoryExistAsync(string name, CancellationToken cancellationToken)
        {
            return _categoryRepository.IsAnyAsync(c => c.Name == name, cancellationToken: cancellationToken);
        }
    }
}