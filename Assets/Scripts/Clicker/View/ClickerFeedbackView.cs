using UnityEngine;

namespace Game.Features.Clicker.View
{
    public sealed class ClickerFeedbackView : MonoBehaviour, IClickerFeedbackView
    {
        // [SerializeField] private ButtonClickAnimationPlayer _buttonAnimation;
        // [SerializeField] private ClickParticlesPlayer _particlesPlayer;
        // [SerializeField] private ClickSoundPlayer _soundPlayer;
        // [SerializeField] private CurrencyFlyAnimationView _currencyFlyAnimation;

        public void PlayClickFeedback(int rewardAmount)
        {
            // _buttonAnimation?.Play();
            // _particlesPlayer?.Play();
            // _soundPlayer?.Play();
            // _currencyFlyAnimation?.Play();
        }
    }
}