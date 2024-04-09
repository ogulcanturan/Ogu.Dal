using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Ogu.Dal.Abstractions;
using Ogu.Dal.MongoDb.Attributes;
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
        private static readonly HashSet<string> UpdateDisallowedPropertyNames = new HashSet<string>()
        {
            nameof(BaseEntity.Id),
            nameof(BaseEntity.UpdatedOn),
            nameof(BaseEntity.CreatedOn)
        };

        private static UpdateDefinition<T> GetUpdateDefinition<T>() =>
            Builders<T>.Update.Set(nameof(BaseEntity.UpdatedOn), DateTime.UtcNow);

        public static UpdateDefinition<T> ToUpdateDefinition<T>(this Expression<Func<T, T>> exp)
        {
            return Builders<T>.Update.Combine(GetPropertyNameToValueDictionary(exp).Where(kvp => !UpdateDisallowedPropertyNames.Contains(kvp.Key)).Select(kvp =>
                    Builders<T>.Update.Set(kvp.Key, kvp.Value))
                .Append(GetUpdateDefinition<T>()));
        }

        internal static Task CreateIndexesIfIndexAttributeExists<T>(this IMongoCollection<T> table, CancellationToken cancellationToken = default)
        {
            var type = typeof(T);
            var indexAttributes = type.GetCustomAttributes(typeof(MongoIndexAttribute), true)
                .OfType<MongoIndexAttribute>();

            foreach (var attribute in indexAttributes)
            {
                CreateIndexOptions options = null;

                if (attribute.IsUnique)
                {
                    options = new CreateIndexOptions() { Unique = true };
                }

                IndexKeysDefinition<T> indexKeysDefinition = null;

                if (attribute.PropertyNameToIndexTypeDictionary.Count == 1)
                {
                    var firstElement = attribute.PropertyNameToIndexTypeDictionary.First();

                    indexKeysDefinition = firstElement.Value.ToIndexKeysDefinition<T>(firstElement.Key);
                }
                else
                {
                    var indexKeyDefinitions =
                        attribute.PropertyNameToIndexTypeDictionary.Select(p =>
                            p.Value.ToIndexKeysDefinition<T>(p.Key));

                    indexKeysDefinition = Builders<T>.IndexKeys.Combine(indexKeyDefinitions);
                }

                var indexModel = new CreateIndexModel<T>(indexKeysDefinition, options);

                return table.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
            }

            return Task.CompletedTask;
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
                    ((MethodCallExpression)orderedQueryable.Expression).Method.Name == "OrderByDescending")
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

        public static UpdateDefinition<TEntity> GetUpdateDefinitionByAnonymousType<TEntity, TId>(object anonymousType, HashSet<string> propertyNameSet) where TEntity : IBaseEntity<TId>
        {
            return Builders<TEntity>.Update.Combine(anonymousType.GetType().GetProperties()
                .Where(p => propertyNameSet.Contains(p.Name) && !UpdateDisallowedPropertyNames.Contains(p.Name)).Select(p =>
                    Builders<TEntity>.Update.Set(p.Name, p.GetValue(anonymousType)))
                .Append(GetUpdateDefinition<TEntity>()));
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

        public static async Task<IPaginated<TEntity>> ToPaginatedAsync<TEntity>(this IFindFluent<TEntity, TEntity> fluentQuery, int totalItems = 0, int pageIndex = 0, int itemsPerPage = 0, int rangeOfPages = 0,
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

        public static async Task SeedEnumDatabaseAsync(this IMongoClient client, string database, Assembly assembly, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(database))
                throw new ArgumentException("Property name cannot be null or whitespace.", nameof(database));

            var tableTypes = assembly.GetTypes().Where(type => !type.IsAbstract && type.BaseType?.Name == "EnumDatabase`1");

            var getCollectionMethod = typeof(IMongoDatabase).GetMethod("GetCollection");

            var mongoCollectionType = typeof(IMongoCollection<>);

            var mongoCollectionExtensionsMethods = typeof(IMongoCollectionExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public);

            var toListAsyncMethod = typeof(IAsyncCursorSourceExtensions).GetMethod("ToListAsync");

            var findMethod = mongoCollectionExtensionsMethods.FirstOrDefault(m => m.Name == "Find" && m.GetParameters().Any(p => p.ParameterType.Name == "FilterDefinition`1"));

            var db = client.GetDatabase(database);

            var currentTime = DateTime.UtcNow;

            foreach (var tableType in tableTypes)
            {
                var mongoDatabaseAttribute = tableType.GetCustomAttribute<MongoDatabaseAttribute>();
                string tableName = null;
                if (mongoDatabaseAttribute != null)
                {
                    if (!string.IsNullOrWhiteSpace(mongoDatabaseAttribute.Database) &&
                        mongoDatabaseAttribute.Database != database)
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(mongoDatabaseAttribute.Table))
                    {
                        tableName = mongoDatabaseAttribute.Table;
                    }
                }

                if (string.IsNullOrWhiteSpace(tableName))
                {
                    tableName = tableType.Name;
                }

                var enumType = tableType.BaseType.GetGenericArguments()[0]; // CategoryTypeEnum

                var tableConstructor = tableType.GetConstructor(new[] { enumType }); // new CategoryType(categoryTypeEnum)

                var genericMethod = getCollectionMethod.MakeGenericMethod(tableType); // .GetCollection<CategoryType>

                var collection = genericMethod.Invoke(db, new object[] { tableName, null });

                var filterDefinitionType = typeof(FilterDefinition<>).MakeGenericType(tableType);

                var emptyFilter = filterDefinitionType.GetProperty("Empty").GetValue(null);

                var genericFindMethod = findMethod.MakeGenericMethod(tableType);

                var findFluent = genericFindMethod.Invoke(null, new object[] { collection, emptyFilter, null });

                var genericToListAsyncMethod = toListAsyncMethod.MakeGenericMethod(tableType);

                var task = (Task)genericToListAsyncMethod.Invoke(null, new object[] { findFluent, cancellationToken });

                await task.ConfigureAwait(false);

                var enumValuesFromDb = task.GetType().GetProperty("Result").GetValue(task) as IEnumerable<object>;

                var tableTypeBsonIdProperty = tableType.GetProperty("BsonId");
                var tableTypeIdProperty = tableType.GetProperty("Id");
                var tableTypeCodeProperty = tableType.GetProperty("Code");
                var tableTypeDescriptionProperty = tableType.GetProperty("Description");
                var tableTypeIsEnumValueExistsInProgramProperty = tableType.GetProperty("IsEnumValueExistsInProgram");
                var tableTypeCreatedOnProperty = tableType.GetProperty("CreatedOn");
                var tableTypeUpdatedOnProperty = tableType.GetProperty("UpdatedOn");

                var enums = new HashSet<object>(Enum.GetValues(enumType).Cast<object>());

                var enumIdsFromDb = new HashSet<object>();
                var enumClasses = new List<object>();

                foreach (var entity in enumValuesFromDb)
                {
                    var entityId = tableTypeIdProperty.GetValue(entity);

                    enumIdsFromDb.Add(entityId);

                    if (!enums.Contains(entityId))
                    {
                        tableTypeIsEnumValueExistsInProgramProperty.SetValue(entity, false);
                        tableTypeUpdatedOnProperty.SetValue(entity, currentTime);
                        enumClasses.Add(entity);
                        continue;
                    }

                    var newClass = tableConstructor.Invoke(new object[] { entityId });

                    var newClassCodeValue = tableTypeCodeProperty.GetValue(newClass);
                    var newClassDescriptionValue = tableTypeDescriptionProperty.GetValue(newClass);

                    if (tableTypeCodeProperty.GetValue(entity).ToString() != newClassCodeValue.ToString() ||
                        tableTypeDescriptionProperty.GetValue(entity).ToString() != newClassDescriptionValue.ToString() ||
                        (bool)tableTypeIsEnumValueExistsInProgramProperty.GetValue(entity) != true)
                    {
                        tableTypeUpdatedOnProperty.SetValue(entity, currentTime);
                    }

                    tableTypeCodeProperty.SetValue(entity, newClassCodeValue);
                    tableTypeDescriptionProperty.SetValue(entity, newClassDescriptionValue);
                    tableTypeIsEnumValueExistsInProgramProperty.SetValue(entity, true);
                    enumClasses.Add(entity);
                }

                var writeModelType = typeof(WriteModel<>).MakeGenericType(tableType);
                var listType = typeof(List<>).MakeGenericType(writeModelType);
                var writeModelList = Activator.CreateInstance(listType);
                var addMethod = listType.GetMethod("Add");

                var replaceOneModelType = typeof(ReplaceOneModel<>).MakeGenericType(tableType);
                var replaceOneModelConstructor = replaceOneModelType.GetConstructor(new[] { typeof(FilterDefinition<>).MakeGenericType(tableType), tableType });
                var isUpsertProperty = replaceOneModelType.GetProperty("IsUpsert");

                enumClasses.AddRange(enums.Except(enumIdsFromDb).Select(e =>
                {
                    var newRecord = tableConstructor.Invoke(new object[] { e });
                    tableTypeBsonIdProperty.SetValue(newRecord, ObjectId.GenerateNewId().ToString());
                    tableTypeCreatedOnProperty.SetValue(newRecord, currentTime);
                    return newRecord;
                }));

                var buildersType = typeof(Builders<>).MakeGenericType(tableType);

                var filterProperty = buildersType.GetProperty("Filter");

                var filterInstance = filterProperty.GetValue(null);

                var eqMethod = filterInstance.GetType().GetMethods().First(m => m.Name == "Eq" && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() ==
                    typeof(FieldDefinition<,>)).MakeGenericMethod(typeof(string));

                var fieldDef = Activator.CreateInstance(typeof(StringFieldDefinition<,>).MakeGenericType(tableType, typeof(string)), new object[] { "Id", null });

                foreach (var enumClass in enumClasses)
                {
                    var id = tableTypeIdProperty.GetValue(enumClass);

                    var predicate = eqMethod.Invoke(filterInstance, new object[] { fieldDef, ((int)id).ToString() });

                    var replaceOneModel = replaceOneModelConstructor.Invoke(new object[] { predicate, enumClass });
              
                    isUpsertProperty.SetValue(replaceOneModel, true);

                    addMethod.Invoke(writeModelList, new object[] { replaceOneModel });
                }

                var bulkWriteAsyncMethod = mongoCollectionType.MakeGenericType(tableType).GetMethods()
                    .FirstOrDefault(m =>
                        m.Name == "BulkWriteAsync" &&
                        m.GetParameters().Any(p => p.ParameterType.Name == "IEnumerable`1"));

                await ((Task)bulkWriteAsyncMethod.Invoke(collection, new object[] { writeModelList, null, cancellationToken })).ConfigureAwait(false);
            }
        }

        private static IndexKeysDefinition<T> ToIndexKeysDefinition<T>(
            this IndexTypeEnum indexType, string propertyName)
        {
            switch (indexType)
            {
                case IndexTypeEnum.Ascending:
                    return Builders<T>.IndexKeys.Ascending(propertyName);
                case IndexTypeEnum.Descending:
                    return Builders<T>.IndexKeys.Descending(propertyName);
                case IndexTypeEnum.Geo2D:
                    return Builders<T>.IndexKeys.Geo2D(propertyName);
                case IndexTypeEnum.Geo2DSphere:
                    return Builders<T>.IndexKeys.Geo2DSphere(propertyName);
                case IndexTypeEnum.Hashed:
                    return Builders<T>.IndexKeys.Hashed(propertyName);
                case IndexTypeEnum.Text:
                    return Builders<T>.IndexKeys.Text(propertyName);
                case IndexTypeEnum.Wildcard:
                    return Builders<T>.IndexKeys.Wildcard(propertyName);
                default:
                    throw new NotSupportedException();
            }
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

    }
}