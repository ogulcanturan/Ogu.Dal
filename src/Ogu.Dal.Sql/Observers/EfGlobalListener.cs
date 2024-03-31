using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;

namespace Ogu.Dal.Sql.Observers
{
    //Reference: https://github.com/dotnet/efcore/issues/14769
    public sealed class EfGlobalListener : IObserver<DiagnosticListener>, IDisposable
    {
        private bool _disposed;
        private readonly SqlWithNoLockObserver _sqlWithNoLockObserver;
        private IDisposable _withNoLockSubscription;
        private IDisposable _efGlobalSubscription;
        public EfGlobalListener(IOptions<EfGlobalOptions> opts)
        {
            if(opts.Value.IsSqlWithNoLockEnabled)
                _sqlWithNoLockObserver = new SqlWithNoLockObserver();
        }

        public void Subscribe() => _efGlobalSubscription = DiagnosticListener.AllListeners.Subscribe(this);

        public void Dispose()
        {
            if (_disposed) 
                return;

            _withNoLockSubscription?.Dispose();
            _efGlobalSubscription?.Dispose();
            _disposed = true;
        }

        public void OnCompleted() { }
        public void OnError(Exception error) { }
        public void OnNext(DiagnosticListener value)
        {
            if (value.Name == DbLoggerCategory.Name && _sqlWithNoLockObserver != null)
                _withNoLockSubscription = value.Subscribe(_sqlWithNoLockObserver);
        }
    }
}