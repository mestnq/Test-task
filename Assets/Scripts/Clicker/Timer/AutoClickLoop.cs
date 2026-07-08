using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Clicker.Model;
using ProjectName.Features.Clicker.Config;

namespace Game.Features.Clicker.Timer
{
    public class AutoClickLoop
    {
        private readonly IClickerModel _model;
        private readonly ClickerBalanceConfig _config;

        public event Action AutoClickPerformed;

        public AutoClickLoop(IClickerModel model, ClickerBalanceConfig config)
        {
            _model = model;
            _config = config;
        }

        public async UniTask RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(_config.autoRegenCurrencyInIntervalSeconds),
                    cancellationToken: token);

                if (!_model.TrySpendEnergy(_config.autoClickEnergyCost))
                    continue;

                _model.AddCurrency(_config.autoClickCurrencyReward);
                AutoClickPerformed?.Invoke();
            }
        }
    }
}
