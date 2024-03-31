using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Ogu.Dal.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ogu.Dal.MongoDb.Attributes;
using Ogu.Dal.MongoDb.Entities;
using Ogu.Dal.MongoDb.Extensions;

namespace Ogu.Dal.MongoDb.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

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
        }

        public IMongoCollection<TEntity> Table { get; }

        public IMongoQueryable<TEntity> AsQueryable() => Table.AsQueryable();

        public virtual Task<TEntity> InstantAddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.CreatedOn = DateTime.UtcNow;

            return Table.InsertOneAsync(entity, cancellationToken: cancellationToken).ContinueWith(t => entity, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> InstantAddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entitiesAsArray = entities as TEntity[] ?? entities.ToArray();

            if (entitiesAsArray.Length > 0)
            {
                foreach (var entity in entitiesAsArray)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }

                await Table.InsertManyAsync(entitiesAsArray, cancellationToken: cancellationToken);
            }

            return entitiesAsArray;
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var cursor = await Table.FindAsync(predicate, cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public virtual Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, TEntity>> selectColumn = null, CancellationToken cancellationToken = default)
        {
            var query = AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            if (selectColumn != null)
                query = query.Select(selectColumn);

            return query.FirstOrDefaultAsync(cancellationToken);
        }
        public virtual async Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cursor = await Table.FindAsync(Builders<TEntity>.Filter.Eq("_id", id), cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public virtual IMongoQueryable<TEntity> GetAllAsQueryable(
            Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, TEntity>> selectColumn = null,
            Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>> orderBy = null,
            CancellationToken cancellationToken = default)
        {
            var query = AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            if (selectColumn != null)
                query = query.Select(selectColumn);

            return orderBy != null ? orderBy(query) : query;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsAsyncEnumerable(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, TEntity>> selectColumn = null, Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>> orderBy = null, CancellationToken cancellationToken = default)
        { 
            var query = AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            if (selectColumn != null)
                query = query.Select(selectColumn);

            if (orderBy != null)
                return await orderBy(query).ToListAsync(cancellationToken);

            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<IList<TEntity>> GetAllAsAsyncList(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, TEntity>> selectColumn = null, Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>> orderBy = null, CancellationToken cancellationToken = default)
        {
            var query = AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            if (selectColumn != null)
                query = query.Select(selectColumn);

            if (orderBy != null)
                return await orderBy(query).ToListAsync(cancellationToken);

            return await query.ToListAsync(cancellationToken);
        }

        public virtual Task<IPaginated<TEntity>> GetAllAsAsyncPaginated(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, TEntity>> selectColumn = null, Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>> orderBy = null, int pageIndex = 0, int itemsPerPage = 0, int rangeOfPages = 0, CancellationToken cancellationToken = default)
        {
            var query = AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            if (selectColumn != null)
                query = query.Select(selectColumn);

            return query.ToPaginatedAsync(orderBy, pageIndex, itemsPerPage, rangeOfPages, cancellationToken);
        }

        public virtual Task<ILongPaginated<TEntity>> GetAllAsAsyncLongPaginated(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, TEntity>> selectColumn = null, Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>> orderBy = null, long pageIndex = 0, long itemsPerPage = 0, long rangeOfPages = 0, CancellationToken cancellationToken = default)
        {
            var query = AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            if (selectColumn != null)
                query = query.Select(selectColumn);

            return query.ToLongPaginatedAsync(orderBy, pageIndex, itemsPerPage, rangeOfPages, cancellationToken);
        }

        public virtual Task<TEntity> InstantReplaceAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if(entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.UpdatedOn = DateTime.UtcNow;
            return Table.FindOneAndReplaceAsync(predicate, entity, cancellationToken: cancellationToken);
        }

        public virtual Task<TEntity> InstantUpdateAsync(Expression<Func<TEntity, bool>> predicate, object anonymousType, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (anonymousType == null)
                throw new ArgumentNullException(nameof(anonymousType));

            return Table.FindOneAndUpdateAsync(predicate, MongoDbExtensions.GetUpdateDefinitionByAnonymousType<TEntity>(anonymousType), cancellationToken: cancellationToken);
        }

        public virtual async Task<TEntity> InstantUpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityFromDb = await GetAsync(predicate, cancellationToken).ConfigureAwait(false);

            if (entityFromDb == null)
                return null;

            await Table.UpdateOneAsync(predicate, MongoDbExtensions.GetUpdateDefinitionByComparingTwoEntities(entity, entityFromDb), cancellationToken: cancellationToken).ConfigureAwait(false);

            return entity;
        }

        public virtual async Task<long> InstantUpdateRangeAsync(Expression<Func<TEntity, bool>> predicate, object anonymousType, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (anonymousType == null)
                throw new ArgumentNullException(nameof(anonymousType));

            var updateResult = await Table.UpdateManyAsync(predicate, MongoDbExtensions.GetUpdateDefinitionByAnonymousType<TEntity>(anonymousType), cancellationToken: cancellationToken).ConfigureAwait(false);

            return updateResult.IsAcknowledged ? updateResult.MatchedCount : 0;
        }

        public virtual async Task<bool> InstantRemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var deleteResult = await Table.DeleteOneAsync(predicate, cancellationToken).ConfigureAwait(false);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public virtual async Task<bool> InstantRemoveAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var deleteResult = await Table.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id), cancellationToken).ConfigureAwait(false);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public virtual async Task<long> InstantRemoveRangeAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var deleteResult = await Table.DeleteManyAsync(predicate, cancellationToken).ConfigureAwait(false);

            return deleteResult.IsAcknowledged ? deleteResult.DeletedCount : 0;
        }

        public virtual async Task<long> InstantRemoveAllAsync(CancellationToken cancellationToken = default)
        {
            var deleteResult = await Table.DeleteManyAsync(Builders<TEntity>.Filter.Empty, cancellationToken).ConfigureAwait(false);

            return deleteResult.IsAcknowledged ? deleteResult.DeletedCount : 0;
        }

        public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate == null ? Table.EstimatedDocumentCountAsync(cancellationToken: cancellationToken) : Table.CountDocumentsAsync(predicate, cancellationToken: cancellationToken);
        }

        public virtual Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var query = AsQueryable();
            
            return predicate != null ?
                query.AnyAsync(predicate, cancellationToken) : 
                query.AnyAsync(cancellationToken);
        }

        public virtual async Task WithSessionAsync(Func<IMongoCollection<TEntity>, Task> action, CancellationToken cancellationToken = default)
        {
            using (var session = await _client.StartSessionAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                session.StartTransaction();
                await action(Table).ConfigureAwait(false);
                await session.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}