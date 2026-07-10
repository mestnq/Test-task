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
    /// Все локальные async-операции автоматически отменяются, если view отключили.
    /// </summary>
    public sealed class ClickerFeedbackView : MonoBehaviour, IClickerFeedbackView
    {
        private static readonly TimeSpan ButtonPressedDuration = TimeSpan.FromMilliseconds(150);

        [SerializeField] private Button clickButton;
        [SerializeField] private TMP_Text rewardLabel;
        [SerializeField] private ParticleSystem particleSystem;
        [SerializeField] private AudioSource audioSource;

        private CancellationTokenSource _viewLifetimeCts;
        private CancellationTokenSource _feedbackCts;

        private void Awake()
        {
            HideRewardLabel();
        }

        private void OnEnable()
        {
            RecreateViewLifetime();
        }

        private void OnDisable()
        {
            CancelFeedback();
            CancelViewLifetime();
            HideRewardLabel();
        }

        private void OnDestroy()
        {
            CancelFeedback();
            CancelViewLifetime();
        }

        public void PlayClickFeedback(int rewardAmount, CancellationToken externalCancellationToken)
        {
            if (!isActiveAndEnabled)
                return;

            CancelFeedback();

            _feedbackCts = CancellationTokenSource.CreateLinkedTokenSource(_viewLifetimeCts.Token, externalCancellationToken);

            var token = _feedbackCts.Token;

            DisplayReward(rewardAmount);
            PlayBurst();
            PlayClickSound();
            PlayButtonPressAsync(token).Forget();
        }

        private void PlayBurst()
        {
            if (!particleSystem)
                return;

            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleSystem.Play();
        }

        private void PlayClickSound()
        {
            audioSource?.Play();
        }

        private void DisplayReward(int amount)
        {
            if (!rewardLabel)
                return;

            rewardLabel.text = amount.ToString();
            rewardLabel.gameObject.SetActive(true);
        }

        private void HideRewardLabel()
        {
            if (rewardLabel)
                rewardLabel.gameObject.SetActive(false);
        }

        private async UniTaskVoid PlayButtonPressAsync(CancellationToken token)
        {
            if (!clickButton)
                return;

            var eventData = new PointerEventData(EventSystem.current);
            var pressed = false;

            try
            {
                SetButtonPressed(eventData);
                pressed = true;

                await UniTask.Delay(ButtonPressedDuration, cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                // Ожидаемая отмена: UI скрыли или фидбек был перезапущен.
            }
            finally
            {
                if (pressed && clickButton && clickButton.gameObject && clickButton.gameObject.activeInHierarchy)
                {
                    ReleaseButton(eventData);
                }
            }
        }

        private void SetButtonPressed(PointerEventData eventData)
        {
            ExecuteEvents.Execute(
                clickButton.gameObject,
                eventData,
                ExecuteEvents.pointerDownHandler);
        }

        private void ReleaseButton(PointerEventData eventData)
        {
            ExecuteEvents.Execute(
                clickButton.gameObject,
                eventData,
                ExecuteEvents.pointerUpHandler);
        }

        private void RecreateViewLifetime()
        {
            CancelViewLifetime();
            _viewLifetimeCts = new CancellationTokenSource();
        }

        private void CancelViewLifetime()
        {
            if (_viewLifetimeCts == null)
                return;

            if (!_viewLifetimeCts.IsCancellationRequested)
                _viewLifetimeCts.Cancel();

            _viewLifetimeCts.Dispose();
            _viewLifetimeCts = null;
        }

        private void CancelFeedback()
        {
            if (_feedbackCts == null)
                return;

            if (!_feedbackCts.IsCancellationRequested)
                _feedbackCts.Cancel();

            _feedbackCts.Dispose();
            _feedbackCts = null;
        }
    }
}