using Cysharp.Threading.Tasks;
using System.Threading;
using WeatherFeature.API;
using WeatherFeature.Domain;

namespace WeatherFeature.Services
{
    public sealed class WeatherService : IWeatherService
    {
        private readonly IWeatherApiClient _apiClient;

        public WeatherService(IWeatherApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async UniTask<WeatherData> GetCurrentWeatherAsync(CancellationToken cancellationToken)
        {
            var response = await _apiClient.GetForecastAsync(cancellationToken);
            var period = response.properties.periods[0];

            return new WeatherData(
                dayName: "Сегодня",
                temperature: period.temperature,
                temperatureUnit: period.temperatureUnit
            );
        }
    }
}