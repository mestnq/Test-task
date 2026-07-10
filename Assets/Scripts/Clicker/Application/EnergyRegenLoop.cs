using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Features.Clicker.Config;
using Game.Features.Clicker.Model;
using Zenject;

namespace Game.Features.Clicker.Application
{
    public sealed class EnergyRegenLoop
    {
        #region DI

        private readonly IClickerModel _model;
        private readonly ClickerBalanceConfig _config;

        [Inject]
        public EnergyRegenLoop(IClickerModel model, ClickerBalanceConfig config)
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
                        TimeSpan.FromSeconds(_config.EnergyRegenIntervalSeconds),
                        cancellationToken: token);

                    _model.AddEnergy(_config.EnergyRegenAmount);
                }
            }
            catch (OperationCanceledException)
            {
                // Нормальная остановка цикла.
            }
        }
    }
}