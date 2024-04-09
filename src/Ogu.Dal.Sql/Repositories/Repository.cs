using Microsoft.EntityFrameworkCore;
using Ogu.Dal.Abstractions;
using Ogu.Dal.Sql.Entities;
using Ogu.Dal.Sql.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ogu.Dal.Sql.Repositories
{
    public class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class, IBaseEntity<TId>, new()
    {
        private bool _disposed;
        private DatabaseProviderEnum? _databaseProvider;
        internal readonly DbContext Context;

        public Repository(DbContext context)
        {
            Context = context;
            Table = Context.Set<TEntity>();
        }

        public DbSet<TEntity> Table { get; }

        public IQueryable<TEntity> AsQueryable() => Table;

        public virtual void Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == null || entity.Id.Equals(default))
                return;

            entity.CreatedOn = DateTime.UtcNow;

            Table.Add(entity);
        }

        public virtual Task AddAndFetchSpecialValueGeneratorsBeforeSavingAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (!entity.Id.Equals(default))
                return Task.CompletedTask;

            entity.CreatedOn = DateTime.UtcNow;

            return Table.AddAsync(entity, cancellationToken).AsTask();
        }

        public virtual IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var currentTime = DateTime.UtcNow;

            var entitiesAsArray = entities
                .Where(e => e.Id == null || e.Id.Equals(default))
                .Select(e =>
                {
                    e.CreatedOn = currentTime;
                    return e;
                })
                .ToArray();

            if (entitiesAsArray.Length > 0)
            {
                Table.AddRange(entitiesAsArray);
            }

            return entitiesAsArray;
        }

        public virtual async Task<IEnumerable<TEntity>> AddRangeAndFetchSpecialValueGeneratorsBeforeSavingAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var currentTime = DateTime.UtcNow;

            var entitiesAsArray = entities
                .Where(e => e.Id == null || e.Id.Equals(default))
                .Select(e =>
                {
                    e.CreatedOn = currentTime;
                    return e;
                })
                .ToArray();

            if (entitiesAsArray.Length > 0)
            {
                await Table.AddRangeAsync(entitiesAsArray, cancellationToken).ConfigureAwait(false);
            }

            return entitiesAsArray;
        }

        public virtual Task<bool> InstantAddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return InstantAddAsync(entity, true, cancellationToken);
        }

        public virtual async Task<bool> InstantAddAsync(TEntity entity, bool ignoreIfNotExist, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id != null && !entity.Id.Equals(default(TId)))
                return false;

            entity.CreatedOn = DateTime.UtcNow;

            Table.Add(entity);

            try
            {
                return await SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExist)
            {
                return false;
            }
        }


        public virtual Task<IEnumerable<TEntity>> InstantAddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return InstantAddRangeAsync(entities, true, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> InstantAddRangeAsync(IEnumerable<TEntity> entities, bool ignoreIfNotExist, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var currentTime = DateTime.UtcNow;

            var entitiesAsArray = entities
                .Where(e => e.Id == null || e.Id.Equals(default))
                .Select(e =>
                {
                    e.CreatedOn = currentTime;
                    return e;
                })
                .ToArray();

            if (entitiesAsArray.Length == 0)
                return null;

            Table.AddRange(entitiesAsArray);

            try
            {
                return await SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0 ? entitiesAsArray : null;
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExist)
            {
                return null;
            }
        }

        public virtual Task<TEntity> GetAsync(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null,
            string includeProperties = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Expression<Func<TEntity, TEntity>> selectColumn = null, QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = Table;

            var isTrackingActivityIsInactive = trackingActivity == TrackingActivityEnum.Inactive;

            if (isTrackingActivityIsInactive)
                query = query.AsNoTracking();

            query = orderBy != null ? orderBy(query) ?? query.OrderBy(e => e.Id) : query.OrderBy(e => e.Id);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                query = IncludeProperties(includeProperties, query);
#if !NETSTANDARD2_0
                if (querySplittingBehavior != QuerySplittingBehavior.SingleQuery && isTrackingActivityIsInactive)
                    query = query.AsSplitQuery();
#endif
            }

            if (predicate != null)
                query = query.Where(predicate);

            if (selectColumn != null)
                query = query.Select(selectColumn);

            return isTrackingActivityIsInactive ?
                 query.FirstOrDefaultWithNoLockSessionAsync(cancellationToken) :
                 query.FirstOrDefaultAsync(cancellationToken);
        }

        public virtual Task<TEntity> GetByIdAsync(TrackingActivityEnum trackingActivity, TId id, CancellationToken cancellationToken = default)
        {
            return trackingActivity == TrackingActivityEnum.Active
                ? Table.FindAsync(new object[] { id }, cancellationToken).AsTask()
                : Table.AsNoTracking().Where(e => e.Id.Equals(id)).FirstOrDefaultWithNoLockSessionAsync(cancellationToken);
        }

        public virtual IQueryable<TEntity> GetAllAsQueryable(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null,
            string includeProperties = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Expression<Func<TEntity, TEntity>> selectColumn = null, QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery)
        {
            IQueryable<TEntity> query = Table;

            var isTrackingActivityIsInactive = trackingActivity == TrackingActivityEnum.Inactive;

            if (isTrackingActivityIsInactive)
                query = query.AsNoTracking();

            query = orderBy != null ? orderBy(query) ?? query.OrderBy(e => e.Id) : query.OrderBy(e => e.Id);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                query = IncludeProperties(includeProperties, query); ;
#if !NETSTANDARD2_0
                if (querySplittingBehavior != QuerySplittingBehavior.SingleQuery && isTrackingActivityIsInactive)
                    query = query.AsSplitQuery();
#endif
            }

            if (predicate != null)
                query = query.Where(predicate);

            if (selectColumn != null)
                query = query.Select(selectColumn);

            return query;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsAsyncEnumerable(TrackingActivityEnum trackingActivity,
            Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null,
            QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default)
        {
            return await GetAllAsAsyncArray(trackingActivity, predicate, includeProperties, orderBy, selectColumn,
                querySplittingBehavior, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<IList<TEntity>> GetAllAsAsyncList(TrackingActivityEnum trackingActivity,
            Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null,
            QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = Table;

            var isTrackingActivityIsInactive = trackingActivity == TrackingActivityEnum.Inactive;

            if (isTrackingActivityIsInactive)
                query = query.AsNoTracking();

            query = orderBy != null ? orderBy(query) ?? query.OrderBy(e => e.Id) : query.OrderBy(e => e.Id);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                query = IncludeProperties(includeProperties, query);
#if !NETSTANDARD2_0
                if (querySplittingBehavior != QuerySplittingBehavior.SingleQuery && isTrackingActivityIsInactive)
                    query = query.AsSplitQuery();
#endif
            }

            if (predicate != null)
                query = query.Where(predicate);

            if (selectColumn != null)
                query = query.Select(selectColumn);

            return isTrackingActivityIsInactive ?
                await query.ToListWithNoLockSessionAsync(cancellationToken) :
                await query.ToListAsync(cancellationToken);
        }

        public virtual Task<TEntity[]> GetAllAsAsyncArray(TrackingActivityEnum trackingActivity,
            Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Expression<Func<TEntity, TEntity>> selectColumn = null,
            QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = Table;

            var isTrackingActivityIsInactive = trackingActivity == TrackingActivityEnum.Inactive;

            if (isTrackingActivityIsInactive)
                query = query.AsNoTracking();

            query = orderBy != null ? orderBy(query) ?? query.OrderBy(e => e.Id) : query.OrderBy(e => e.Id);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                query = IncludeProperties(includeProperties, query);
#if !NETSTANDARD2_0
                if (querySplittingBehavior != QuerySplittingBehavior.SingleQuery && isTrackingActivityIsInactive)
                    query = query.AsSplitQuery();
#endif
            }

            if (predicate != null)
                query = query.Where(predicate);

            if (selectColumn != null)
                query = query.Select(selectColumn);

            return isTrackingActivityIsInactive ?
                query.ToArrayWithNoLockSessionAsync(cancellationToken) :
                query.ToArrayAsync(cancellationToken);
        }

        public virtual Task<IPaginated<TEntity>> GetAllAsAsyncPaginated(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null,
            string includeProperties = null, Expression<Func<TEntity, TEntity>> selectColumn = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int pageIndex = 0, int itemsPerPage = 0, int rangeOfPages = 0,
            QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = Table;

            var isTrackingActivityIsInactive = trackingActivity == TrackingActivityEnum.Inactive;

            if (isTrackingActivityIsInactive)
                query = query.AsNoTracking();

            query = orderBy != null ? orderBy(query) ?? query.OrderBy(e => e.Id) : query.OrderBy(e => e.Id);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                query = IncludeProperties(includeProperties, query);
#if !NETSTANDARD2_0
                if (querySplittingBehavior != QuerySplittingBehavior.SingleQuery && isTrackingActivityIsInactive)
                    query = query.AsSplitQuery();
#endif
            }

            if (predicate != null)
                query = query.Where(predicate);

            if (selectColumn != null)
                query = query.Select(selectColumn);

            return query.ToPaginatedAsync(pageIndex, itemsPerPage, rangeOfPages, cancellationToken);
        }

        public virtual Task<ILongPaginated<TEntity>> GetAllAsAsyncLongPaginatedModel(TrackingActivityEnum trackingActivity, Expression<Func<TEntity, bool>> predicate = null,
            string includeProperties = null, Expression<Func<TEntity, TEntity>> selectColumn = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, long pageIndex = 0,
            long itemsPerPage = 0, long rangeOfPages = 0, QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = Table;

            var isTrackingActivityIsInactive = trackingActivity == TrackingActivityEnum.Inactive;

            if (isTrackingActivityIsInactive)
                query = query.AsNoTracking();

            query = orderBy != null ? orderBy(query) ?? query.OrderBy(e => e.Id) : query.OrderBy(e => e.Id);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                query = IncludeProperties(includeProperties, query);
#if !NETSTANDARD2_0
                if (querySplittingBehavior != QuerySplittingBehavior.SingleQuery && isTrackingActivityIsInactive)
                    query = query.AsSplitQuery();
#endif
            }

            if (predicate != null)
                query = query.Where(predicate);

            if (selectColumn != null)
                query = query.Select(selectColumn);

            return query.ToLongPaginatedAsync(pageIndex, itemsPerPage, rangeOfPages, cancellationToken);
        }

        public virtual void Attach(TEntity entity) => Table.Attach(entity);

        public virtual TEntity Attach(TrackingActivityEnum trackingActivity, TId id)
        {
            TEntity entity = default;

            if (trackingActivity == TrackingActivityEnum.Active)
            {
                entity = Table.Local.FirstOrDefault(e => e.Id != null && e.Id.Equals(id));
            }

            if (entity == null)
            {
                entity = new TEntity { Id = id };

                Attach(entity);
            }

            return entity;
        }

        public virtual void AttachRange(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Table.AttachRange(entities);
        }

        public virtual void AttachRange(params TEntity[] entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Table.AttachRange(entities);
        }

        public virtual IEnumerable<TEntity> AttachRange(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            TEntity[] entities = default;

            if (trackingActivity == TrackingActivityEnum.Active)
            {
                entities = Table.Local.Where(e => ids.Contains(e.Id)).ToArray();
            }

            if (entities == null)
            {
                entities = ids.Select(id => new TEntity { Id = id }).ToArray();

                Table.AttachRange(entities);
            }

            return entities;
        }

        public virtual IEnumerable<TEntity> AttachRange(params TId[] ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var entities = ids.Select(id => new TEntity { Id = id }).ToArray();

            Table.AttachRange(entities);

            return entities;
        }

        public virtual void DetachObject(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Context.Entry(entity).State = EntityState.Detached;
        }

        public virtual void Detach(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Context.Entry<TEntity>(entity).State = EntityState.Detached;
        }

        public virtual void DetachRange(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                Detach(entity);
            }
        }

        public virtual void DetachObjectRange(IEnumerable<object> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                DetachObject(entity);
            }
        }

        public virtual void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == null || entity.Id.Equals(default))
                return;

            Table.Update(entity);

            entity.UpdatedOn = DateTime.UtcNow;
        }

        public virtual void Update(TrackingActivityEnum trackingActivity, TEntity entity, Action<TEntity> updateAction)
        {
            if (updateAction == null)
                throw new ArgumentNullException(nameof(updateAction));

            if (entity.Id == null || entity.Id.Equals(default))
                return;

            if (trackingActivity == TrackingActivityEnum.Inactive)
                Attach(entity);

            updateAction(entity);

            entity.UpdatedOn = DateTime.UtcNow;
        }

        public virtual TEntity Update(TrackingActivityEnum trackingActivity, TId id, Action<TEntity> updateAction)
        {
            if (updateAction == null)
                throw new ArgumentNullException(nameof(updateAction));

            if (id == null || id.Equals(default))
                return null;

            TEntity entity = default;

            if (trackingActivity == TrackingActivityEnum.Active)
            {
                entity = Table.Local.FirstOrDefault(e => e.Id != null && e.Id.Equals(id));
            }

            if (entity == null)
            {
                entity = new TEntity { Id = id };

                Table.Attach(entity);
            }

            updateAction(entity);

            entity.UpdatedOn = DateTime.UtcNow;

            return entity;
        }

        public virtual IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var currentTime = DateTime.UtcNow;

            var entitiesAsArray = entities
                .Where(e => e.Id != null && !e.Id.Equals(default))
                .Select(e =>
                {
                    e.UpdatedOn = currentTime;
                    return e;
                })
                .ToArray();

            if (entitiesAsArray.Length == 0)
                return Enumerable.Empty<TEntity>();

            Table.UpdateRange(entitiesAsArray);

            return entitiesAsArray;
        }

        public virtual IEnumerable<TEntity> UpdateRange(params TEntity[] entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var currentTime = DateTime.UtcNow;

            var entitiesAsArray = entities
                .Where(e => e.Id != null && !e.Id.Equals(default))
                .Select(e =>
                {
                    e.UpdatedOn = currentTime;
                    return e;
                })
                .ToArray();

            if (entities.Length == 0)
                return Enumerable.Empty<TEntity>();

            Table.UpdateRange(entitiesAsArray);

            return entitiesAsArray;
        }

        public virtual async Task<IEnumerable<TEntity>> UpdateRange(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids, Func<TEntity, Task> updateFunc)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var entitiesAsArray = ids
                .Where(id => id != null && !id.Equals(default))
                .Select(id => new TEntity { Id = id }).ToArray();

            if (entitiesAsArray.Length == 0)
                return Enumerable.Empty<TEntity>();

            Table.AttachRange(entitiesAsArray);

            var currentTime = DateTime.UtcNow;

            foreach (var entity in entitiesAsArray)
            {
                await updateFunc(entity).ConfigureAwait(false);

                entity.UpdatedOn = currentTime;
            }

            return entitiesAsArray;
        }

        public virtual IEnumerable<TEntity> UpdateRange(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids, Action<TEntity> updateAction)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            if (updateAction == null)
                throw new ArgumentNullException(nameof(updateAction));

            var entitiesAsArray = ids
                .Where(id => id != null && !id.Equals(default))
                .Select(id => new TEntity { Id = id }).ToArray();

            if (entitiesAsArray.Length == 0)
                return Enumerable.Empty<TEntity>();

            var currentTime = DateTime.UtcNow;

            foreach (var entity in entitiesAsArray)
            {
                updateAction(entity);

                entity.UpdatedOn = currentTime;
            }

            return entitiesAsArray;
        }

        public virtual Task<bool> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TEntity entity,
            Func<TEntity, Task> updateFunc,
            CancellationToken cancellationToken = default)
        {
            return InstantUpdateAsync(trackingActivity, entity, updateFunc, true, cancellationToken);
        }

        public virtual async Task<bool> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TEntity entity, Func<TEntity, Task> updateFunc, bool ignoreIfNotExist,
            CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (updateFunc == null)
                throw new ArgumentNullException(nameof(updateFunc));

            if (entity.Id == null || entity.Id.Equals(default))
                return false;

            if (trackingActivity == TrackingActivityEnum.Inactive)
                Table.Attach(entity);

            await updateFunc(entity).ConfigureAwait(false);

            entity.UpdatedOn = DateTime.UtcNow;

            try
            {
                return await SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExist)
            {
                return false;
            }
        }

        public virtual Task<bool> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TEntity entity,
            Action<TEntity> updateAction,
            CancellationToken cancellationToken = default)
        {
            return InstantUpdateAsync(trackingActivity, entity, updateAction, true, cancellationToken);
        }

        public virtual async Task<bool> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TEntity entity, Action<TEntity> updateAction,
            bool ignoreIfNotExist, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (updateAction == null)
                throw new ArgumentNullException(nameof(updateAction));

            if (entity.Id == null || entity.Id.Equals(default))
                return false;

            if (trackingActivity == TrackingActivityEnum.Inactive)
                Table.Attach(entity);

            updateAction(entity);

            entity.UpdatedOn = DateTime.UtcNow;

            try
            {
                return await SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExist)
            {
                return false;
            }
        }

        public virtual Task<TEntity> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TId id,
            Func<TEntity, Task> updateFunc,
            CancellationToken cancellationToken = default)
        {
            return InstantUpdateAsync(trackingActivity, id, updateFunc, true, cancellationToken);
        }

        public virtual async Task<TEntity> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TId id, Func<TEntity, Task> updateFunc, bool ignoreIfNotExist, CancellationToken cancellationToken = default)
        {
            if (updateFunc == null)
                throw new ArgumentNullException(nameof(updateFunc));

            if (id == null || id.Equals(default))
                return null;

            TEntity entity = default;

            if (trackingActivity == TrackingActivityEnum.Active)
            {
                entity = Table.Local.FirstOrDefault(e => e.Id != null && e.Id.Equals(id));
            }

            if (entity == null)
            {
                entity = new TEntity { Id = id };

                Attach(entity);
            }

            await updateFunc(entity).ConfigureAwait(false);

            entity.UpdatedOn = DateTime.UtcNow;

            try
            {
                return await SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0 ? entity : null;
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExist)
            {
                return null;
            }
        }

        public virtual Task<TEntity> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TId id,
            Action<TEntity> updateAction, CancellationToken cancellationToken = default)
        {
            return InstantUpdateAsync(trackingActivity, id, updateAction, true, cancellationToken);
        }

        public virtual async Task<TEntity> InstantUpdateAsync(TrackingActivityEnum trackingActivity, TId id, Action<TEntity> updateAction, bool ignoreIfNotExist, CancellationToken cancellationToken = default)
        {
            if (updateAction == null)
                throw new ArgumentNullException(nameof(updateAction));

            if (id == null || id.Equals(default))
                return null;

            TEntity entity = default;

            if (trackingActivity == TrackingActivityEnum.Active)
            {
                entity = Table.Local.FirstOrDefault(e => e.Id != null && e.Id.Equals(id));
            }

            if (entity == null)
            {
                entity = new TEntity { Id = id };

                Attach(entity);
            }

            updateAction(entity);

            entity.UpdatedOn = DateTime.UtcNow;

            try
            {
                return await SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0 ? entity : null;
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExist)
            {
                return null;
            }
        }

        public virtual Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity,
            IEnumerable<TEntity> entities, Func<TEntity, Task> updateFunc,
            CancellationToken cancellationToken = default)
        {
            return InstantUpdateRangeAsync(trackingActivity, entities, updateFunc, true, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TEntity> entities, Func<TEntity, Task> updateFunc, bool ignoreIfNotExists, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entitiesAsArray = entities
                .Where(e => e.Id != null && !e.Id.Equals(default))
                .ToArray();

            if (entitiesAsArray.Length == 0)
                return Enumerable.Empty<TEntity>();

            if (trackingActivity == TrackingActivityEnum.Inactive)
                Table.AttachRange(entitiesAsArray);

            var currentTime = DateTime.UtcNow;

            foreach (var entity in entitiesAsArray)
            {
                await updateFunc(entity).ConfigureAwait(false);

                entity.UpdatedOn = currentTime;
            }

            try
            {
                return await SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0 ? entitiesAsArray : null;
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExists)
            {
                return Enumerable.Empty<TEntity>();
            }
        }

        public virtual Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TEntity> entities, Action<TEntity> updateAction, CancellationToken cancellationToken = default)
        {
            return InstantUpdateRangeAsync(trackingActivity, entities, updateAction, true, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TEntity> entities, Action<TEntity> updateAction, bool ignoreIfNotExists, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entitiesAsArray = entities
                .Where(e => e.Id != null && !e.Id.Equals(default))
                .ToArray();

            if (entitiesAsArray.Length == 0)
                return Enumerable.Empty<TEntity>();

            if (trackingActivity == TrackingActivityEnum.Inactive)
                Table.AttachRange(entitiesAsArray);

            var currentTime = DateTime.UtcNow;

            foreach (var entity in entitiesAsArray)
            {
                updateAction(entity);

                entity.UpdatedOn = currentTime;
            }

            try
            {
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return entitiesAsArray;
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExists)
            {
                return Enumerable.Empty<TEntity>();
            }
        }

        public virtual Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity,
            IEnumerable<TId> ids, Func<TEntity, Task> updateAction,
            CancellationToken cancellationToken = default)
        {
            return InstantUpdateRangeAsync(trackingActivity, ids, updateAction, true, cancellationToken);
        }


        public virtual async Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids, Func<TEntity, Task> updateFunc, bool ignoreIfNotExists, CancellationToken cancellationToken = default)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var entitiesAsArray = ids
                .Where(id => id != null && !id.Equals(default))
                .Select(id => new TEntity { Id = id }).ToArray();

            if (entitiesAsArray.Length == 0)
                return Enumerable.Empty<TEntity>();

            Table.AttachRange(entitiesAsArray);

            var currentTime = DateTime.UtcNow;

            foreach (var entity in entitiesAsArray)
            {
                await updateFunc(entity).ConfigureAwait(false);

                entity.UpdatedOn = currentTime;
            }

            try
            {
                return await SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0 ? entitiesAsArray : null;
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExists)
            {
                return Enumerable.Empty<TEntity>();
            }
        }

        public virtual Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity,
            IEnumerable<TId> ids, Action<TEntity> updateAction,
            CancellationToken cancellationToken = default)
        {
            return InstantUpdateRangeAsync(trackingActivity, ids, updateAction, true, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> InstantUpdateRangeAsync(TrackingActivityEnum trackingActivity, IEnumerable<TId> ids, Action<TEntity> updateAction, bool ignoreIfNotExists, CancellationToken cancellationToken = default)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            if (updateAction == null)
                throw new ArgumentNullException(nameof(updateAction));

            var entitiesAsArray = ids
                .Where(id => id != null && !id.Equals(default))
                .Select(id => new TEntity { Id = id }).ToArray();

            if (entitiesAsArray.Length == 0)
                return Enumerable.Empty<TEntity>();

            Table.AttachRange(entitiesAsArray);

            var currentTime = DateTime.UtcNow;

            foreach (var entity in entitiesAsArray)
            {
                updateAction(entity);

                entity.UpdatedOn = currentTime;
            }

            try
            {
                return await SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0 ? entitiesAsArray : null;
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExists)
            {
                return Enumerable.Empty<TEntity>();
            }
        }

        public virtual void Remove(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id != null && !entity.Id.Equals(default))
                Table.Remove(entity);
        }

        public virtual void Remove(TId id)
        {
            if (id != null && !id.Equals(default))
                Table.Remove(new TEntity { Id = id });
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entitiesAsArray = entities
                .Where(e => e.Id != null && !e.Id.Equals(default))
                .ToArray();

            if (entitiesAsArray.Length > 0)
                Table.RemoveRange(entitiesAsArray);
        }

        public virtual void RemoveRange(params TEntity[] entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entitiesAsArray = entities
                .Where(e => e.Id != null && !e.Id.Equals(default))
                .ToArray();

            if (entitiesAsArray.Length > 0)
                Table.RemoveRange(entitiesAsArray);
        }

        public virtual void RemoveRange(params TId[] ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var filteredEntities = ids
                .Where(id => id != null && !id.Equals(default))
                .Select(id => new TEntity { Id = id })
                .ToArray();

            if (filteredEntities.Length > 0)
                Table.RemoveRange(filteredEntities);
        }

        public virtual Task<bool> InstantRemoveAsync(TEntity entity,
            CancellationToken cancellationToken = default)
        {
            return InstantRemoveAsync(entity, true, cancellationToken);
        }

        public virtual async Task<bool> InstantRemoveAsync(TEntity entity, bool ignoreIfNotExist, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == null || entity.Id.Equals(default))
                return false;

            Table.Remove(entity);

            try
            {
                return await SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExist)
            {
                return false;
            }
        }

        public virtual Task<bool> InstantRemoveAsync(TId id,
            CancellationToken cancellationToken = default)
        {
            return InstantRemoveAsync(id, true, cancellationToken);
        }

        public virtual async Task<bool> InstantRemoveAsync(TId id, bool ignoreIfNotExist, CancellationToken cancellationToken = default)
        {
            if (id == null || id.Equals(default))
                return false;

            Table.Remove(new TEntity { Id = id });

            try
            {
                return await SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExist)
            {
                return false;
            }
        }

        public virtual Task<int> InstantRemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return InstantRemoveRangeAsync(entities, true, cancellationToken);
        }

        public virtual Task<int> InstantRemoveRangeAsync(IEnumerable<TEntity> entities, bool ignoreIfNotExists, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entitiesAsArray = entities
                .Where(e => e.Id != null && !e.Id.Equals(default))
                .ToArray();

            if (entitiesAsArray.Length == 0)
                return Task.FromResult(0);

            Table.RemoveRange(entitiesAsArray);

            try
            {
                return SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExists)
            {
                return Task.FromResult(0);
            }
        }

        public virtual Task<int> InstantRemoveRangeAsync(CancellationToken cancellationToken, params TEntity[] entities)
        {
            return InstantRemoveRangeAsync(true, cancellationToken, entities);
        }

        public virtual Task<int> InstantRemoveRangeAsync(bool ignoreIfNotExists, CancellationToken cancellationToken, params TEntity[] entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entitiesAsArray = entities
                .Where(e => e.Id != null && !e.Id.Equals(default))
                .ToArray();

            if (entitiesAsArray.Length == 0)
                return Task.FromResult(0);

            Table.RemoveRange(entitiesAsArray);

            try
            {
                return SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExists)
            {
                return Task.FromResult(0);
            }
        }

        public virtual Task<int> InstantRemoveRangeAsync(CancellationToken cancellationToken = default, params TId[] ids)
        {
            return InstantRemoveRangeAsync(true, cancellationToken, ids);
        }

        public virtual Task<int> InstantRemoveRangeAsync(bool ignoreIfNotExists, CancellationToken cancellationToken, params TId[] ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var entitiesAsArray = ids
                .Where(id => id != null && !id.Equals(default))
                .Select(id => new TEntity { Id = id }).ToArray();

            if (entitiesAsArray.Length == 0)
                return Task.FromResult(0);

            Table.RemoveRange(entitiesAsArray);

            try
            {
                return SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (ignoreIfNotExists)
            {
                return Task.FromResult(0);
            }
        }

        public virtual Task<int> InstantRemoveRangeAsync(CancellationToken cancellationToken = default)
        {
            var tableName = Context.GetTableNameOrDefault<TEntity>();

            if (tableName == null)
                return Task.FromResult(0);

            var sqlCommand = Context.IsSqliteServer() ? "DELETE FROM [{0}]" : "TRUNCATE TABLE [{0}]";

            return Context.Database.ExecuteSqlRawAsync(string.Format(sqlCommand, tableName), cancellationToken);
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = Table.AsNoTracking().OrderBy(e => e.Id);

            if (predicate != null)
                query = query.Where(predicate);

            return query.CountWithNoLockSessionAsync(cancellationToken);
        }

        public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = Table.AsNoTracking().OrderBy(e => e.Id);

            if (predicate != null)
                query = query.Where(predicate);

            return query.LongCountWithNoLockSessionAsync(cancellationToken);
        }

        public virtual Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> predicate = null, string includeProperties = null,
            QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery, CancellationToken cancellationToken = default)
        {
            var query = Table.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                query = IncludeProperties(includeProperties, query);
#if !NETSTANDARD2_0
                if (querySplittingBehavior != QuerySplittingBehavior.SingleQuery)
                    query = query.AsSplitQuery();
#endif
            }

            return SqlExtensions.GetUncommittedAsyncScope(() => predicate != null ?
                query.AnyAsync(predicate, cancellationToken) :
                query.AnyAsync(cancellationToken));
        }

        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default,
            TrackingActivityEnum trackingActivity = TrackingActivityEnum.Inactive)
            => Context.SaveChangesAsync(trackingActivity, cancellationToken);

        public virtual Task<int> SaveChangesWithDateAsync(CancellationToken cancellationToken = default,
            TrackingActivityEnum trackingActivity = TrackingActivityEnum.Inactive)
            => Context.SaveChangesWithDateAsync(trackingActivity, cancellationToken);

        public virtual DatabaseProviderEnum GetDatabaseProvider()
        {
            if (!_databaseProvider.HasValue)
            {
                _databaseProvider = Context.GetDatabaseProvider();
            }

            return _databaseProvider.Value;
        }

        public virtual void ClearChangeTracker()
        {
#if NETSTANDARD2_0
            Context.ChangeTracker.Entries().Where(x => x.Entity != null).ToList().ForEach(x => x.State = EntityState.Detached);
#else
            Context.ChangeTracker.Clear();
#endif
        }

        private static IQueryable<TEntity> IncludeProperties(string includeProperties, IQueryable<TEntity> query)
        {
            return includeProperties
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .Aggregate(query,
                    (current, includeProperty) =>
                        current.Include(includeProperty));
        }

        ~Repository() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
                Context?.Dispose();

            this._disposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (Context != null && !_disposed)
                await Context.DisposeAsync().ConfigureAwait(false);
        }
    }
}