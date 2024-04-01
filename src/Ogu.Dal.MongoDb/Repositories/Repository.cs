using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Ogu.Dal.Abstractions;
using Ogu.Dal.MongoDb.Attributes;
using Ogu.Dal.MongoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Ogu.Dal.MongoDb.Repositories
{
    public class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class, IBaseEntity<TId>, new() where TId : IEquatable<TId>
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly HashSet<string> _propertyNameSet;

        public Repository(IMongoClient client, string database = null, string table = null)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            if (string.IsNullOrEmpty(database))
            {
                database = typeof(TEntity).GetCustomAttribute<MongoDatabaseAttribute>()?.Database;

                if (string.IsNullOrWhiteSpace(database))
                    throw new ArgumentException("Property name cannot be null or whitespace.", nameof(database));
            }

            if (string.IsNullOrEmpty(table))
            {
                table = typeof(TEntity).GetCustomAttribute<MongoDatabaseAttribute>()?.Table;

                if (string.IsNullOrWhiteSpace(table))
                    throw new ArgumentException("Property name cannot be null or whitespace.", nameof(table));
            }

            _database = _client.GetDatabase(database);

            Table = _database.GetCollection<TEntity>(table);

            _propertyNameSet = new HashSet<string>(typeof(TEntity).GetProperties().Select(x => x.Name));
        }

        public IMongoCollection<TEntity> Table { get; }

        public IMongoQueryable<TEntity> AsQueryable() => Table.AsQueryable();

        public virtual Task<bool> InstantAddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id != null && !entity.Id.Equals(default))
                return Task.FromResult(false);

            entity.CreatedOn = DateTime.UtcNow;

            return Table.InsertOneAsync(entity, cancellationToken: cancellationToken).ContinueWith(t => !t.IsFaulted, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> InstantAddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var currentTime = DateTime.UtcNow;
            
            var entitiesAsArray = entities
                .Where(e => e.Id == null || e.Id.Equals(default(TId)))
                .Select(e =>
                {
                    e.CreatedOn = currentTime;
                    return e;
                })
                .ToArray();

            if (entitiesAsArray.Length == 0)
                return null;

            await Table.InsertManyAsync(entitiesAsArray, cancellationToken: cancellationToken).ConfigureAwait(false);

            return entitiesAsArray;
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if(predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var cursor = await Table.FindAsync(predicate, cancellationToken: cancellationToken).ConfigureAwait(false);

            return await cursor.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null, CancellationToken cancellationToken = default)
        {
            var fluentQuery = Table.Find(predicate ?? FilterDefinition<TEntity>.Empty);

            if (orderBy != null)
            {
                fluentQuery = fluentQuery.Sort(orderBy.ToSortDefinition());
            }

            if (selectColumn != null)
            {
                fluentQuery.Project(selectColumn);
            }

            return fluentQuery.FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var cursor = await Table.FindAsync(Builders<TEntity>.Filter.Eq("_id", id), cancellationToken: cancellationToken).ConfigureAwait(false);

            return await cursor.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual IMongoQueryable<TEntity> GetAllAsQueryable(Expression<Func<TEntity, bool>> predicate = null, Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null, CancellationToken cancellationToken = default)
        {
            var query = AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            if (selectColumn != null)
                query = query.Select(selectColumn);

            return orderBy != null ? orderBy(query) : query;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsAsyncEnumerable(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null, CancellationToken cancellationToken = default)
        {
            return await GetAllAsAsyncList(predicate, orderBy, selectColumn, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<IList<TEntity>> GetAllAsAsyncList(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null, CancellationToken cancellationToken = default)
        {
            var fluentQuery = Table.Find(predicate ?? FilterDefinition<TEntity>.Empty);

            if (orderBy != null)
            {
                fluentQuery = fluentQuery.Sort(orderBy.ToSortDefinition());
            }

            if (selectColumn != null)
            {
                fluentQuery.Project(selectColumn);
            }

            return await fluentQuery.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<IPaginated<TEntity>> GetAllAsAsyncPaginated(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null, int pageIndex = 0, int itemsPerPage = 0, int rangeOfPages = 0, CancellationToken cancellationToken = default)
        {
            var isPredicateNull = predicate == null;

            var fluentQuery = Table.Find(isPredicateNull ? FilterDefinition<TEntity>.Empty : predicate);

            if (orderBy != null)
            {
                fluentQuery = fluentQuery.Sort(orderBy.ToSortDefinition());
            }

            if (selectColumn != null)
            {
                fluentQuery.Project(selectColumn);
            }

            var totalItems = (int)
                await (predicate == null
                    ? Table.EstimatedDocumentCountAsync(cancellationToken: cancellationToken)
                    : fluentQuery.CountDocumentsAsync(cancellationToken)).ConfigureAwait(false);

            return await fluentQuery.ToPaginatedAsync(totalItems, pageIndex, itemsPerPage, rangeOfPages, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<ILongPaginated<TEntity>> GetAllAsAsyncLongPaginated(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Expression<Func<TEntity, TEntity>> selectColumn = null, int pageIndex = 0, int itemsPerPage = 0, int rangeOfPages = 0, CancellationToken cancellationToken = default)
        {
            var isPredicateNull = predicate == null;

            var fluentQuery = Table.Find(isPredicateNull ? FilterDefinition<TEntity>.Empty : predicate);

            if (orderBy != null)
            {
                fluentQuery = fluentQuery.Sort(orderBy.ToSortDefinition());
            }

            if (selectColumn != null)
            {
                fluentQuery.Project(selectColumn);
            }

            var totalItems =
                await (predicate == null
                    ? Table.EstimatedDocumentCountAsync(cancellationToken: cancellationToken)
                    : fluentQuery.CountDocumentsAsync(cancellationToken)).ConfigureAwait(false);

            return await fluentQuery.ToLongPaginatedAsync(totalItems, pageIndex, itemsPerPage, rangeOfPages, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<TEntity> InstantReplaceAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if(entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == null || entity.Id.Equals(default))
                return null;

            var filter = Builders<TEntity>.Filter.Eq("_id", entity.Id);

            entity.UpdatedOn = DateTime.UtcNow;

            var result = await Table.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken).ConfigureAwait(false);

            return result.IsAcknowledged && result.ModifiedCount > 0 ? entity : null;
        }

        public virtual async Task<TEntity> InstantReplaceAndReturnPrevious(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == null || entity.Id.Equals(default))
                return null;

            var filter = Builders<TEntity>.Filter.Eq("_id", entity.Id);

            entity.UpdatedOn = DateTime.UtcNow;

            var previousEntity = await Table.FindOneAndReplaceAsync(filter, entity, cancellationToken: cancellationToken).ConfigureAwait(false);

            return previousEntity;
        }

        public virtual Task<TEntity> InstantUpdateAsync(object id, Expression<Func<TEntity, TEntity>> selector, CancellationToken cancellationToken = default)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var filter = Builders<TEntity>.Filter.Eq("_id", id);

            return Table.FindOneAndUpdateAsync(filter, selector.ToUpdateDefinition(), cancellationToken: cancellationToken);
        }

        public virtual Task<TEntity> InstantUpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> selector,  CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return Table.FindOneAndUpdateAsync(predicate, selector.ToUpdateDefinition(), cancellationToken: cancellationToken);
        }

        public virtual Task<TEntity> InstantUpdateAsync(object id, object anonymousType, CancellationToken cancellationToken = default)
        {
            if (anonymousType == null)
                throw new ArgumentNullException(nameof(anonymousType));

            var filter = Builders<TEntity>.Filter.Eq("_id", id);

            return Table.FindOneAndUpdateAsync(filter, MongoDbExtensions.GetUpdateDefinitionByAnonymousType<TEntity, TId>(anonymousType, _propertyNameSet), cancellationToken: cancellationToken);
        }

        public virtual Task<TEntity> InstantUpdateAsync(Expression<Func<TEntity, bool>> predicate, object anonymousType, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (anonymousType == null)
                throw new ArgumentNullException(nameof(anonymousType));
            
            return Table.FindOneAndUpdateAsync(predicate, MongoDbExtensions.GetUpdateDefinitionByAnonymousType<TEntity, TId>(anonymousType, _propertyNameSet), cancellationToken: cancellationToken);
        }

        public virtual async Task<TEntity> InstantUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == null || entity.Id.Equals(default))
                return null;

            var filter = Builders<TEntity>.Filter.Eq("_id", entity.Id);

            entity.UpdatedOn = DateTime.UtcNow;

            var result = await Table.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken).ConfigureAwait(false);
            
            return result.IsAcknowledged && result.ModifiedCount > 0 ? entity : null;
        }

        public virtual async Task<long> InstantUpdateRangeAsync(Expression<Func<TEntity, bool>> predicate, object anonymousType, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (anonymousType == null)
                throw new ArgumentNullException(nameof(anonymousType));

            var updateResult = await Table.UpdateManyAsync(predicate, MongoDbExtensions.GetUpdateDefinitionByAnonymousType<TEntity, TId>(anonymousType, _propertyNameSet), cancellationToken: cancellationToken).ConfigureAwait(false);

            return updateResult.IsAcknowledged ? updateResult.MatchedCount : 0;
        }

        public virtual async Task<long> InstantUpdateRangeAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> selector, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var updateResult = await Table.UpdateManyAsync(predicate, selector.ToUpdateDefinition(), cancellationToken: cancellationToken).ConfigureAwait(false);

            return updateResult.IsAcknowledged ? updateResult.MatchedCount : 0;
        }

        public virtual async Task<bool> InstantRemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var deleteResult = await Table.DeleteOneAsync(predicate, cancellationToken).ConfigureAwait(false);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public virtual async Task<bool> InstantRemoveAsync(object id, CancellationToken cancellationToken = default)
        {
            var deleteResult = await Table.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id), cancellationToken).ConfigureAwait(false);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public virtual async Task<long> InstantRemoveRangeAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var deleteResult = await Table.DeleteManyAsync(predicate, cancellationToken).ConfigureAwait(false);

            return deleteResult.IsAcknowledged ? deleteResult.DeletedCount : 0;
        }

        public virtual async Task<long> InstantRemoveRangeAsync(CancellationToken cancellationToken = default)
        {
            var deleteResult = await Table.DeleteManyAsync(Builders<TEntity>.Filter.Empty, cancellationToken).ConfigureAwait(false);

            return deleteResult.IsAcknowledged ? deleteResult.DeletedCount : 0;
        }

        public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate == null ? Table.EstimatedDocumentCountAsync(cancellationToken: cancellationToken) : Table.CountDocumentsAsync(predicate, cancellationToken: cancellationToken);
        }

        public virtual async Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var projection = Builders<TEntity>.Projection.Include(entity => entity.Id);

            var result = await Table.Find(predicate ?? Builders<TEntity>.Filter.Empty).Project(projection).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

            return result != null;
        }

        public virtual async Task WithSessionAsync(Func<IMongoCollection<TEntity>, Task> func, CancellationToken cancellationToken = default)
        {
            using (var session = await _client.StartSessionAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                session.StartTransaction();
                await func(Table).ConfigureAwait(false);
                await session.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}