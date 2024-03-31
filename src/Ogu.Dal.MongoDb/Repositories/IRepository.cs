using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Ogu.Dal.Abstractions;
using Ogu.Dal.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ogu.Dal.MongoDb.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        IMongoCollection<TEntity> Table { get; }
        IMongoQueryable<TEntity> AsQueryable();
        Task<TEntity> InstantAddAsync(TEntity obj, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> InstantAddRangeAsync(IEnumerable<TEntity> objects, CancellationToken cancellationToken = default);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, TEntity>> selectColumn = null, CancellationToken cancellationToken = default);
        Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        IMongoQueryable<TEntity> GetAllAsQueryable(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, TEntity>> selectColumn = null, Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>> orderBy = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllAsAsyncEnumerable(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, TEntity>> selectColumn = null, Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>> orderBy = null, CancellationToken cancellationToken = default);
        Task<IList<TEntity>> GetAllAsAsyncList(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, TEntity>> selectColumn = null, Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>> orderBy = null, CancellationToken cancellationToken = default);
        Task<IPaginated<TEntity>> GetAllAsAsyncPaginated(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, TEntity>> selectColumn = null, Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>> orderBy = null, int pageIndex = 0, int itemsPerPage = 0, int rangeOfPages = 0, CancellationToken cancellationToken = default);
        Task<ILongPaginated<TEntity>> GetAllAsAsyncLongPaginated(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, TEntity>> selectColumn = null, Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>> orderBy = null, long pageIndex = 0, long itemsPerPage = 0, long rangeOfPages = 0, CancellationToken cancellationToken = default);
        Task<TEntity> InstantReplaceAsync(Expression<Func<TEntity, bool>> predicate, TEntity obj, CancellationToken cancellationToken = default);
        Task<TEntity> InstantUpdateAsync(Expression<Func<TEntity, bool>> predicate, object anonymousType, CancellationToken cancellationToken = default);
        Task<TEntity> InstantUpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity obj, CancellationToken cancellationToken = default);
        Task<long> InstantUpdateRangeAsync(Expression<Func<TEntity, bool>> predicate, object anonymousType, CancellationToken cancellationToken = default);
        Task<bool> InstantRemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> InstantRemoveAsync(Guid id, CancellationToken cancellationToken = default);
        Task<long> InstantRemoveRangeAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<long> InstantRemoveAllAsync(CancellationToken cancellationToken = default);
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task WithSessionAsync(Func<IMongoCollection<TEntity>, Task> action, CancellationToken cancellationToken = default);
    }
}