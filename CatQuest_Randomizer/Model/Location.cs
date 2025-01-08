namespace CatQuest_Randomizer.Model
{
    public class Location
    {
        public string Id { get; }
        public string Name { get; }

        public Location(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
