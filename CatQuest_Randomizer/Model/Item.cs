namespace CatQuest_Randomizer.Model
{
    public class Item
    {
        public string Id { get; }
        public string Name { get; }
        public string Player { get; }

        public Item(string id, string name, string player = null)
        {
            Id = id;
            Name = name;
            Player = player;
        }

        public ItemType GetItemType()
        {
            return this.Id.Split('.')[0] switch
            {
                "gold" => ItemType.gold,
                "exp" => ItemType.exp,
                "skill" => ItemType.skill,
                "skillupgrade" => ItemType.skillupgrade,
                "art" => ItemType.art,
                _ => ItemType.key,
            };
        }

        public string GetItemValue()
        {
            return this.Id.Split('.')[1];
        }
    }
}
