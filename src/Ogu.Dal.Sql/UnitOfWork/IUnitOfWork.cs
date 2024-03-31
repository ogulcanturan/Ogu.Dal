using Ogu.Dal.Sql.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ogu.Dal.Sql.UnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable, IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default, TrackingActivityEnum trackingActivity = TrackingActivityEnum.Inactive);

        Task<int> SaveChangesWithDateAsync(CancellationToken cancellationToken = default, TrackingActivityEnum trackingActivity = TrackingActivityEnum.Inactive);
    }
}