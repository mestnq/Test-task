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

        // [SerializeField] private ParticleSystem clickParticles;
        // [SerializeField] private Animator buttonAnimator;
        // [SerializeField] private Animator currencyFlyAnimator;
        // [SerializeField] private AudioSource audioSource;
        // [SerializeField] private AudioClip manualClickSound;
        // [SerializeField] private AudioClip autoClickSound;

        private readonly Subject<Unit> _clickRequested = new();
        private readonly CompositeDisposable _disposables = new();

        private static readonly int ClickTrigger = Animator.StringToHash("Click");
        private static readonly int FlyTrigger = Animator.StringToHash("Fly");

        public IObservable<Unit> ClickRequested => _clickRequested;

        private void OnValidate()
        {
            if (clickButton == null)
                Debug.LogWarning($"{nameof(clickButton)} is not assigned", this);

            if (currencyText == null)
                Debug.LogWarning($"{nameof(currencyText)} is not assigned", this);

            if (energyText == null)
                Debug.LogWarning($"{nameof(energyText)} is not assigned", this);
        }

        private void Awake()
        {
            clickButton.onClick.AddListener(OnClickButtonPressed);
        }

        public void BindCurrency(IReadOnlyReactiveProperty<int> currency)
        {
            currency
                .DistinctUntilChanged()
                .Subscribe(UpdateCurrency)
                .AddTo(_disposables);
        }

        public void BindEnergy(IReadOnlyReactiveProperty<int> energy, int maxEnergy)
        {
            energy
                .DistinctUntilChanged()
                .Subscribe(value => UpdateEnergy(value, maxEnergy))
                .AddTo(_disposables);
        }

        public void SetClickButtonInteractable(bool isInteractable)
        {
            clickButton.interactable = isInteractable;
        }

        public void PlayManualClickFeedback()
        {
            // PlayFeedback(manualClickSound);
        }
        
        public void PlayAutoClickFeedback()
        {
            // PlayFeedback(autoClickSound);
        }

        private void OnClickButtonPressed()
        {
            _clickRequested.OnNext(Unit.Default);
        }

        private void PlayFeedback(AudioClip clip)
        {
            // PlayVisualFeedback();
            // PlaySound(clip);
        }

        // private void PlayVisualFeedback()
        // {
        //     clickParticles?.Play();
        //     buttonAnimator?.SetTrigger(ClickTrigger);
        //     currencyFlyAnimator?.SetTrigger(FlyTrigger);
        // }
        //
        // private void PlaySound(AudioClip clip)
        // {
        //     if (audioSource != null && clip != null)
        //         audioSource.PlayOneShot(clip);
        // }

        private void UpdateCurrency(int value)
        {
            currencyText.text = value.ToString();
        }

        private void UpdateEnergy(int currentEnergy, int maxEnergy)
        {
            var clampedValue = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            energyText.text = $"{clampedValue}/{maxEnergy}";
        }

        private void OnDestroy()
        {
            clickButton.onClick.RemoveListener(OnClickButtonPressed);
            _disposables.Dispose();
            _clickRequested.Dispose();
        }
    }
}
