namespace CatQuest_Randomizer.Model
{
    public class Location
    {
        public string Id { get; }
        public string InGameId { get; }
        public string Name { get; }

        public Location(string id, string inGameId, string name)
        {
            Id = id;
            Name = name;
            InGameId = inGameId;
        }
    }
}
