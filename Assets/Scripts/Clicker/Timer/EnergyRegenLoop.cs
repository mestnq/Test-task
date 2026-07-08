using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Clicker.Model;
using ProjectName.Features.Clicker.Config;

namespace Game.Features.Clicker.Application
{
    public class EnergyRegenLoop
    {
        private readonly IClickerModel _model;
        private readonly ClickerBalanceConfig _config;

        public EnergyRegenLoop(IClickerModel model, ClickerBalanceConfig config)
        {
            _model = model;
            _config = config;
        }

        public async UniTask RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(_config.energyRegenIntervalSeconds),
                    cancellationToken: token);

                _model.AddEnergy(_config.energyRegenAmount);
            }
        }
    }
}