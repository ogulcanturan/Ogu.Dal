using MongoDb.Sample.Api.Domain.Entities;
using Ogu.Dal.MongoDb.Repositories;

namespace MongoDb.Sample.Api.Domain.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product, string> { }
}