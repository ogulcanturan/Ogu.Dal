using Microsoft.EntityFrameworkCore;
using Ogu.Dal.Sql.Entities;
using Ogu.Dal.Sql.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ogu.Dal.Sql.UnitOfWork
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        protected readonly DbContext Context;
        protected UnitOfWork(DbContext context) => Context = context;

        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default,
            TrackingActivityEnum trackingActivity = TrackingActivityEnum.Inactive) 
            => Context.SaveChangesAsync(trackingActivity, cancellationToken);

        public virtual Task<int> SaveChangesWithDateAsync(CancellationToken cancellationToken = default,
            TrackingActivityEnum trackingActivity = TrackingActivityEnum.Inactive)
            => Context.SaveChangesWithDateAsync(trackingActivity, cancellationToken);

        ~UnitOfWork() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
                Context?.Dispose();
            _disposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if(Context != null && !_disposed)
                await Context.DisposeAsync().ConfigureAwait(false);
        }
    }
}