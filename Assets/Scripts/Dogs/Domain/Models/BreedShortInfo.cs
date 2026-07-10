namespace Dogs.Domain.Models
{
    public sealed class BreedShortInfo
    {
        public string Id { get; }
        public string Name { get; }

        public BreedShortInfo(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
