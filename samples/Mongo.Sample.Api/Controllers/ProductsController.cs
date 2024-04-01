using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDb.Sample.Api.Services.Interfaces;

namespace MongoDb.Sample.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            return Task.FromResult<IActionResult>(Ok());
        }
    }
}