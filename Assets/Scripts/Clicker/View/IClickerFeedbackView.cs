using System.Threading;

namespace Game.Features.Clicker.View
{
    public interface IClickerFeedbackView
    {
        void PlayClickFeedback(int rewardAmount, CancellationToken cancellationToken);
    }
}