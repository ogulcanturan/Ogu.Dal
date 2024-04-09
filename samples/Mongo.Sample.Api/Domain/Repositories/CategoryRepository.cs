using MongoDb.Sample.Api.Domain.Entities;
using MongoDb.Sample.Api.Domain.Repositories.Interfaces;
using MongoDb.Sample.Api.Settings;
using MongoDB.Driver;
using Ogu.Dal.MongoDb.Repositories;

namespace MongoDb.Sample.Api.Domain.Repositories
{
    internal sealed class CategoryRepository : Repository<Category, string>, ICategoryRepository
    {
        public CategoryRepository(IMongoClient client, DbSettings settings) : base(client, settings.Database) { }
    }
}