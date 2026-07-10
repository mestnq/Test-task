namespace Dogs.Domain.Models
{
    public sealed class BreedDetails
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }

        public BreedDetails(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
