using UniRx;

namespace Game.Features.Clicker.Model
{
    public interface IClickerModel
    {
        IReadOnlyReactiveProperty<int> Currency { get; }
        IReadOnlyReactiveProperty<int> Energy { get; }

        int MaxEnergy { get; }

        bool TrySpendEnergy(int amount);
        void AddCurrency(int amount);
        void AddEnergy(int amount);
    }
}
