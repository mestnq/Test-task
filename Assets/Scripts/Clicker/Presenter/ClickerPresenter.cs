using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Clicker.Model;
using Game.Features.Clicker.Application;
using Game.Features.Clicker.Timer;
using Game.Features.Clicker.View;
using UniRx;
using Zenject;

namespace Game.Features.Clicker.Presenter
{
    public class ClickerPresenter : IInitializable, IDisposable
    {
        private readonly IClickerView _view;
        private readonly IClickerModel _model;
        private readonly ManualClickService _manualClickService;
        private readonly AutoClickLoop _autoClickLoop;
        private readonly EnergyRegenLoop _energyRegenLoop;

        private readonly CompositeDisposable _disposables = new();
        private readonly CancellationTokenSource _lifetimeCts = new();

        [Inject]
        public ClickerPresenter(
            IClickerView view,
            IClickerModel model,
            ManualClickService manualClickService,
            AutoClickLoop autoClickLoop,
            EnergyRegenLoop energyRegenLoop)
        {
            _view = view;
            _model = model;
            _manualClickService = manualClickService;
            _autoClickLoop = autoClickLoop;
            _energyRegenLoop = energyRegenLoop;
        }

        public void Initialize()
        {
            BindView();
            BindLoopEvents();
            StartLoops();
        }

        public void Dispose()
        {
            _autoClickLoop.AutoClickPerformed -= HandleAutoClickPerformed;

            _disposables.Dispose();
            _lifetimeCts.Cancel();
            _lifetimeCts.Dispose();
        }

        private void BindView()
        {
            BindCounters();
            BindManualClick();
            BindClickButtonState();
        }

        private void BindCounters()
        {
            _view.BindCurrency(_model.Currency);
            _view.BindEnergy(_model.Energy, _model.MaxEnergy);
        }

        private void BindManualClick()
        {
            _view.ClickRequested
                .Subscribe(_ => HandleManualClick())
                .AddTo(_disposables);
        }

        private void BindClickButtonState()
        {
            _model.Energy
                .Select(_manualClickService.CanClick)
                .DistinctUntilChanged()
                .Subscribe(_view.SetClickButtonInteractable)
                .AddTo(_disposables);
        }

        private void BindLoopEvents()
        {
            _autoClickLoop.AutoClickPerformed += HandleAutoClickPerformed;
        }

        private void HandleManualClick()
        {
            if (!_manualClickService.TryProcessClick())
                return;

            _view.PlayManualClickFeedback();
        }

        private void HandleAutoClickPerformed()
        {
            _view.PlayAutoClickFeedback();
        }

        private void StartLoops()
        {
            _autoClickLoop.RunAsync(_lifetimeCts.Token).Forget();
            _energyRegenLoop.RunAsync(_lifetimeCts.Token).Forget();
        }
    }
}
