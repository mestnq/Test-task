using Cysharp.Threading.Tasks;
using System.Threading;
using WeatherFeature.Domain;

namespace WeatherFeature.Services
{
    public interface IWeatherService
    {
        UniTask<WeatherData> GetCurrentWeatherAsync(CancellationToken cancellationToken);
    }
}