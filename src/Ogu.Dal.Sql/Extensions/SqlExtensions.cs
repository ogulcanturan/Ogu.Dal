using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ogu.Dal.Abstractions;
using Ogu.Dal.Sql.Entities;
using Ogu.Dal.Sql.Observers;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static async Task SeedEnumDatabaseAsync(this DbContext context, CancellationToken cancellationToken = default)
        {
            var propertyInfos = context.GetType().GetProperties().Where(x => typeof(DbSet<>).IsTypeOfGeneric(x.PropertyType) && typeof(EnumDatabase<>).IsSubTypeOfRawGeneric(x.PropertyType.GetGenericArguments()[0]));

            var currentTime = DateTime.UtcNow;

            foreach (var item in propertyInfos)
            {
                if (!(item.GetValue(context) is IEnumerable<object> enumValuesFromDb))
                    continue;

                var tableType = item.PropertyType.GetGenericArguments()[0]; //ex: IconType - (DbSet<IconType>)
                var enumType = tableType.BaseType?.GenericTypeArguments[0]; //ex: IconTypeEnum - EnumDatabase<IconTypeEnum>

                if (enumType == null)
                    continue;

                var tableConstructor = tableType.GetConstructor(new [] { enumType });

                if (tableConstructor == null)
                    continue;

                var tableTypeIdProperty = tableType.GetProperty("Id");
                var tableTypeCodeProperty = tableType.GetProperty("Code");
                var tableTypeDescriptionProperty = tableType.GetProperty("Description");
                var tableTypeIsEnumValueExistsInProgramProperty = tableType.GetProperty("IsEnumValueExistsInProgram");
                var tableTypeCreatedOnProperty = tableType.GetProperty("CreatedOn");
                var tableTypeUpdatedOnProperty = tableType.GetProperty("UpdatedOn");

                var enums = new HashSet<object>(Enum.GetValues(enumType).Cast<object>());

                var enumIdsFromDb = new HashSet<object>();

                foreach (var entity in enumValuesFromDb)
                {
                    var entityId = tableTypeIdProperty.GetValue(entity);

                    enumIdsFromDb.Add(entityId);

                    if (!enums.Contains(entityId))
                    {
                        tableTypeIsEnumValueExistsInProgramProperty.SetValue(entity, false);
                        tableTypeUpdatedOnProperty.SetValue(entity, currentTime);
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
                }

                foreach (var newEnum in enums.Except(enumIdsFromDb))
                {
                    var entity = tableConstructor.Invoke(new object[] { newEnum });

                    tableTypeCreatedOnProperty.SetValue(entity, currentTime);

                    context.Add(entity);
                }
            }

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

#if NETSTANDARD2_0
            foreach (var entityEntry in context.ChangeTracker.Entries()
                         .Where(entityEntry => entityEntry.Entity != null))
            {
                entityEntry.State = EntityState.Detached;
            }
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
    }
}