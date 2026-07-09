using System;
using Game.Features.Clicker.Model;
using Game.Features.Clicker.Application;
using Game.Features.Clicker.Service;
using Game.Features.Clicker.View;
using UniRx;
using Zenject;

namespace Game.Features.Clicker.Presentation
{
    public class ClickerPresenter : IInitializable, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

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
            _view.BindCurrency(_model.Currency);
            _view.BindEnergy(_model.Energy, _model.MaxEnergy);

            _model.Energy
                .Subscribe(_ => UpdateButtonState())
                .AddTo(_disposables);

            _view.ClickRequested
                .Subscribe(_ => OnManualClick())
                .AddTo(_disposables);

            _loopRunner.AutoClickPerformed += OnAutoClickPerformed;

            UpdateButtonState();
        }

        public void Dispose()
        {
            _loopRunner.AutoClickPerformed -= OnAutoClickPerformed;
            _disposables.Dispose();
        }

        private void UpdateButtonState()
        {
            _view.SetClickButtonInteractable(_manualClickService.CanClick());
        }

        private void OnManualClick()
        {
            if (!_manualClickService.TryClick())
                return;

            _feedbackView.PlayClickFeedback(_manualClickService.ClickCurrencyReward);
        }

        private void OnAutoClickPerformed(int reward)
        {
            _feedbackView.PlayClickFeedback(reward);
        }
    }
}