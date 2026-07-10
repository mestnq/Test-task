using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Features.Clicker.Config;
using Game.Features.Clicker.Model;
using Zenject;

namespace Game.Features.Clicker.Application
{
    public class AutoClickLoop
    {
        public event Action<int> AutoClickPerformed;

        #region DI

        private readonly IClickerModel _model;
        private readonly ClickerBalanceConfig _config;

        [Inject]
        public AutoClickLoop(IClickerModel model, ClickerBalanceConfig config)
        {
            _model = model;
            _config = config;
        }

        #endregion

        public async UniTask RunAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await UniTask.Delay(
                        TimeSpan.FromSeconds(_config.AutoRegenCurrencyInIntervalSeconds),
                        cancellationToken: token);

                    if (!_model.TrySpendEnergy(_config.AutoClickEnergyCost))
                        continue;

                    var rewardAmount = _config.AutoClickCurrencyReward;
                    _model.AddCurrency(rewardAmount);
                    AutoClickPerformed?.Invoke(rewardAmount);
                }
            }
            catch (OperationCanceledException)
            {
                // Нормальная остановка цикла.
            }
        }
    }
}