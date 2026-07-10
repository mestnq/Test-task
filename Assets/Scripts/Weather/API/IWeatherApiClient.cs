using Cysharp.Threading.Tasks;
using System.Threading;

namespace WeatherFeature.API
{
    public interface IWeatherApiClient
    {
        UniTask<WeatherApiResponse> GetForecastAsync(CancellationToken cancellationToken);
    }
}