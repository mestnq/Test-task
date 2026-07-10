using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Dogs.Infrastructure
{
    /// <summary>
    /// Очередь "последний запрос побеждает"
    /// При новом Enqueue:
    /// - текущий запрос отменяется
    /// - предыдущий ожидающий теряет актуальность
    /// - исполняется новый запрос
    /// </summary>
    public sealed class RequestSerialQueue : IDisposable
    {
        private CancellationTokenSource _currentCts;
        private int _version;
        private bool _disposed;

        public void CancelCurrent()
        {
            _currentCts?.Cancel();
            _currentCts?.Dispose();
            _currentCts = null;
        }

        public async UniTask Enqueue(Func<CancellationToken, UniTask> requestFactory)
        {
            ThrowIfDisposed();

            _version++;
            int localVersion = _version;

            CancelCurrent();

            _currentCts = new CancellationTokenSource();
            var ct = _currentCts.Token;

            try
            {
                if (localVersion != _version)
                    return;

                await requestFactory(ct);
            }
            catch (OperationCanceledException)
            {
                
            }
            finally
            {
                if (_currentCts != null && _currentCts.Token == ct)
                {
                    _currentCts.Dispose();
                    _currentCts = null;
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            CancelCurrent();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RequestSerialQueue));
        }
    }
}
