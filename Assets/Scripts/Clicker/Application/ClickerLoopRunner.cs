using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Features.Clicker.Application
{
    public sealed class ClickerLoopRunner : IDisposable
    {
        private CancellationTokenSource _sessionCts;
        private bool _isRunning;

        public event Action<int> AutoClickPerformed;

        #region DI

        private readonly AutoClickLoop _autoClickLoop;
        private readonly EnergyRegenLoop _energyRegenLoop;

        public ClickerLoopRunner(
            AutoClickLoop autoClickLoop,
            EnergyRegenLoop energyRegenLoop)
        {
            _autoClickLoop = autoClickLoop;
            _energyRegenLoop = energyRegenLoop;
        }

        #endregion

        public void Start(CancellationToken externalToken)
        {
            if (_isRunning)
                return;

            _sessionCts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            _autoClickLoop.AutoClickPerformed += OnAutoClickPerformed;

            _autoClickLoop.RunAsync(_sessionCts.Token).Forget();
            _energyRegenLoop.RunAsync(_sessionCts.Token).Forget();

            _isRunning = true;
        }

        public void Stop()
        {
            if (!_isRunning)
                return;

            _autoClickLoop.AutoClickPerformed -= OnAutoClickPerformed;

            if (_sessionCts != null)
            {
                if (!_sessionCts.IsCancellationRequested)
                    _sessionCts.Cancel();

                _sessionCts.Dispose();
                _sessionCts = null;
            }

            _isRunning = false;
        }

        public void Dispose()
        {
            Stop();
        }

        private void OnAutoClickPerformed(int rewardAmount)
        {
            AutoClickPerformed?.Invoke(rewardAmount);
        }
    }
}