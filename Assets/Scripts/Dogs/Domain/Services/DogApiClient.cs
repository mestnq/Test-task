using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dogs.Config;
using Dogs.Domain.ApiDtos;
using Dogs.Domain.Models;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Dogs.Domain.Services
{
    public sealed class DogApiClient : IDogApiClient
    {
        private readonly DogApiConfig _config;

        public DogApiClient(DogApiConfig config)
        {
            _config = config;
        }

        public async UniTask<IReadOnlyList<BreedShortInfo>> GetBreedsAsync(int pageSize, CancellationToken cancellationToken)
        {
            string url = $"{_config.BaseUrl}/breeds?page[size]={pageSize}";
            string json = await SendGetRequestAsync(url, cancellationToken);

            var response = JsonConvert.DeserializeObject<DogApiBreedsResponse>(json);
            if (response?.data == null)
                throw new Exception("Breeds response is null or invalid.");

            var result = new List<BreedShortInfo>(response.data.Count);
            foreach (var item in response.data)
            {
                if (item == null)
                    continue;

                string id = item.id;
                string name = item.attributes?.name ?? "Unknown";

                result.Add(new BreedShortInfo(id, name));
            }

            return result;
        }

        public async UniTask<BreedDetails> GetBreedDetailsAsync(string breedId, CancellationToken cancellationToken)
        {
            string url = $"{_config.BaseUrl}/breeds/{breedId}";
            string json = await SendGetRequestAsync(url, cancellationToken);

            var response = JsonConvert.DeserializeObject<DogApiBreedDetailsResponse>(json);
            if (response?.data == null)
                throw new Exception($"Breed details response is null. BreedId={breedId}");

            var data = response.data;
            var attributes = data.attributes;

            string name = attributes?.name ?? "Unknown";
            string description = string.IsNullOrWhiteSpace(attributes?.description)
                ? "Описание отсутствует."
                : attributes.description;

            return new BreedDetails(data.id, name, description);
        }

        private async UniTask<string> SendGetRequestAsync(string url, CancellationToken cancellationToken)
        {
            using var request = UnityWebRequest.Get(url);
            request.timeout = _config.RequestTimeoutSeconds;

            await request.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception($"HTTP request failed. Url={url}. Error={request.error}");

            return request.downloadHandler.text;
        }
    }
}
