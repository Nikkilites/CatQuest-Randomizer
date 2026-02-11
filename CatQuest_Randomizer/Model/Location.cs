namespace CatQuest_Randomizer.Model
{
    public class Location
    {
        public string Id { get; }
        public string Name { get; }
        public string Art {  get; }
        public bool HasFist { get; }

        public Location(string id, string name, string art, bool hasFist)
        {
            Id = id;
            Name = name;
            Art = art;
            HasFist = hasFist;
        }
    }
}
