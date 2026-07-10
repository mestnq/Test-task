using System;
using System.Threading;
using UniRx;

namespace Game.Features.Clicker.View
{
    public interface IClickerView
    {
        IObservable<Unit> ClickRequested { get; }
        IObservable<Unit> Shown { get; }
        IObservable<Unit> Hidden { get; }

        bool IsVisible { get; }
        CancellationToken ViewLifetimeToken { get; }

        void BindCurrency(IReadOnlyReactiveProperty<int> currency);
        void BindEnergy(IReadOnlyReactiveProperty<int> energy, int maxEnergy);
        void SetClickButtonInteractable(bool isInteractable);
    }
}