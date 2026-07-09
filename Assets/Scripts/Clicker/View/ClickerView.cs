using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Features.Clicker.View
{
    public class ClickerView : MonoBehaviour, IClickerView
    {
        [Header("UI")]
        [SerializeField] private Button clickButton;
        [SerializeField] private TMP_Text currencyText;
        [SerializeField] private TMP_Text energyText;

        private readonly Subject<Unit> _clickRequested = new();

        public IObservable<Unit> ClickRequested => _clickRequested;

        public void OnClickButtonPressed()
        {
            _clickRequested.OnNext(Unit.Default);
        }

        public void BindCurrency(IReadOnlyReactiveProperty<int> currency)
        {
            currency
                .Subscribe(UpdateCurrency)
                .AddTo(this);
        }

        public void BindEnergy(IReadOnlyReactiveProperty<int> energy, int maxEnergy)
        {
            energy
                .Subscribe(value => UpdateEnergy(value, maxEnergy))
                .AddTo(this);
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

        private void OnDestroy()
        {
            _clickRequested.Dispose();
        }
    }
}
