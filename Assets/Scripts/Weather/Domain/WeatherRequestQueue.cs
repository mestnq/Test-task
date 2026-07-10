using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace WeatherFeature.Domain
{
    public sealed class WeatherRequestQueue : IWeatherRequestQueue, IDisposable
    {
        private sealed class QueueItem
        {
            public string RequestId;
            public string OwnerId;
            public Func<CancellationToken, UniTask> RequestFunc;
            public CancellationTokenSource Cts;
        }

        private readonly Queue<QueueItem> _queue = new();
        private readonly object _lock = new();

        private QueueItem _currentItem;
        private bool _isProcessing;
        private bool _isDisposed;

        public string Enqueue(string ownerId, Func<CancellationToken, UniTask> requestFunc)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(WeatherRequestQueue));

            var item = new QueueItem
            {
                RequestId = Guid.NewGuid().ToString(),
                OwnerId = ownerId,
                RequestFunc = requestFunc,
                Cts = new CancellationTokenSource()
            };

            var shouldStartProcessing = false;

            lock (_lock)
            {
                _queue.Enqueue(item);

                if (!_isProcessing)
                {
                    _isProcessing = true;
                    shouldStartProcessing = true;
                }
            }

            if (shouldStartProcessing)
            {
                ProcessQueueAsync().Forget();
            }

            return item.RequestId;
        }

        public bool HasRequestsForOwner(string ownerId)
        {
            if (_isDisposed)
                return false;

            lock (_lock)
            {
                if (_currentItem != null && _currentItem.OwnerId == ownerId)
                    return true;

                return _queue.Any(item => item.OwnerId == ownerId);
            }
        }

        public void CancelAllForOwner(string ownerId)
        {
            if (_isDisposed)
                return;

            lock (_lock)
            {
                if (_queue.Count > 0)
                {
                    var filtered = _queue.Where(x => x.OwnerId != ownerId).ToList();
                    _queue.Clear();

                    foreach (var item in filtered)
                        _queue.Enqueue(item);
                }

                if (_currentItem != null && _currentItem.OwnerId == ownerId)
                {
                    _currentItem.Cts.Cancel();
                }
            }
        }

        private async UniTaskVoid ProcessQueueAsync()
        {
            try
            {
                while (TryDequeueNext(out var item))
                {
                    try
                    {
                        await item.RequestFunc(item.Cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.Log($"Weather request cancelled. Owner={item.OwnerId}, Request={item.RequestId}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Weather request failed. Owner={item.OwnerId}, Request={item.RequestId}, Error={e}");
                    }
                    finally
                    {
                        item.Cts.Dispose();

                        lock (_lock)
                        {
                            if (_currentItem == item)
                                _currentItem = null;
                        }
                    }
                }
            }
            finally
            {
                lock (_lock)
                {
                    _isProcessing = false;

                    // Если пока завершался processor, в очередь успели положить новый item —
                    // перезапускаем обработку
                    if (_queue.Count > 0 && !_isDisposed)
                    {
                        _isProcessing = true;
                        ProcessQueueAsync().Forget();
                    }
                }
            }
        }

        private bool TryDequeueNext(out QueueItem item)
        {
            lock (_lock)
            {
                if (_isDisposed || _queue.Count == 0)
                {
                    item = null;
                    return false;
                }

                item = _queue.Dequeue();
                _currentItem = item;
                return true;
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            lock (_lock)
            {
                foreach (var item in _queue)
                {
                    item.Cts.Cancel();
                    item.Cts.Dispose();
                }

                _queue.Clear();

                if (_currentItem != null)
                {
                    _currentItem.Cts.Cancel();
                    _currentItem.Cts.Dispose();
                    _currentItem = null;
                }

                _isProcessing = false;
            }
        }
    }
}