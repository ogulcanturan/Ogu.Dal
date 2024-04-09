using Microsoft.EntityFrameworkCore;
using Ogu.Dal.Abstractions;
using Ogu.Dal.Sql.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ogu.Dal.Sql.Repositories
{
    public interface IRepository<TEntity, in TId> : IAsyncDisposable, IDisposable where TEntity : class, IBaseEntity<TId>, new()
    {
        DbSet<TEntity> Table { get; }
        IQueryable<TEntity> AsQueryable();
        void Add(TEntity entity);
        Task<bool> InstantAddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> InstantAddAsync(TEntity entity, bool ignoreIfNotExists, CancellationToken cancellationToken = default);
        Task AddAndFetchSpecialValueGeneratorsBeforeSavingAsync(TEntity entity, CancellationToken cancellationToken = default);
        IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities);
        Task<IEnumerable<TEntity>> InstantAddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> InstantAddRangeAsync(IEnumerable<TEntity> entities, bool ignoreIfNotExists, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> AddRangeAndFetchSpecialValueGeneratorsBeforeSavingAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task<TEntity> GetAsync(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null,  string includeProperties = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null, QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default);
        Task<TEntity> GetByIdAsync(TrackingActivityEnum trackingActivity, TId id, CancellationToken cancellationToken = default);
        IQueryable<TEntity> GetAllAsQueryable(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null, QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery);
        Task<IEnumerable<TEntity>> GetAllAsAsyncEnumerable(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null, QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default);
        Task<IList<TEntity>> GetAllAsAsyncList(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null, QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default);
        Task<TEntity[]> GetAllAsAsyncArray(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null, QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default);
        Task<IPaginated<TEntity>> GetAllAsAsyncPaginated(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null, Expression<Func<TEntity, TEntity>> selectColumn = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int pageIndex = 0, int itemsPerPage = 0, int rangeOfPages = 0, QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default);
        Task<ILongPaginated<TEntity>> GetAllAsAsyncLongPaginatedModel(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null, Expression<Func<TEntity, TEntity>> selectColumn = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, long pageIndex = 0, long itemsPerPage = 0, long  rangeOfPages = 0, QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default);
        void Attach(TEntity entity);
        TEntity Attach(TrackingActivityEnum trackingActivity, TId id);
        void AttachRange(IEnumerable<TEntity> entities);
        void AttachRange(params TEntity[] entities);
        IEnumerable<TEntity> AttachRange(params TId[] ids);
        void DetachObject(object entity);
        void Detach(TEntity entity);
        void DetachRange(IEnumerable<TEntity> entities);
        void DetachObjectRange(IEnumerable<object> entities);
        void Update(TEntity entity);
        void Update(TrackingActivityEnum trackingActivity, TEntity entity, Action<TEntity> updateAction);
        TEntity Update(TrackingActivityEnum trackingActivity, TId id, Action<TEntity> updateAction);
        IEnumerable<TEntity> UpdateRange(params TEntity[] entities);
        IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities);
        Task<IEnumerable<TEntity>> UpdateRange(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids, Func<TEntity, Task> updateFunc);
        Task<bool> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TEntity entity, Func<TEntity, Task> updateFunc, CancellationToken cancellationToken = default);
        Task<bool> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TEntity entity, Func<TEntity, Task> updateFunc, bool ignoreIfNotExists, CancellationToken cancellationToken = default);
        Task<bool> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TEntity entity, Action<TEntity> updateAction, CancellationToken cancellationToken = default);
        Task<bool> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TEntity entity, Action<TEntity> updateAction, bool ignoreIfNotExists, CancellationToken cancellationToken = default);
        Task<TEntity> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TId id, Func<TEntity, Task> updateFunc, CancellationToken cancellationToken = default);
        Task<TEntity> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TId id, Func<TEntity, Task> updateFunc, bool ignoreIfNotExists, CancellationToken cancellationToken = default);
        Task<TEntity> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TId id, Action<TEntity> updateAction, CancellationToken cancellationToken = default);
        Task<TEntity> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TId id, Action<TEntity> updateAction, bool ignoreIfNotExists, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TEntity> entities, Func<TEntity, Task> updateFunc, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TEntity> entities, Func<TEntity, Task> updateFunc, bool ignoreIfNotExists, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TEntity> entities, Action<TEntity> updateAction, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TEntity> entities, Action<TEntity> updateAction, bool ignoreIfNotExists, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids, Func<TEntity, Task> updateAction, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids, Func<TEntity, Task> updateAction, bool ignoreIfNotExists, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids, Action<TEntity> updateAction, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids, Action<TEntity> updateAction, bool ignoreIfNotExists, CancellationToken cancellationToken = default);
        void Remove(TEntity entity);
        void Remove(TId id);
        Task<bool> InstantRemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> InstantRemoveAsync(TEntity entity, bool ignoreIfNotExists, CancellationToken cancellationToken = default);
        Task<bool> InstantRemoveAsync(TId id, CancellationToken cancellationToken = default);
        Task<bool> InstantRemoveAsync(TId id, bool ignoreIfNotExists, CancellationToken cancellationToken = default);
        void RemoveRange(IEnumerable<TEntity> entities);
        void RemoveRange(params TEntity[] entities);
        void RemoveRange(params TId[] ids);
        Task<int> InstantRemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task<int> InstantRemoveRangeAsync(IEnumerable<TEntity> entities, bool ignoreIfNotExists, CancellationToken cancellationToken = default);
        Task<int> InstantRemoveRangeAsync(CancellationToken cancellationToken = default, params TEntity[] entities);
        Task<int> InstantRemoveRangeAsync(bool ignoreIfNotExists, CancellationToken cancellationToken, params TEntity[] entities);
        Task<int> InstantRemoveRangeAsync(CancellationToken cancellationToken, params TId[] ids);
        Task<int> InstantRemoveRangeAsync(bool ignoreIfNotExists, CancellationToken cancellationToken, params TId[] ids);
        Task<int> InstantRemoveRangeAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null, QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default);
        DatabaseProviderEnum GetDatabaseProvider();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default, TrackingActivityEnum trackingActivity = TrackingActivityEnum.Inactive);
        Task<int> SaveChangesWithDateAsync(CancellationToken cancellationToken = default, TrackingActivityEnum trackingActivity = TrackingActivityEnum.Inactive);
        void ClearChangeTracker();
    }
}