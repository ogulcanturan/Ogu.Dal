using Ogu.Dal.Sql.Repositories;
using Sql.Sample.Api.Domain.Entities;

namespace Sql.Sample.Api.Domain.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category, int> { }
}