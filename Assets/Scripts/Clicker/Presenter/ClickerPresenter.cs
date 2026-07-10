using System;
using System.Threading;
using Game.Features.Clicker.Application;
using Game.Features.Clicker.Model;
using Game.Features.Clicker.Service;
using Game.Features.Clicker.View;
using UniRx;
using Zenject;

namespace Game.Features.Clicker.Presentation
{
    public class ClickerPresenter : IInitializable, IDisposable
    {
        private readonly CompositeDisposable _rootDisposables = new();
        private CompositeDisposable _uiSessionDisposables;

        #region DI

        private readonly IClickerView _view;
        private readonly IClickerFeedbackView _feedbackView;
        private readonly IClickerModel _model;
        private readonly ManualClickService _manualClickService;
        private readonly ClickerLoopRunner _loopRunner;

        [Inject]
        public ClickerPresenter(
            IClickerView view,
            IClickerFeedbackView feedbackView,
            IClickerModel model,
            ManualClickService manualClickService,
            ClickerLoopRunner loopRunner)
        {
            _view = view;
            _feedbackView = feedbackView;
            _model = model;
            _manualClickService = manualClickService;
            _loopRunner = loopRunner;
        }

        #endregion

        public void Initialize()
        {
            _view.Shown.Subscribe(_ => StartUiSession()).AddTo(_rootDisposables);

            _view.Hidden.Subscribe(_ => StopUiSession()).AddTo(_rootDisposables);

            if (_view.IsVisible)
                StartUiSession();
        }

        public void Dispose()
        {
            StopUiSession();
            _rootDisposables.Dispose();
        }

        private void StartUiSession()
        {
            StopUiSession();

            _uiSessionDisposables = new CompositeDisposable();

            _view.BindCurrency(_model.Currency);
            _view.BindEnergy(_model.Energy, _model.MaxEnergy);

            _model.Energy.Subscribe(_ => UpdateButtonState()).AddTo(_uiSessionDisposables);
            _view.ClickRequested.Subscribe(_ => OnManualClick(_view.ViewLifetimeToken)).AddTo(_uiSessionDisposables);

            _loopRunner.AutoClickPerformed += OnAutoClickPerformed;
            _loopRunner.Start(_view.ViewLifetimeToken);

            UpdateButtonState();
        }

        private void StopUiSession()
        {
            _loopRunner.AutoClickPerformed -= OnAutoClickPerformed;
            _loopRunner.Stop();

            _uiSessionDisposables?.Dispose();
            _uiSessionDisposables = null;
        }

        private void UpdateButtonState()
        {
            _view.SetClickButtonInteractable(_manualClickService.CanClick());
        }

        private void OnManualClick(CancellationToken token)
        {
            if (!_manualClickService.TryClick())
                return;

            _feedbackView.PlayClickFeedback(_manualClickService.ClickCurrencyReward, token);
        }

        private void OnAutoClickPerformed(int reward)
        {
            _feedbackView.PlayClickFeedback(reward, _view.ViewLifetimeToken);
        }
    }
}