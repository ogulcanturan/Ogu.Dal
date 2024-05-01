using Sql.Sample.Api.Domain.Repositories.Interfaces;
using Sql.Sample.Api.Services.Interfaces;

namespace Sql.Sample.Api.Services
{
    internal sealed class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
    }
}