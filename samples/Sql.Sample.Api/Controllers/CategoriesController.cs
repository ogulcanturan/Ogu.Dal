using Microsoft.AspNetCore.Mvc;
using Sql.Sample.Api.Models.Requests;
using Sql.Sample.Api.Models.Requests.Category;
using Sql.Sample.Api.Services.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Sql.Sample.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Get category by its Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(GetCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _categoryService.GetAsync(request, cancellationToken);

            if (result == null)
                NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Get categories
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(bool includeProducts, CancellationToken cancellationToken = default)
        {
            var result = await _categoryService.GetAllAsync(includeProducts, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get categories in a paginated format
        /// </summary>
        [HttpGet("paginated")]
        public async Task<IActionResult> GetAllAsPaginated(GetAllAsPaginatedCategoryRequest request,CancellationToken cancellationToken = default)
        {
            var result = await _categoryService.GetAllAsAsyncPaginated(request, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Add category
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]AddCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _categoryService.AddAsync(request, cancellationToken);

            if (result == null)
                BadRequest();

            return Ok(result);
        }

        /// <summary>
        /// Add multiple categories
        /// </summary>
        [HttpPost("range")]
        public async Task<IActionResult> AddRange([FromBody] AddCategoriesRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _categoryService.AddRangeAsync(request, cancellationToken);

            if (result == null)
                BadRequest();

            return Ok(result);
        }

        /// <summary>
        /// Update category by its Id
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _categoryService.UpdateAsync(request, cancellationToken);

            if (result == null)
                BadRequest();

            return Ok(result);
        }

        /// <summary>
        /// Update multiple categories
        /// </summary>
        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange(UpdateCategoriesRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _categoryService.UpdateRangeAsync(request, cancellationToken);

            if (result == null)
                BadRequest();

            return Ok(result);
        }

        /// <summary>
        /// Remove category by its Id
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(RemoveRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _categoryService.RemoveAsync(request, cancellationToken);

            return result ? Ok() : NotFound();
        }

        /// <summary>
        /// Remove all categories
        /// </summary>
        [HttpDelete("range")]
        public async Task<IActionResult> RemoveAll(CancellationToken cancellationToken = default)
        {
            var numberOfRemovedRows = await _categoryService.RemoveAllAsync(cancellationToken);

            return Ok($"Removed '{numberOfRemovedRows}' records from db");
        }
    }
}