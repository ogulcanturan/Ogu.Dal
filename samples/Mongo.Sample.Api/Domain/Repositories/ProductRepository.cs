using MongoDb.Sample.Api.Domain.Entities;
using MongoDb.Sample.Api.Domain.Repositories.Interfaces;
using MongoDb.Sample.Api.Models.Settings;
using MongoDB.Driver;
using Ogu.Dal.MongoDb.Repositories;

namespace MongoDb.Sample.Api.Domain.Repositories
{
    internal sealed class ProductRepository : Repository<Product, string>, IProductRepository
    {
        public ProductRepository(IMongoClient client, DbSettings settings) : base(client, settings.Database) { }
    }
}