using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Game.Features.Clicker.Application
{
    public class ClickerLoopRunner : IInitializable, IDisposable
    {
        private readonly CancellationTokenSource _cts = new();

        public event Action<int> AutoClickPerformed;

        #region DI

        private readonly AutoClickLoop _autoClickLoop;
        private readonly EnergyRegenLoop _energyRegenLoop;

        [Inject]
        public ClickerLoopRunner(AutoClickLoop autoClickLoop, EnergyRegenLoop energyRegenLoop)
        {
            _autoClickLoop = autoClickLoop;
            _energyRegenLoop = energyRegenLoop;
        }

        #endregion

        public void Initialize()
        {
            _autoClickLoop.AutoClickPerformed += OnAutoClickPerformed;

            _autoClickLoop.RunAsync(_cts.Token).Forget();
            _energyRegenLoop.RunAsync(_cts.Token).Forget();
        }

        public void Dispose()
        {
            _autoClickLoop.AutoClickPerformed -= OnAutoClickPerformed;

            _cts.Cancel();
            _cts.Dispose();
        }

        private void OnAutoClickPerformed(int rewardAmount)
        {
            AutoClickPerformed?.Invoke(rewardAmount);
        }
    }
}