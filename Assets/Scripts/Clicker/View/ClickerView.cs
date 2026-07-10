using System;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Features.Clicker.View
{
    public sealed class ClickerView : MonoBehaviour, IClickerView
    {
        [Header("UI")]
        [SerializeField] private Button clickButton;
        [SerializeField] private TMP_Text currencyText;
        [SerializeField] private TMP_Text energyText;

        private readonly Subject<Unit> _clickRequested = new();
        private readonly Subject<Unit> _shown = new();
        private readonly Subject<Unit> _hidden = new();

        private CompositeDisposable _bindingsDisposable;
        private CancellationTokenSource _viewLifetimeCts;

        public IObservable<Unit> ClickRequested => _clickRequested;
        public IObservable<Unit> Shown => _shown;
        public IObservable<Unit> Hidden => _hidden;

        public bool IsVisible => isActiveAndEnabled;
        public CancellationToken ViewLifetimeToken => _viewLifetimeCts?.Token ?? CancellationToken.None;

        private void Awake()
        {
            _bindingsDisposable = new CompositeDisposable();
        }

        private void OnEnable()
        {
            RecreateLifetime();
            _shown.OnNext(Unit.Default);
        }

        private void OnDisable()
        {
            CancelLifetime();
            ClearBindings();
            _hidden.OnNext(Unit.Default);
        }

        private void OnDestroy()
        {
            CancelLifetime();
            ClearBindings();

            _clickRequested.Dispose();
            _shown.Dispose();
            _hidden.Dispose();
        }

        public void OnClickButtonPressed()
        {
            if (!IsVisible)
                return;

            _clickRequested.OnNext(Unit.Default);
        }

        public void BindCurrency(IReadOnlyReactiveProperty<int> currency)
        {
            currency
                .Subscribe(UpdateCurrency)
                .AddTo(_bindingsDisposable);
        }

        public void BindEnergy(IReadOnlyReactiveProperty<int> energy, int maxEnergy)
        {
            energy
                .Subscribe(value => UpdateEnergy(value, maxEnergy))
                .AddTo(_bindingsDisposable);
        }

        public void SetClickButtonInteractable(bool isInteractable)
        {
            if (clickButton is not null)
                clickButton.interactable = isInteractable;
        }

        private void UpdateCurrency(int value)
        {
            if (currencyText is not null)
                currencyText.text = value.ToString();
        }

        private void UpdateEnergy(int value, int maxEnergy)
        {
            if (energyText is not null)
                energyText.text = $"{value}/{maxEnergy}";
        }

        private void RecreateLifetime()
        {
            CancelLifetime();
            _viewLifetimeCts = new CancellationTokenSource();
        }

        private void CancelLifetime()
        {
            if (_viewLifetimeCts == null)
                return;

            if (!_viewLifetimeCts.IsCancellationRequested)
                _viewLifetimeCts.Cancel();

            _viewLifetimeCts.Dispose();
            _viewLifetimeCts = null;
        }

        private void ClearBindings()
        {
            _bindingsDisposable?.Dispose();
            _bindingsDisposable = new CompositeDisposable();
        }
    }
}
