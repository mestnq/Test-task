using System;
using UniRx;

namespace Game.Features.Clicker.View
{
    public interface IClickerView
    {
        IObservable<Unit> ClickRequested { get; }

        void BindCurrency(IReadOnlyReactiveProperty<int> currency);
        void BindEnergy(IReadOnlyReactiveProperty<int> energy, int maxEnergy);

        void SetClickButtonInteractable(bool isInteractable);
        void PlayManualClickFeedback();
        void PlayAutoClickFeedback();
    }
}
