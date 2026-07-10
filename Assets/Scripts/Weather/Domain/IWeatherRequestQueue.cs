using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace WeatherFeature.Domain
{
    public interface IWeatherRequestQueue
    {
        string Enqueue(string ownerId, Func<CancellationToken, UniTask> requestFunc);

        bool HasRequestsForOwner(string ownerId);

        void CancelAllForOwner(string ownerId);
    }
}