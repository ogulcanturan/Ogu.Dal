using MongoDb.Sample.Api.Domain.Entities;
using MongoDb.Sample.Api.Domain.Repositories.Interfaces;
using MongoDb.Sample.Api.Models.Dtos;
using MongoDb.Sample.Api.Models.Requests.Category;
using MongoDb.Sample.Api.Services.Interfaces;
using Ogu.Dal.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDb.Sample.Api.Services
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

            var entity = await _categoryRepository.GetAsync(c => c.Id == request.Id, cancellationToken: cancellationToken);

            return entity?.ToDto();
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync(bool includeProducts, CancellationToken cancellationToken = default)
        {
            var includeProperties = includeProducts ? nameof(Category.Products) : null;

            var entities= await _categoryRepository.GetAllAsAsyncEnumerable(cancellationToken: cancellationToken);

            return entities.ToDto();
        }

        public async Task<IPaginated<CategoryDto>> GetAllAsAsyncPaginated(GetAllAsPaginatedCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var entities = await _categoryRepository.GetAllAsAsyncPaginated(pageIndex: request.PageIndex, itemsPerPage: request.ItemsPerPage, cancellationToken: cancellationToken);
            
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
            var entity = await _categoryRepository.InstantUpdateAsync(request.Id, c => new Category() { Name = request.Body.Name}, cancellationToken);
            
            return entity?.ToDto();
        }

        public async Task<IEnumerable<CategoryDto>> UpdateRangeAsync(UpdateCategoriesRequest request, CancellationToken cancellationToken = default)
        {
            var ids = request.Body.Ids.Select(i => i.ToString()).ToHashSet();

            _= await _categoryRepository.InstantUpdateRangeAsync(c => ids.Contains(c.Id), new { Name = request.Body.Name }, cancellationToken: cancellationToken);

            var entities = await _categoryRepository.GetAllAsAsyncEnumerable(c => ids.Contains(c.Id), cancellationToken: cancellationToken);

            return entities.ToDto();
        }

        public Task<bool> RemoveAsync(RemoveRequest request, CancellationToken cancellationToken = default)
        {
            return  _categoryRepository.InstantRemoveAsync(request.Id, cancellationToken);
        }

        public Task<long> RemoveRangeAsync(CancellationToken cancellationToken = default)
        {
            return _categoryRepository.InstantRemoveRangeAsync(cancellationToken);
        }

        public Task<bool> IsCategoryExistAsync(string name, CancellationToken cancellationToken)
        {
            return _categoryRepository.IsAnyAsync(c => c.Name == name, cancellationToken);
        }
    }
}