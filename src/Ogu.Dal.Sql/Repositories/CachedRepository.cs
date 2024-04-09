namespace Ogu.Dal.Sql.Repositories
{
    //Todo
    //public class CachedRepository<TEntity, TId> : Repository<TEntity, TId>, IRepository<TEntity, TId> where TEntity : class, IBaseEntity<TId>, new()
    //{
    //    private readonly IDistributedCache _cache;
    //    public CachedRepository(DbContext context, IDistributedCache cache) : base(context)
    //    {
    //        _cache = cache;
    //    }

    //    public override void Add(TEntity entity)
    //    {
    //        base.Add(entity);
    //    }

    //    public override Task AddAndFetchSpecialValueGeneratorsBeforeSavingAsync(TEntity entity, CancellationToken cancellationToken = default)
    //    {

    //        return base.AddAndFetchSpecialValueGeneratorsBeforeSavingAsync(entity, cancellationToken);
    //    }

    //    public override IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
    //    {
    //        return base.AddRange(entities); 
    //    }

    //    public override Task<IEnumerable<TEntity>> AddRangeAndFetchSpecialValueGeneratorsBeforeSavingAsync(IEnumerable<TEntity> entities,
    //        CancellationToken cancellationToken = default)
    //    {
    //        return base.AddRangeAndFetchSpecialValueGeneratorsBeforeSavingAsync(entities, cancellationToken);
    //    }

    //    public override Task<bool> InstantAddAsync(TEntity entity, CancellationToken cancellationToken = default)
    //    {
    //        return InstantAddAsync(entity, true, cancellationToken);
    //    }

    //    public override Task<bool> InstantAddAsync(TEntity entity, bool ignoreIfNotExist, CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantAddAsync(entity, ignoreIfNotExist, cancellationToken);
    //    }


    //    public override Task<IEnumerable<TEntity>> InstantAddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    //    {
    //        return InstantAddRangeAsync(entities, true, cancellationToken);
    //    }

    //    public override Task<IEnumerable<TEntity>> InstantAddRangeAsync(IEnumerable<TEntity> entities, bool ignoreIfNotExist, CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantAddRangeAsync(entities, ignoreIfNotExist, cancellationToken);
    //    }

    //    public override Task<TEntity> GetAsync(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null,
    //        string includeProperties = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
    //        Expression<Func<TEntity, TEntity>> selectColumn = null, QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery,
    //        CancellationToken cancellationToken = default)
    //    {

    //        return base.GetAsync(trackingActivity, predicate, includeProperties, orderBy, selectColumn,
    //            querySplittingBehavior, cancellationToken);
    //    }

    //    public override Task<TEntity> GetByIdAsync(TrackingActivityEnum trackingActivity, TId id, CancellationToken cancellationToken = default)
    //    {
    //        return base.GetByIdAsync(trackingActivity, id, cancellationToken);
    //    }

    //    public override IQueryable<TEntity> GetAllAsQueryable(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null,
    //        string includeProperties = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
    //        Expression<Func<TEntity, TEntity>> selectColumn = null, QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery)
    //    {
    //        return base.GetAllAsQueryable(trackingActivity, predicate, includeProperties, orderBy, selectColumn,
    //            querySplittingBehavior);
    //    }

    //    public override Task<IEnumerable<TEntity>> GetAllAsAsyncEnumerable(TrackingActivityEnum trackingActivity,
    //        Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null,
    //        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null,
    //        QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default)
    //    {
    //        return base.GetAllAsAsyncEnumerable(trackingActivity, predicate, includeProperties, orderBy, selectColumn,
    //            querySplittingBehavior, cancellationToken);
    //    }

    //    public override Task<IList<TEntity>> GetAllAsAsyncList(TrackingActivityEnum trackingActivity,
    //        Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null,
    //        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null,
    //        QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default)
    //    {
    //        return base.GetAllAsAsyncList(trackingActivity, predicate, includeProperties, orderBy, selectColumn,
    //            querySplittingBehavior, cancellationToken);
    //    }

    //    public override Task<TEntity[]> GetAllAsAsyncArray(TrackingActivityEnum trackingActivity,
    //        Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null,
    //        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
    //        Expression<Func<TEntity, TEntity>> selectColumn = null,
    //        QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery,
    //        CancellationToken cancellationToken = default)
    //    {
    //        return base.GetAllAsAsyncArray(trackingActivity, predicate, includeProperties, orderBy, selectColumn,
    //            querySplittingBehavior, cancellationToken);
    //    }

    //    public override Task<IPaginated<TEntity>> GetAllAsAsyncPaginated(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null,
    //        string includeProperties = null, Expression<Func<TEntity, TEntity>> selectColumn = null,
    //        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int pageIndex = 0, int itemsPerPage = 0, int rangeOfPages = 0,
    //        QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default)
    //    {
    //        return base.GetAllAsAsyncPaginated(trackingActivity, predicate, includeProperties, selectColumn, orderBy,
    //            pageIndex, itemsPerPage, rangeOfPages, querySplittingBehavior, cancellationToken);
    //    }

    //    public override Task<ILongPaginated<TEntity>> GetAllAsAsyncLongPaginatedModel(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null,
    //        string includeProperties = null, Expression<Func<TEntity, TEntity>> selectColumn = null,
    //        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, long pageIndex = 0,
    //        long itemsPerPage = 0, long rangeOfPages = 0, QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery,
    //        CancellationToken cancellationToken = default)
    //    {
    //        return base.GetAllAsAsyncLongPaginatedModel(trackingActivity, predicate, includeProperties, selectColumn,
    //            orderBy, pageIndex, itemsPerPage, rangeOfPages, querySplittingBehavior, cancellationToken);
    //    }

    //    public override void Attach(TEntity entity)
    //    {
    //        base.Attach(entity);
    //    }

    //    public override TEntity Attach(TrackingActivityEnum trackingActivity, TId id)
    //    {
    //        return base.Attach(trackingActivity, id);
    //    }

    //    public override void AttachRange(IEnumerable<TEntity> entities)
    //    {
    //        base.AttachRange(entities);
    //    }

    //    public override void AttachRange(params TEntity[] entities)
    //    {
    //        base.AttachRange(entities);
    //    }

    //    public override IEnumerable<TEntity> AttachRange(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids)
    //    {
    //        return base.AttachRange(trackingActivity, ids);
    //    }

    //    public override IEnumerable<TEntity> AttachRange(params TId[] ids)
    //    {
    //        return base.AttachRange(ids);
    //    }

    //    public override void DetachObject(object entity)
    //    {
    //       base.DetachObject(entity);
    //    }

    //    public override void Detach(TEntity entity)
    //    {
    //      base.Detach(entity);
    //    }

    //    public override void DetachRange(IEnumerable<TEntity> entities)
    //    {
    //        base.DetachRange(entities);
    //    }

    //    public override void DetachObjectRange(IEnumerable<object> entities)
    //    {
    //     base.DetachObjectRange(entities); 
    //    }

    //    public override void Update(TEntity entity)
    //    {
    //        base.Update(entity);
    //    }

    //    public override void Update(TrackingActivityEnum trackingActivity, TEntity entity, Action<TEntity> updateAction)
    //    {
    //        base.Update(trackingActivity, entity, updateAction);
    //    }

    //    public override TEntity Update(TrackingActivityEnum trackingActivity, TId id, Action<TEntity> updateAction)
    //    {
    //        return base.Update(trackingActivity, id, updateAction);
    //    }

    //    public override IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities)
    //    {
    //        return base.UpdateRange(entities);
    //    }

    //    public override IEnumerable<TEntity> UpdateRange(params TEntity[] entities)
    //    {
    //        return base.UpdateRange(entities);
    //    }

    //    public override Task<IEnumerable<TEntity>> UpdateRange(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids, Func<TEntity, Task> updateFunc)
    //    {
    //        return base.UpdateRange(trackingActivity, ids, updateFunc);
    //    }

    //    public override IEnumerable<TEntity> UpdateRange(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids, Action<TEntity> updateAction)
    //    {
    //        return base.UpdateRange(trackingActivity, ids, updateAction);
    //    }

    //    public override Task<bool> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TEntity entity,
    //        Func<TEntity, Task> updateFunc,
    //        CancellationToken cancellationToken = default)
    //    {
    //        return InstantUpdateAsync(trackingActivity, entity, updateFunc, true, cancellationToken);
    //    }

    //    public override Task<bool> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TEntity entity, Func<TEntity, Task> updateFunc, bool ignoreIfNotExist,
    //        CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantUpdateAsync(trackingActivity, entity, updateFunc, ignoreIfNotExist, cancellationToken);
    //    }

    //    public override Task<bool> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TEntity entity,
    //        Action<TEntity> updateAction,
    //        CancellationToken cancellationToken = default)
    //    {
    //        return InstantUpdateAsync(trackingActivity, entity, updateAction, true, cancellationToken);
    //    }

    //    public override Task<bool> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TEntity entity, Action<TEntity> updateAction,
    //        bool ignoreIfNotExist, CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantUpdateAsync(trackingActivity, entity, updateAction, ignoreIfNotExist, cancellationToken);
    //    }

    //    public override Task<TEntity> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TId id,
    //        Func<TEntity, Task> updateFunc,
    //        CancellationToken cancellationToken = default)
    //    {
    //        return InstantUpdateAsync(trackingActivity, id, updateFunc, true, cancellationToken);
    //    }

    //    public override Task<TEntity> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TId id, Func<TEntity, Task> updateFunc, bool ignoreIfNotExist, CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantUpdateAsync(trackingActivity, id, updateFunc, ignoreIfNotExist, cancellationToken);
    //    }

    //    public override Task<TEntity> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TId id,
    //        Action<TEntity> updateAction, CancellationToken cancellationToken = default)
    //    {
    //        return InstantUpdateAsync(trackingActivity, id, updateAction, true, cancellationToken);
    //    }

    //    public override Task<TEntity> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TId id, Action<TEntity> updateAction, bool ignoreIfNotExist, CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantUpdateAsync(trackingActivity, id, updateAction, ignoreIfNotExist, cancellationToken);
    //    }

    //    public override Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity,
    //        IEnumerable<TEntity> entities, Func<TEntity, Task> updateFunc,
    //        CancellationToken cancellationToken = default)
    //    {
    //        return InstantUpdateRangeAsync(trackingActivity, entities, updateFunc, true, cancellationToken);
    //    }

    //    public override Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TEntity> entities, Func<TEntity, Task> updateFunc, bool ignoreIfNotExists, CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantUpdateRangeAsync(trackingActivity, entities, updateFunc, ignoreIfNotExists,
    //            cancellationToken);
    //    }

    //    public override Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TEntity> entities, Action<TEntity> updateAction, CancellationToken cancellationToken = default)
    //    {
    //        return InstantUpdateRangeAsync(trackingActivity, entities, updateAction, true, cancellationToken);
    //    }

    //    public override Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TEntity> entities, Action<TEntity> updateAction, bool ignoreIfNotExists, CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantUpdateRangeAsync(trackingActivity, entities, updateAction, ignoreIfNotExists,
    //            cancellationToken);
    //    }

    //    public override Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity,
    //        IEnumerable<TId> ids, Func<TEntity, Task> updateAction,
    //        CancellationToken cancellationToken = default)
    //    {
    //        return this.InstantUpdateRangeAsync(trackingActivity, ids, updateAction, true, cancellationToken);
    //    }


    //    public override Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids, Func<TEntity, Task> updateFunc, bool ignoreIfNotExists, CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantUpdateRangeAsync(trackingActivity, ids, updateFunc, ignoreIfNotExists,
    //            cancellationToken);
    //    }

    //    public override Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity,
    //        IEnumerable<TId> ids, Action<TEntity> updateAction,
    //        CancellationToken cancellationToken = default)
    //    {
    //        return this.InstantUpdateRangeAsync(trackingActivity, ids, updateAction, true, cancellationToken);
    //    }

    //    public override Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids, Action<TEntity> updateAction, bool ignoreIfNotExists, CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantUpdateRangeAsync(trackingActivity, ids, updateAction, ignoreIfNotExists,
    //            cancellationToken);
    //    }

    //    public override void Remove(TEntity entity)
    //    {
    //        base.Remove(entity);
    //    }

    //    public override void Remove(TId id)
    //    {
    //        base.Remove(id);
    //    }

    //    public override void RemoveRange(IEnumerable<TEntity> entities)
    //    {
    //      base.RemoveRange(entities);
    //    }

    //    public override void RemoveRange(params TEntity[] entities)
    //    {
    //        base.RemoveRange(entities);
    //    }

    //    public override void RemoveRange(params TId[] ids)
    //    {
    //        base.RemoveRange(ids);
    //    }

    //    public override Task<bool> InstantRemoveAsync(TEntity entity,
    //        CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantRemoveAsync(entity, cancellationToken);
    //    }

    //    public override Task<bool> InstantRemoveAsync(TEntity entity, bool ignoreIfNotExist, CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantRemoveAsync(entity, ignoreIfNotExist, cancellationToken);
    //    }

    //    public override Task<bool> InstantRemoveAsync(TId id,
    //        CancellationToken cancellationToken = default)
    //    {
    //        return this.InstantRemoveAsync(id, true, cancellationToken);
    //    }

    //    public override Task<bool> InstantRemoveAsync(TId id, bool ignoreIfNotExist, CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantRemoveAsync(id, ignoreIfNotExist, cancellationToken);
    //    }

    //    public override Task<int> InstantRemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    //    {
    //        return this.InstantRemoveRangeAsync(entities, true, cancellationToken);
    //    }

    //    public override Task<int> InstantRemoveRangeAsync(IEnumerable<TEntity> entities, bool ignoreIfNotExists, CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantRemoveRangeAsync(entities, ignoreIfNotExists, cancellationToken);
    //    }

    //    public override Task<int> InstantRemoveRangeAsync(CancellationToken cancellationToken, params TEntity[] entities)
    //    {
    //        return this.InstantRemoveRangeAsync(true, cancellationToken, entities);
    //    }

    //    public override Task<int> InstantRemoveRangeAsync(bool ignoreIfNotExists, CancellationToken cancellationToken, params TEntity[] entities)
    //    {
    //        return base.InstantRemoveRangeAsync(ignoreIfNotExists, cancellationToken, entities);
    //    }

    //    public override Task<int> InstantRemoveRangeAsync(CancellationToken cancellationToken = default, params TId[] ids)
    //    {
    //        return this.InstantRemoveRangeAsync(true, cancellationToken, ids);
    //    }

    //    public override Task<int> InstantRemoveRangeAsync(bool ignoreIfNotExists, CancellationToken cancellationToken, params TId[] ids)
    //    {
    //        return base.InstantRemoveRangeAsync(ignoreIfNotExists, cancellationToken, ids);
    //    }

    //    public override Task<int> InstantRemoveRangeAsync(CancellationToken cancellationToken = default)
    //    {
    //        return base.InstantRemoveRangeAsync(cancellationToken);
    //    }

    //    public override Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
    //    {
    //        return base.CountAsync(predicate, cancellationToken);
    //    }

    //    public override Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
    //    {
    //        return base.LongCountAsync(predicate, cancellationToken);
    //    }

    //    public override Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null,
    //        QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default)
    //    {
    //        return base.IsAnyAsync(predicate, includeProperties, querySplittingBehavior, cancellationToken);
    //    }

    //    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default,
    //        TrackingActivityEnum trackingActivity = TrackingActivityEnum.Inactive)
    //    {
    //        return base.SaveChangesAsync(cancellationToken, trackingActivity);
    //    }

    //    public override Task<int> SaveChangesWithDateAsync(CancellationToken cancellationToken = default,
    //        TrackingActivityEnum trackingActivity = TrackingActivityEnum.Inactive)
    //    {
    //        return base.SaveChangesWithDateAsync(cancellationToken, trackingActivity);
    //    }

    //    private static class Keys
    //    {
    //        public const string AllKeys = nameof(AllKeys);
    //        public const string GetAsync = nameof(GetAsync);
    //        public const string GetByIdAsync = nameof(GetByIdAsync);
    //        public const string GetAllAsAsync = nameof(GetAllAsAsync);
    //        public const string GetAllAsAsyncPaginated = nameof(GetAllAsAsyncPaginated);
    //        public const string GetAllAsAsyncLongPaginated = nameof(GetAllAsAsyncLongPaginated);
    //        public const string CountAsync = nameof(CountAsync);
    //        public const string LongCountAsync = nameof(LongCountAsync);
    //        public const string IsAnyAsync = nameof(IsAnyAsync);

    //    }
    //}
}
