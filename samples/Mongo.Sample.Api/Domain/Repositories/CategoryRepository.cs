using MongoDb.Sample.Api.Domain.Entities;
using MongoDb.Sample.Api.Domain.Repositories.Interfaces;
using MongoDB.Driver;
using Ogu.Dal.MongoDb.Repositories;
using MongoDb.Sample.Api.Settings;

namespace MongoDb.Sample.Api.Domain.Repositories
{
    internal sealed class CategoryRepository : Repository<Category, string>, ICategoryRepository
    {
        public CategoryRepository(IMongoClient client, DbSettings settings) : base(client, settings.Database) { }
    }
}