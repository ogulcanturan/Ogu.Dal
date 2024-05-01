using Ogu.Dal.Sql.Repositories;
using Sql.Sample.Api.Domain.Entities;
using Sql.Sample.Api.Domain.Repositories.Interfaces;

namespace Sql.Sample.Api.Domain.Repositories
{
    internal sealed class CategoryRepository : Repository<Category, int>, ICategoryRepository
    {
        public CategoryRepository(Context context) : base(context) { }
    }
}