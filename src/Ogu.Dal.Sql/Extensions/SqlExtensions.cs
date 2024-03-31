using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ogu.Dal.Abstractions;
using Ogu.Dal.Sql.Entities;
using Ogu.Dal.Sql.Observers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Ogu.Dal.Sql.Extensions
{
    public static class SqlExtensions
    {
        public const string C_SQLITE_CONNECTION = "Microsoft.EntityFrameworkCore.Sqlite";
        public const string C_SQLSERVER_CONNECTION = "Microsoft.EntityFrameworkCore.SqlServer";
        public const string C_MYSQL_CONNECTION = "Pomelo.EntityFrameworkCore.MySql";
        public const string C_POSTGRESQL_CONNECTION = "Npgsql.EntityFrameworkCore.PostgreSQL";

        public static bool IsSqliteServer(this DbContext context) => context.Database.ProviderName == C_SQLITE_CONNECTION;
        public static bool IsSqlServer(this DbContext context) => context.Database.ProviderName == C_SQLSERVER_CONNECTION;
        public static bool IsPostgreSqlServer(this DbContext context) => context.Database.ProviderName == C_POSTGRESQL_CONNECTION;
        public static bool IsMySqlServer(this DbContext context) => context.Database.ProviderName == C_MYSQL_CONNECTION;

        public static bool IsUnknown(this DatabaseProviderEnum databaseProvider) => databaseProvider == DatabaseProviderEnum.Unknown;
        public static bool IsSqliteServer(this DatabaseProviderEnum databaseProvider) => databaseProvider == DatabaseProviderEnum.Sqlite;
        public static bool IsSqlServer(this DatabaseProviderEnum databaseProvider) => databaseProvider == DatabaseProviderEnum.SqlServer;
        public static bool IsPostgreSqlServer(this DatabaseProviderEnum databaseProvider) => databaseProvider == DatabaseProviderEnum.PostgreSql;
        public static bool IsMySqlServer(this DatabaseProviderEnum databaseProvider) => databaseProvider == DatabaseProviderEnum.MySql;

        public static DatabaseProviderEnum GetDatabaseProvider(this DbContext context)
        {
            switch (context.Database.ProviderName)
            {
                case C_SQLSERVER_CONNECTION:
                    return DatabaseProviderEnum.SqlServer;
                case C_SQLITE_CONNECTION:
                    return DatabaseProviderEnum.Sqlite;
                case C_POSTGRESQL_CONNECTION:
                    return DatabaseProviderEnum.PostgreSql;
                case C_MYSQL_CONNECTION:
                    return DatabaseProviderEnum.MySql;
                default:
                    return DatabaseProviderEnum.Unknown;
            }
        }

        public static string GetUtcDateTimeValue(this DbContext context)
        {
            switch (context.GetDatabaseProvider())
            {
                case DatabaseProviderEnum.Sqlite:
                case DatabaseProviderEnum.PostgreSql:
                    return "CURRENT_TIMESTAMP";
                case DatabaseProviderEnum.SqlServer:
                    return "GETUTCDATE()";
                case DatabaseProviderEnum.MySql:
                    return "UTC_DATE()";
                default:
                    throw new NotSupportedException($"Db type({context.Database.ProviderName}) is not known by Ogu.Common.Dal.Sql");
            }
        }

        public static string GetTableNameOrDefault<TEntity>(this DbContext context) => context.Model.FindEntityType(typeof(TEntity)).GetAnnotations().FirstOrDefault(x => x.Name == "Relational:TableName")?.Value.ToString();

        public static async Task<IPaginated<TEntity>> ToPaginatedAsync<TEntity>(this IQueryable<TEntity> items, int pageIndex = 0, int itemsPerPage = 0, int rangeOfPages = 0, CancellationToken cancellationToken = default)
        {
            int totalItems;
            TEntity[] entities = default;

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                totalItems = await items.CountAsync(cancellationToken).ConfigureAwait(false);
                if (totalItems > 0)
                {
                    if (itemsPerPage > 0 && pageIndex > 0)
                        items = items.Skip((pageIndex - 1) * itemsPerPage).Take(itemsPerPage);

                    entities = await items.ToArrayAsync(cancellationToken).ConfigureAwait(false);
                }
                scope.Complete();
            }

            return new Paginated<TEntity>(pageIndex, itemsPerPage, totalItems, rangeOfPages, entities);
        }

        public static async Task<IPaginated<TEntity>> ToPaginatedAsync<TEntity>(this IQueryable<TEntity> items, CancellationToken cancellationToken = default)
        {
            int totalItems;
            TEntity[] entities = default;

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                totalItems = await items.CountAsync(cancellationToken).ConfigureAwait(false);

                if (totalItems > 0)
                    entities = await items.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                scope.Complete();
            }

            return new Paginated<TEntity>(totalItems, entities);
        }

        public static async Task<ILongPaginated<TEntity>> ToLongPaginatedAsync<TEntity>(this IQueryable<TEntity> items, long pageIndex = 0, long itemsPerPage = 0, long rangeOfPages = 0, CancellationToken cancellationToken = default)
        {
            long totalItems;
            TEntity[] entities = default;

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                totalItems = await items.LongCountAsync(cancellationToken).ConfigureAwait(false);
                if (totalItems > 0)
                {
                    if (itemsPerPage > 0 && pageIndex > 0)
                        items = items.LongSkip((pageIndex - 1) * itemsPerPage).LongTake(itemsPerPage);

                    entities = await items.ToArrayAsync(cancellationToken).ConfigureAwait(false);
                }
                scope.Complete();
            }

            return new LongPaginated<TEntity>(pageIndex, itemsPerPage, totalItems, rangeOfPages, entities);
        }

        public static async Task<ILongPaginated<TEntity>> ToLongPaginatedAsync<TEntity>(this IQueryable<TEntity> items, CancellationToken cancellationToken)
        {
            long totalItems;
            TEntity[] entities = default;

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                totalItems = await items.LongCountAsync(cancellationToken).ConfigureAwait(false);

                if (totalItems > 0)
                    entities = await items.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                scope.Complete();
            }

            return new LongPaginated<TEntity>(totalItems, entities);
        }

        public static Task<TEntity[]> ToArrayWithNoLockSessionAsync<TEntity>(this IQueryable<TEntity> items, CancellationToken cancellationToken = default) => GetUncommittedAsyncScope(() => items.ToArrayAsync(cancellationToken));
        public static Task<List<TEntity>> ToListWithNoLockSessionAsync<TEntity>(this IQueryable<TEntity> items, CancellationToken cancellationToken = default) => GetUncommittedAsyncScope(() => items.ToListAsync(cancellationToken));
        public static Task<TEntity> FirstOrDefaultWithNoLockSessionAsync<TEntity>(this IQueryable<TEntity> items, CancellationToken cancellationToken = default) => GetUncommittedAsyncScope(() => items.FirstOrDefaultAsync(cancellationToken));
        public static Task<int> CountWithNoLockSessionAsync<TEntity>(this IQueryable<TEntity> items, CancellationToken cancellationToken = default) => GetUncommittedAsyncScope(() => items.CountAsync(cancellationToken));
        public static Task<long> LongCountWithNoLockSessionAsync<TEntity>(this IQueryable<TEntity> items, CancellationToken cancellationToken = default) => GetUncommittedAsyncScope(() => items.LongCountAsync(cancellationToken));

        internal static async Task<T> GetUncommittedAsyncScope<T>(Func<Task<T>> func)
        {
            T result;
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await func().ConfigureAwait(false);
                scope.Complete();
            }
            return result;
        }

        public static async Task SeedEnumDatabaseAsync(this DbContext context)
        {
            var propertyInfos = context.GetType().GetProperties().Where(x => typeof(DbSet<>).IsTypeOfGeneric(x.PropertyType) && typeof(EnumDatabase<>).IsSubTypeOfRawGeneric(x.PropertyType.GetGenericArguments()[0]));

            foreach (var item in propertyInfos)
            {
                var enumValues = item.GetValue(context) is IEnumerable<object> enumValuesFromDb ? new HashSet<object>(enumValuesFromDb) : null;

                if (enumValues == null)
                {
                    continue;
                }

                var enumIds = new HashSet<object>(enumValues.Select(x => x.GetType()?.GetProperty("Id")?.GetValue(x)));

                var classType = item.PropertyType.GetGenericArguments()[0]; //ex: IconType - (DbSet<IconType>)
                var enumType = classType.BaseType?.GenericTypeArguments[0]; //ex: IconTypeEnum - (EnumDatabase<IconTypeEnum>)

                if (enumType == null)
                {
                    continue;
                }

                var constructorOfClass = classType.GetConstructor(new Type[] { enumType });
                //If code, description, or IsEnumTypeExistsInProgram value has changed, update enums.
                var enums = new HashSet<object>(Enum.GetValues(enumType).Cast<object>());
                var updateEnums = enums.Where(x => enumIds.Contains(x)).ToArray();
                foreach (var id in updateEnums)
                {
                    var enumEntity = enumValues.FirstOrDefault(x => x.GetType().GetProperty("Id").GetValue(x).Equals(id));
                    var newClass = constructorOfClass?.Invoke(new object[] { id });

                    var defaultCodeValue = classType.GetProperty("Code")?.GetValue(newClass);
                    var defaultDescriptionValue = classType.GetProperty("Description")?.GetValue(newClass);
                    //object defaultIsEnumTypeExistsValue = classType.GetProperty(nameof(EnumDatabase<EmptyEnum>.IsEnumTypeExistsInProgram)).GetValue(newClass);

                    var existedCodeValue = classType.GetProperty("Code")?.GetValue(enumEntity);
                    var existedDescriptionValue = classType.GetProperty("Description")?.GetValue(enumEntity);
                    //object existedIsEnumTypeExistsValue = classType.GetProperty(nameof(EnumDatabase<EmptyEnum>.IsEnumTypeExistsInProgram)).GetValue(enumEntity);

                    if (defaultCodeValue != existedCodeValue || defaultDescriptionValue != existedDescriptionValue)
                    {
                        classType.GetProperty("UpdatedOn")?.SetValue(enumEntity, DateTime.UtcNow);
                    }

                    classType.GetProperty("Code")?.SetValue(enumEntity, defaultCodeValue);

                    classType.GetProperty("Description")?.SetValue(enumEntity, defaultDescriptionValue);
                }

                //If enum type is not exist in the program, update enum property IsEnumTypeExistsInProgram to 'false'
                var notExistEnumTypesInProgram = enumIds.Where(x => !enums.Contains(x));
                foreach (var id in notExistEnumTypesInProgram)
                {
                    var enumEntity = enumValues.FirstOrDefault(x => x.GetType().GetProperty("Id").GetValue(x).Equals(id));
                    classType.GetProperty("IsEnumTypeExistsInProgram")?.SetValue(enumEntity, false);
                    classType.GetProperty("UpdatedOn")?.SetValue(enumEntity, DateTime.UtcNow);
                }

                //Insert new enum values to database
                object[] newEnums = enums.Except(updateEnums).Select(x =>
                {
                    var newEnum = constructorOfClass?.Invoke(new object[] { x });
                    classType.GetProperty("CreatedOn")?.SetValue(newEnum, DateTime.UtcNow);
                    return newEnum;
                }).ToArray();

                if (newEnums.Length > 0)
                    context.AddRange(newEnums);
            }

            await context.SaveChangesAsync();

#if NETSTANDARD2_0
            context.ChangeTracker.Entries().Where(x => x.Entity != null).ToList().ForEach(x => x.State = EntityState.Detached);
#else
            context.ChangeTracker.Clear();
#endif
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string orderBy, bool isAscending = false)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                throw new ArgumentException("Property name cannot be null or whitespace.", nameof(orderBy));

            return isAscending ?
                query.OrderBy(c => EF.Property<object>(c, orderBy)) :
                query.OrderByDescending(c => EF.Property<object>(c, orderBy));
        }

        public static EfGlobalListener UseEfGlobalListener(Action<EfGlobalOptions> opts = null)
        {
            var efGlobalOptions = new EfGlobalOptions();
            opts?.Invoke(efGlobalOptions);
            var efGlobalListener = new EfGlobalListener(Options.Create<EfGlobalOptions>(efGlobalOptions));
            efGlobalListener.Subscribe();
            return efGlobalListener;
        }

        

        private static readonly HashSet<EntityState> TargetStates = new HashSet<EntityState>
        {
            EntityState.Added,
            EntityState.Modified
        };

        internal static Task<int> SaveChangesWithDateAsync(this DbContext context, TrackingActivityEnum trackingActivity, CancellationToken cancellationToken = default)
        {
            var currentUtcTime = DateTime.UtcNow;

            foreach (var entityEntry in context.ChangeTracker.Entries().Where(e => TargetStates.Contains(e.State)))
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property("CreatedOn").CurrentValue = currentUtcTime;
                    entityEntry.Property("UpdatedOn").IsModified = false;
                }
                else
                {
                    entityEntry.Property("UpdatedOn").CurrentValue = currentUtcTime;
                    entityEntry.Property("CreatedOn").IsModified = false;
                }
            }

            return context.SaveChangesAsync(trackingActivity, cancellationToken);
        }

        internal static Task<int> SaveChangesAsync(this DbContext context, TrackingActivityEnum trackingActivity, CancellationToken cancellationToken = default)
        {
            return context.SaveChangesAsync(trackingActivity == TrackingActivityEnum.Inactive, cancellationToken);
        }

        private static bool IsTypeOfGeneric(this Type genericType, Type toCheck)
        {
            if (!toCheck.IsGenericType)
                return false;

            return genericType == toCheck.GetTypeInfo().GetGenericTypeDefinition();
        }

        private static bool IsSubTypeOfRawGeneric(this Type genericType, Type toCheck)
        {
            return genericType.GetTypeInfo().IsInterface ? (toCheck.GetTypeInfo().IsClass && IsInterfaceOfRawGeneric(genericType, toCheck)) : IsSubclassOfRawGeneric(genericType, toCheck);
        }

        private static bool IsInterfaceOfRawGeneric(this Type genericType, Type toCheck)
        {
            return genericType.GetInterfaces().Any(i => (i.GetTypeInfo().IsGenericType ? i.GetGenericTypeDefinition() : i) == genericType);
        }

        private static bool IsSubclassOfRawGeneric(this Type genericType, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.GetTypeInfo().IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (genericType == cur)
                {
                    return true;
                }
                toCheck = toCheck.GetTypeInfo().BaseType;
            }
            return false;
        }
    }
}