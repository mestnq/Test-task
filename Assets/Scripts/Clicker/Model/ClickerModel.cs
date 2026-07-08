using ProjectName.Features.Clicker.Config;
using UniRx;
using UnityEngine;

namespace Game.Clicker.Model
{
    public class ClickerModel : IClickerModel
    {
        public IReadOnlyReactiveProperty<int> Currency => _currency;
        public IReadOnlyReactiveProperty<int> Energy => _energy;
        public int MaxEnergy => _config.maxEnergy;

        private readonly ReactiveProperty<int> _currency = new();
        private readonly ReactiveProperty<int> _energy = new();
        private readonly ClickerBalanceConfig _config;

        public ClickerModel(ClickerBalanceConfig config)
        {
            _config = config;
            _currency.Value = 0;
            _energy.Value = _config.maxEnergy;
        }

        public bool TrySpendEnergy(int amount)
        {
            if (amount <= 0 || _energy.Value < amount)
                return false;

            _energy.Value -= amount;
            return true;
        }

        public void AddCurrency(int amount)
        {
            if (amount <= 0)
                return;

            _currency.Value += amount;
        }

        public void AddEnergy(int amount)
        {
            if (amount <= 0)
                return;

            _energy.Value = Mathf.Min(_energy.Value + amount, _config.maxEnergy);
        }
    }
}
