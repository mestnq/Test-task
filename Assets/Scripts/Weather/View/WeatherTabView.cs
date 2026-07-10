using System;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace WeatherFeature.Presentation
{
    public sealed class WeatherTabView : MonoBehaviour, IWeatherTabView
    {
        [SerializeField] private TMP_Text weatherText;

        private readonly Subject<Unit> _shownSubject = new();
        private readonly Subject<Unit> _hiddenSubject = new();

        private WeatherTabPresenter _presenter;

        public IObservable<Unit> OnShown => _shownSubject;
        public IObservable<Unit> OnHidden => _hiddenSubject;

        [Inject]
        public void Construct(WeatherTabPresenter.Factory presenterFactory)
        {
            _presenter = presenterFactory.Create(this);
        }

        private void Awake()
        {
            _presenter.Initialize();
        }

        private void OnEnable()
        {
            _shownSubject.OnNext(Unit.Default);
        }

        private void OnDisable()
        {
            _hiddenSubject.OnNext(Unit.Default);
        }

        private void OnDestroy()
        {
            _presenter.Dispose();

            _shownSubject.OnCompleted();
            _shownSubject.Dispose();

            _hiddenSubject.OnCompleted();
            _hiddenSubject.Dispose();
        }

        public void SetWeatherText(string text)
        {
            if (weatherText)
                weatherText.text = text;
        }

        public void SetLoadingText()
        {
            if (weatherText)
                weatherText.text = "Загрузка...";
        }

        public void SetErrorText(string text)
        {
            if (weatherText)
                weatherText.text = text;
        }
    }
}