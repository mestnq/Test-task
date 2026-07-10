using UnityEngine;

namespace Dogs.Config
{
    [CreateAssetMenu(menuName = "Configs/DogApiConfig")]
    public class DogApiConfig : ScriptableObject
    {
        [field: SerializeField] public string BaseUrl { get; private set; } = "https://dogapi.dog/api/v2";
        [field: SerializeField] public int BreedsPageSize { get; private set; } = 10;
        [field: SerializeField] public int RequestTimeoutSeconds { get; private set; } = 15;
    }
}
