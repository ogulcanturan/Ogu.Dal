using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Ogu.Dal.Abstractions;
using Ogu.Dal.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ogu.Dal.MongoDb.Extensions
{
    public static class MongoDbExtensions
    {
        public static UpdateDefinition<TEntity> GetUpdateDefinitionByComparingTwoEntities<TEntity>(TEntity newEntity, TEntity currentEntity) where TEntity : BaseEntity
        {
            newEntity.CreatedOn = currentEntity.CreatedOn;
            var propertiesWithValueAssigned = typeof(TEntity).GetProperties().Where(x => x.GetValue(newEntity, null) != x.GetValue(currentEntity, null));
            var updateDefinitions = propertiesWithValueAssigned.Select(property => Builders<TEntity>.Update.Set(property.Name, property.GetValue(newEntity, null)));
            updateDefinitions = updateDefinitions.Append(Builders<TEntity>.Update.Set(nameof(BaseEntity.UpdatedOn), DateTime.UtcNow));
            return Builders<TEntity>.Update.Combine(updateDefinitions);
        }

        public static UpdateDefinition<TEntity> GetUpdateDefinitionByAnonymousType<TEntity>(object anonymousType) where TEntity : BaseEntity
        {
            var propertiesName = typeof(TEntity).GetProperties().Select(x => x.Name);
            var updateDefinitions = anonymousType.GetType().GetProperties().Where(x => propertiesName.Contains(x.Name)).Select(property => Builders<TEntity>.Update.Set(property.Name, property.GetValue(anonymousType, null)));
            updateDefinitions = updateDefinitions.Append(Builders<TEntity>.Update.Set(nameof(BaseEntity.UpdatedOn), DateTime.UtcNow));
            return Builders<TEntity>.Update.Combine(updateDefinitions);
        }

        public static async Task<IPaginated<TEntity>> ToPaginatedAsync<TEntity>(this IMongoQueryable<TEntity> items, Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>> orderBy, int pageIndex = 0, int itemsPerPage = 0, int rangeOfPages = 0, CancellationToken cancellationToken = default)
        {
            var totalItems = await items.CountAsync(cancellationToken).ConfigureAwait(false);

            List<TEntity> entities;

            if (totalItems > 0)
            {
                if (itemsPerPage > 0 && pageIndex > 0)
                    items = items?.Skip((pageIndex - 1) * itemsPerPage).Take(itemsPerPage);

                if (orderBy != null)
                    items = orderBy(items);

                entities = await items.ToListAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                entities = new List<TEntity>(0);
            }

            return new Paginated<TEntity>(pageIndex, itemsPerPage, totalItems, rangeOfPages, entities);
        }

        public static async Task<IPaginated<TEntity>> ToPaginatedAsync<TEntity>(this IMongoQueryable<TEntity> items, Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>> orderBy, CancellationToken cancellationToken = default)
        {
            var totalItems = await items.CountAsync(cancellationToken).ConfigureAwait(false);

            List<TEntity> entities;

            if (totalItems > 0)
            {
                if (orderBy != null)
                    items = orderBy(items);

                entities = await items.ToListAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                entities = new List<TEntity>(0);
            }

            return new Paginated<TEntity>(totalItems, entities);
        }

        public static async Task<ILongPaginated<TEntity>> ToLongPaginatedAsync<TEntity>(this IMongoQueryable<TEntity> items, Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>> orderBy = null, long pageIndex = 0, long itemsPerPage = 0, long rangeOfPages = 0, CancellationToken cancellationToken = default)
        {
            var totalItems = await items.LongCountAsync(cancellationToken).ConfigureAwait(false);

            List<TEntity> entities;

            if (totalItems > 0)
            {
                if (itemsPerPage > 0 && pageIndex > 0)
                    items = items.LongSkip((pageIndex - 1) * itemsPerPage).LongTake(itemsPerPage);
                if (orderBy != null)
                    items = orderBy(items);

                entities = await items.ToListAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                entities = new List<TEntity>(0);
            }

            return new LongPaginated<TEntity>(pageIndex, itemsPerPage, totalItems, rangeOfPages, entities);
        }

        public static async Task<ILongPaginated<TEntity>> ToLongPaginatedAsync<TEntity>(this IMongoQueryable<TEntity> items, CancellationToken cancellationToken)
        {
            var totalItems = await items.LongCountAsync(cancellationToken).ConfigureAwait(false);

            var entities = totalItems > 0 ? await items.ToListAsync(cancellationToken).ConfigureAwait(false) : new List<TEntity>(0);

            return new LongPaginated<TEntity>(totalItems, entities);
        }

        public static IMongoQueryable<T> LongSkip<T>(this IMongoQueryable<T> items, long count) => LongSkip(items, int.MaxValue, count);
        internal static IMongoQueryable<T> LongSkip<T>(IMongoQueryable<T> items, int maxSize, long count)
        {
            var segmentCount = Math.DivRem(count, maxSize, out var remainder);

            for (var i = 0L; i < segmentCount; i++)
                items = items.Skip(maxSize);

            if (remainder != 0)
                items = items.Skip((int)remainder);

            return items;
        }

        public static IMongoQueryable<T> LongTake<T>(this IMongoQueryable<T> items, long count) => LongTake(items, int.MaxValue, count);
        internal static IMongoQueryable<T> LongTake<T>(IMongoQueryable<T> items, int maxSize, long count)
        {
            var segmentCount = Math.DivRem(count, maxSize, out var remainder);

            for (var i = 0L; i < segmentCount; i++)
                items = items.Take(maxSize);

            if (remainder != 0)
                items = items.Take((int)remainder);

            return items;
        }
    }
}