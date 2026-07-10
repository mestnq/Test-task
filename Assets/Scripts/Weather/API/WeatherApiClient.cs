using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace WeatherFeature.API
{
    public sealed class WeatherApiClient : IWeatherApiClient
    {
        private const string ForecastUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";

        public async UniTask<WeatherApiResponse> GetForecastAsync(CancellationToken cancellationToken)
        {
            using var request = UnityWebRequest.Get(ForecastUrl);

            request.SetRequestHeader("Accept", "application/geo+json");
            request.SetRequestHeader("User-Agent", "UnityWeatherApp/1.0");

            await request.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Weather request failed: {request.error}");

            var json = request.downloadHandler.text;
            var response = JsonUtility.FromJson<WeatherApiResponse>(json);

            if (response == null)
                Debug.LogError("Weather response is null");

            if (response?.properties?.periods == null || response.properties.periods.Count == 0)
                Debug.LogError("Weather response has no periods");

            return response;
        }
    }
}