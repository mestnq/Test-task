using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Features.Clicker.View
{
    /// <summary>
    /// Отвечает за визуальный фидбек клика:
    /// - имитация нажатия кнопки
    /// - отображение полученной награды
    /// - запуск дополнительных эффектов
    /// </summary>
    public sealed class ClickerFeedbackView : MonoBehaviour, IClickerFeedbackView
    {
        private static readonly TimeSpan ButtonPressedDuration = TimeSpan.FromMilliseconds(150);

        [SerializeField] private Button clickButton;
        [SerializeField] private TMP_Text rewardLabel;
        [SerializeField] private ParticleSystem particleSystem;

        // можно вынести в отдельный класс - контроллер звука
        [SerializeField] private AudioSource audioSource;

        private CancellationToken _destroyCancellationToken;

        private void Awake()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();

            HideRewardLabel();
        }

        public void PlayClickFeedback(int rewardAmount)
        {
            DisplayReward(rewardAmount);
            PlayBurst();
            PlayClickSound();
            PlayButtonPress().Forget();
        }

        private void PlayBurst()
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleSystem.Play();
        }

        private void PlayClickSound() => audioSource?.Play();

        private void DisplayReward(int amount)
        {
            rewardLabel.text = amount.ToString();
            rewardLabel.gameObject.SetActive(true);
        }

        private void HideRewardLabel()
        {
            rewardLabel.gameObject.SetActive(false);
        }

        /// <summary>
        /// Анимация нажатия кнопки
        /// </summary>
        private async UniTaskVoid PlayButtonPress()
        {
            var eventData = new PointerEventData(EventSystem.current);

            SetButtonPressed(eventData);

            await UniTask.Delay(ButtonPressedDuration, cancellationToken: _destroyCancellationToken);

            ReleaseButton(eventData);
        }

        private void SetButtonPressed(PointerEventData eventData)
        {
            ExecuteEvents.Execute(
                clickButton.gameObject,
                eventData,
                ExecuteEvents.pointerDownHandler
            );
        }

        private void ReleaseButton(PointerEventData eventData)
        {
            ExecuteEvents.Execute(
                clickButton.gameObject,
                eventData,
                ExecuteEvents.pointerUpHandler
            );
        }
    }
}