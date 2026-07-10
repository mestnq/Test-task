using System;
using UniRx;

namespace WeatherFeature.Presentation
{
    public interface IWeatherTabView
    {
        IObservable<Unit> OnShown { get; }
        IObservable<Unit> OnHidden { get; }

        void SetWeatherText(string text);
        void SetLoadingText();
        void SetErrorText(string text);
    }
}