using Sql.Sample.Api.Domain.Repositories.Interfaces;

namespace Sql.Sample.Api.Domain.UnitOfWork
{
    public interface IUnitOfWork : Ogu.Dal.Sql.UnitOfWork.IUnitOfWork
    {
        public ICategoryRepository CategoryRepository { get; }
        public IProductRepository ProductRepository { get; }
    }
}