using DG.Tweening;
using UnityEngine;

namespace Game.Common.Animation
{
    /// <summary>
    /// Класс, который может передвигать объект в заданном направлении с определенной скоростью и его деактивировать
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class UIFadeMoveAnimation : MonoBehaviour
    {
        [Header("Play")]
        [SerializeField] private bool playOnAwake;
        [SerializeField] private bool playOnEnable = true;

        [Header("Move")]
        [SerializeField] private Vector2 direction = Vector2.up; // например (0,1), (1,0), (-1,1)
        [SerializeField] private float speed = 150f;
        [SerializeField] private float duration = 0.6f;

        [Header("Disappear")]
        [SerializeField] private bool fadeOut = true;
        [SerializeField] private float fadeDuration = 0.25f;
        [SerializeField] private bool deactivateOnComplete = true;

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        private Vector2 _startPosition;
        private Tween _moveTween;
        private Tween _fadeTween;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();

            _startPosition = _rectTransform.anchoredPosition;

            if (playOnAwake)
                Play();
        }

        private void OnEnable()
        {
            if (playOnEnable)
                Play();
        }

        private void OnDisable()
        {
            KillTweens();
        }

        private void OnDestroy()
        {
            KillTweens();
        }

        public void Play()
        {
            KillTweens();

            // сброс в стартовое состояние
            _rectTransform.anchoredPosition = _startPosition;

            if (_canvasGroup != null)
                _canvasGroup.alpha = 1f;

            Vector2 normalizedDirection = direction.sqrMagnitude > 0.001f
                ? direction.normalized
                : Vector2.up;

            Vector2 targetPosition = _startPosition + normalizedDirection * speed * duration;

            _moveTween = _rectTransform.DOAnchorPos(targetPosition, duration).SetEase(Ease.OutCubic).OnComplete(OnMoveComplete);
        }

        private void OnMoveComplete()
        {
            if (fadeOut && _canvasGroup != null)
                _fadeTween = _canvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.Linear).OnComplete(DeactivateIfNeeded);
            else
                DeactivateIfNeeded();
        }

        private void DeactivateIfNeeded()
        {
            if (deactivateOnComplete)
                gameObject.SetActive(false);
        }

        private void KillTweens()
        {
            _moveTween?.Kill();
            _fadeTween?.Kill();
        }
    }
}
