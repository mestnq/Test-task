using System;
using UniRx;
using WeatherFeature.Domain;
using WeatherFeature.Services;
using Zenject;

namespace WeatherFeature.Presentation
{
    public sealed class WeatherTabPresenter : IDisposable
    {
        private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);

        private readonly CompositeDisposable _disposables = new();

        // Уникальный идентификатор владельца запросов этой вкладки
        private readonly string _ownerId = Guid.NewGuid().ToString();

        private IDisposable _pollingDisposable;
        private bool _isVisible;

        #region DI

        private readonly IWeatherTabView _view;
        private readonly IWeatherService _weatherService;
        private readonly IWeatherRequestQueue _requestQueue;

        public WeatherTabPresenter(
            IWeatherTabView view,
            IWeatherService weatherService,
            IWeatherRequestQueue requestQueue)
        {
            _view = view;
            _weatherService = weatherService;
            _requestQueue = requestQueue;
        }

        #endregion

        public void Initialize()
        {
            _view.OnShown
                .Subscribe(_ => OnTabShown())
                .AddTo(_disposables);

            _view.OnHidden
                .Subscribe(_ => OnTabHidden())
                .AddTo(_disposables);
        }

        private void OnTabShown()
        {
            if (_isVisible)
                return;

            _isVisible = true;
            _view.SetLoadingText();

            StartPolling();
        }

        private void OnTabHidden()
        {
            if (!_isVisible)
                return;

            _isVisible = false;

            StopPolling();

            _requestQueue.CancelAllForOwner(_ownerId);
        }

        private void StartPolling()
        {
            StopPolling();

            TryEnqueueWeatherRequest();

            _pollingDisposable = Observable
                .Interval(PollingInterval)
                .Subscribe(_ =>
                {
                    if (_isVisible)
                        TryEnqueueWeatherRequest();
                });
        }

        private void StopPolling()
        {
            _pollingDisposable?.Dispose();
            _pollingDisposable = null;
        }

        private void TryEnqueueWeatherRequest()
        {
            if (!_isVisible)
                return;

            // если у этой вкладки уже есть активный или pending-запрос, новый запрос не ставим
            if (_requestQueue.HasRequestsForOwner(_ownerId))
                return;

            _requestQueue.Enqueue(_ownerId, async cancellationToken =>
            {
                try
                {
                    var weather = await _weatherService.GetCurrentWeatherAsync(cancellationToken);

                    // Пока запрос выполнялся, вкладка могла скрыться
                    if (!_isVisible)
                        return;

                    _view.SetWeatherText(weather.ToDisplayString());
                }
                catch (OperationCanceledException)
                {
                    // Нормальный сценарий: вкладку скрыли, запрос отменился
                    throw;
                }
                catch (Exception)
                {
                    if (_isVisible)
                        _view.SetErrorText("Не удалось загрузить погоду");
                }
            });
        }

        public void Dispose()
        {
            StopPolling();
            _requestQueue.CancelAllForOwner(_ownerId);
            _disposables.Dispose();
        }

        public sealed class Factory : PlaceholderFactory<IWeatherTabView, WeatherTabPresenter>
        {
        }
    }
}