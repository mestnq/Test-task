using WeatherFeature.API;
using WeatherFeature.Domain;
using WeatherFeature.Presentation;
using WeatherFeature.Services;
using Zenject;

namespace WeatherFeature.Installers
{
    public sealed class WeatherInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IWeatherRequestQueue>().To<WeatherRequestQueue>().AsSingle();

            Container.Bind<IWeatherApiClient>().To<WeatherApiClient>().AsSingle();

            Container.Bind<IWeatherService>().To<WeatherService>().AsSingle();

            Container.BindFactory<IWeatherTabView, WeatherTabPresenter, WeatherTabPresenter.Factory>().AsTransient();
        }
    }
}