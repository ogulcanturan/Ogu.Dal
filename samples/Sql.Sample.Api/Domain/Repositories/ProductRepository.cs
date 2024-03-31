using Microsoft.EntityFrameworkCore;
using Ogu.Dal.Sql.Repositories;
using Sql.Sample.Api.Domain.Entities;
using Sql.Sample.Api.Domain.Repositories.Interfaces;

namespace Sql.Sample.Api.Domain.Repositories
{
    internal sealed class ProductRepository : Repository<Product, int>, IProductRepository
    {
        public ProductRepository(Context context) : base(context) { }
    }
}