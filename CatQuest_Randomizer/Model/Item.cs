namespace CatQuest_Randomizer.Model
{
    public class Item
    {
        public string Id { get; }
        public string Name { get; }
        public string Player { get; }

        public Item(string id, string name, string player)
        {
            Id = id;
            Name = name;
            Player = player;
        }

        public ItemType GetItemType()
        {
            switch (this.Id.Split('.')[0])
            {
                case "gold":
                    return ItemType.gold;
                case "exp":
                    return ItemType.exp;
                case "skill":
                    return ItemType.skill;
                case "royalArt":
                    return ItemType.royalArt;
                default:
                    return ItemType.key;
            }
        }

        public string GetItemValue()
        {
            return this.Id.Split('.')[1];
        }
    }
}
