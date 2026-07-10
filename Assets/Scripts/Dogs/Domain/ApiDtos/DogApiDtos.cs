using System;
using System.Collections.Generic;

namespace Dogs.Domain.ApiDtos
{
    [Serializable]
    public sealed class DogApiBreedsResponse
    {
        public List<DogApiBreedData> data;
    }

    [Serializable]
    public sealed class DogApiBreedDetailsResponse
    {
        public DogApiBreedData data;
    }

    [Serializable]
    public sealed class DogApiBreedData
    {
        public string id;
        public string type;
        public DogApiBreedAttributes attributes;
    }

    [Serializable]
    public sealed class DogApiBreedAttributes
    {
        public string name;
        public string description;
        public bool hypoallergenic;
    }
}
