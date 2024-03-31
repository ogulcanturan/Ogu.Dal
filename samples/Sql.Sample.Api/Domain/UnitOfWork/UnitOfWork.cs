using Microsoft.EntityFrameworkCore;
using Sql.Sample.Api.Domain.Repositories;
using Sql.Sample.Api.Domain.Repositories.Interfaces;

namespace Sql.Sample.Api.Domain.UnitOfWork
{
    internal sealed class UnitOfWork : Ogu.Dal.Sql.UnitOfWork.UnitOfWork, IUnitOfWork
    {
        public UnitOfWork(Context context) : base(context) { }

        private ICategoryRepository _categoryRepository;
        private IProductRepository _productRepository;

        public ICategoryRepository CategoryRepository => _categoryRepository ??= new CategoryRepository((Context)Context);
        public IProductRepository ProductRepository => _productRepository ??= new ProductRepository((Context)Context);
    }
}