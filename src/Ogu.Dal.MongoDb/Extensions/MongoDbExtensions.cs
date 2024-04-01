using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Ogu.Dal.Abstractions;
using Ogu.Dal.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Ogu.Dal.MongoDb.Extensions
{
    public static class MongoDbExtensions
    {
        public static UpdateDefinition<T> ToUpdateDefinition<T>(this Expression<Func<T, T>> exp)
        {
            return Builders<T>.Update.Combine(GetPropertyNameToValueDictionary(exp).Select(kvp =>
                    Builders<T>.Update.Set(kvp.Key, kvp.Value))
                .Append(Builders<T>.Update.Set(nameof(BaseEntity.UpdatedOn), DateTime.UtcNow)));
        }

        public static SortDefinition<T> ToSortDefinition<T>(this Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
        {
            SortDefinition<T> sortDefinition = null;

            var queryable = Enumerable.Empty<T>().AsQueryable();

            var orderedQueryable = orderBy(queryable);

            foreach (var expression in ((MethodCallExpression)orderedQueryable.Expression).Arguments)
            {
                if (!(expression is UnaryExpression unaryExpression)) 
                    continue;

                var lambda = (LambdaExpression)unaryExpression.Operand;

                if (!(lambda.Body is MemberExpression memberExpression)) 
                    continue;

                var propertyName = memberExpression.Member.Name;

                if (unaryExpression.NodeType == ExpressionType.Quote &&
                    orderedQueryable.Expression.ToString().Contains("OrderByDescending"))
                {
                    sortDefinition = sortDefinition == null
                        ? Builders<T>.Sort.Descending(propertyName)
                        : sortDefinition.Descending(propertyName);
                }
                else
                {
                    sortDefinition = sortDefinition == null
                        ? Builders<T>.Sort.Ascending(propertyName)
                        : sortDefinition.Ascending(propertyName);
                }
            }

            return sortDefinition;
        }

        //public static SortDefinition<T> ToSortDefinition<T>(this Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
        //{
        //    var queryable = Enumerable.Empty<T>().AsQueryable();
        //    var orderedQuery = orderBy(queryable);

        //    SortDefinition<T> sortDefinition = null;

        //    foreach (var expression in ((MethodCallExpression)orderedQuery.Expression).Arguments)
        //    {
        //        var lambda = (LambdaExpression)((UnaryExpression)expression).Operand;
        //        var member = (MemberExpression)lambda.Body;
        //        var propertyName = member.Member.Name;

        //        if (expression.ToString().Contains("OrderByDescending"))
        //        {
        //            sortDefinition = Builders<T>.Sort.Descending(propertyName);
        //        }
        //        else
        //        {
        //            sortDefinition = Builders<T>.Sort.Ascending(propertyName);
        //        }
        //    }

        //    return sortDefinition;
        //}

        public static UpdateDefinition<TEntity> GetUpdateDefinitionByComparingTwoEntities<TEntity, TId>(TEntity newEntity, TEntity currentEntity, IEnumerable<PropertyInfo> properties) where TEntity : IBaseEntity<TId> where TId : IEquatable<TId>
        {
            var propertiesWithValueAssigned = GetPropertiesWithValueAssigned<TEntity, TId>(newEntity, currentEntity, properties);

            var updateDefinitions = propertiesWithValueAssigned
                .Select(property => Builders<TEntity>.Update.Set(property.Key, property.Value))
                .Append(Builders<TEntity>.Update.Set(nameof(BaseEntity.UpdatedOn), DateTime.UtcNow));

            return Builders<TEntity>.Update.Combine(updateDefinitions);
        }
    
        public static UpdateDefinition<TEntity> GetUpdateDefinitionByAnonymousType<TEntity, TId>(object anonymousType, HashSet<string> propertyNameSet) where TEntity : IBaseEntity<TId> where TId : IEquatable<TId>
        {
            return Builders<TEntity>.Update.Combine(anonymousType.GetType().GetProperties()
                .Where(p => propertyNameSet.Contains(p.Name) && !IsPropertySkipped(p)).Select(p =>
                    Builders<TEntity>.Update.Set(p.Name, p.GetValue(anonymousType)))
                .Append(Builders<TEntity>.Update.Set(nameof(BaseEntity.UpdatedOn), DateTime.UtcNow)));
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

        public static async Task<IPaginated<TEntity>> ToPaginatedAsync<TEntity>(this IFindFluent<TEntity,TEntity> fluentQuery, int totalItems = 0, int pageIndex = 0, int itemsPerPage = 0, int rangeOfPages = 0,
            CancellationToken cancellationToken = default)
        {
            List<TEntity> entities = default;

            if (totalItems > 0)
            {
                if (itemsPerPage > 0 && pageIndex > 0)
                    fluentQuery = fluentQuery.Skip((pageIndex - 1) * itemsPerPage).Limit(itemsPerPage);

                entities = await fluentQuery.ToListAsync(cancellationToken);
            }

            return new Paginated<TEntity>(pageIndex, itemsPerPage, totalItems, rangeOfPages, entities);
        }

        public static async Task<ILongPaginated<TEntity>> ToLongPaginatedAsync<TEntity>(this IFindFluent<TEntity, TEntity> fluentQuery, long totalItems = 0, long pageIndex = 0, long itemsPerPage = 0, long rangeOfPages = 0,
            CancellationToken cancellationToken = default)
        {
            List<TEntity> entities = default;

            if (totalItems > 0)
            {
                if (itemsPerPage > 0 && pageIndex > 0)
                    fluentQuery = fluentQuery.LongSkip((pageIndex - 1) * itemsPerPage).LongTake(itemsPerPage);

                entities = await fluentQuery.ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            return new LongPaginated<TEntity>(pageIndex, itemsPerPage, totalItems, rangeOfPages, entities);
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

        public static IFindFluent<T, T> LongSkip<T>(this IFindFluent<T, T> fluentQuery, long count) => LongSkip(fluentQuery, int.MaxValue, count);

        internal static IFindFluent<T, T> LongSkip<T>(IFindFluent<T, T> fluentQuery, int maxSize, long count)
        {
            var segmentCount = Math.DivRem(count, maxSize, out var remainder);

            for (var i = 0L; i < segmentCount; i++)
                fluentQuery = fluentQuery.Skip(maxSize);

            if (remainder != 0)
                fluentQuery = fluentQuery.Skip((int)remainder);

            return fluentQuery;
        }

        public static IFindFluent<T, T> LongTake<T>(this IFindFluent<T, T> fluentQuery, long count) => LongTake(fluentQuery, int.MaxValue, count);
        internal static IFindFluent<T, T> LongTake<T>(IFindFluent<T, T> fluentQuery, int maxSize, long count)
        {
            var segmentCount = Math.DivRem(count, maxSize, out var remainder);

            for (var i = 0L; i < segmentCount; i++)
                fluentQuery = fluentQuery.Limit(maxSize);

            if (remainder != 0)
                fluentQuery = fluentQuery.Limit((int)remainder);

            return fluentQuery;
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

        private static object GetValueFromExpression(Expression exp)
        {
            while (true)
            {
                switch (exp)
                {
                    case ConstantExpression expression:
                        return expression.Value;
                    case MemberExpression expression:
                    {
                        var property = (PropertyInfo)expression.Member;
                        var container = GetValueFromExpression(expression.Expression);
                        return property.GetValue(container);
                    }
                    case NewExpression expression:
                    {
                        var arguments = expression.Arguments.Select(GetValueFromExpression).ToArray();
                        return Activator.CreateInstance(expression.Type, arguments);
                    }
                    case UnaryExpression expression:
                    {
                        exp = expression.Operand;
                        continue;
                    }
                    default:
                        throw new NotSupportedException("Expression must be a constant, member, new or unary expression.");
                }
            }
        }

        private static IDictionary<string, object> GetPropertyNameToValueDictionary<T>(Expression<Func<T, T>> exp)
        {
            if (!(exp.Body is MemberInitExpression body))
                throw new ArgumentException("Expression must be a member initialization expression");

            var assignedProperties = new Dictionary<string, object>();

            foreach (var binding in body.Bindings)
            {
                if (binding is MemberAssignment memberAssignment)
                    assignedProperties[memberAssignment.Member.Name] = GetValueFromExpression(memberAssignment.Expression);
            }

            return assignedProperties;
        }

        private static IDictionary<string, object> GetPropertiesWithValueAssigned<TEntity, TId>(TEntity newEntity, TEntity currentEntity, IEnumerable<PropertyInfo> properties) where TEntity : IBaseEntity<TId> where TId : IEquatable<TId>
        {
            var propertiesWithValueAssigned = new Dictionary<string, object>();

            foreach (var propertyInfo in properties)
            {
                var newValue = propertyInfo.GetValue(newEntity);

                if (!IsPropertySkipped(propertyInfo) && !ArePropertyValuesEqual(newValue, propertyInfo.GetValue(currentEntity)))
                {
                    propertiesWithValueAssigned[propertyInfo.Name] = propertyInfo.GetValue(newEntity);
                }
            }

            return propertiesWithValueAssigned;
        }

        private static bool IsPropertySkipped(PropertyInfo propertyInfo)
        {
            return propertyInfo.Name == nameof(BaseEntity.Id) || propertyInfo.Name == nameof(BaseEntity.CreatedOn) || propertyInfo.Name == nameof(BaseEntity.UpdatedOn);
        }

        private static bool ArePropertyValuesEqual(object newValue, object oldValue)
        {
            if (newValue == null && oldValue == null)
            {
                return true;
            }

            if (newValue == null || oldValue == null)
            {
                return false;
            }

            return newValue.Equals(oldValue);
        }
    }
}