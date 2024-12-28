using CatQuest_Randomizer.Model;

namespace CatQuest_Randomizer.Extentions
{
    public class CollectableExtentions
    {
        private readonly PlayerData playerData = Game.instance.gameData.player;
        private readonly string format = "+{0}";

        public void AddCollectable(Item item)
        {
            if (item.GetItemType() == ItemType.gold)
                AddGold(item);
            else
                AddExp(item);
        }

        private void AddGold(Item item)
        {
            if (!int.TryParse(item.GetItemValue(), out int itemValue))
            {
                CombatTextSystem.current.ShowText(CombatTextSystem.TextType.GOLD, "Failed to parse gold value", Game.instance.player.GetPosition(), 1f);
                return;
            }
            string text = string.Format(format, itemValue);

            CombatTextSystem.current.ShowText(CombatTextSystem.TextType.GOLD, text, Game.instance.player.GetPosition(), 1f);
            playerData.currency.gold.Value += itemValue;
        }

        private void AddExp(Item item)
        {
            if (!int.TryParse(item.GetItemValue(), out int itemValue))
            {
                CombatTextSystem.current.ShowText(CombatTextSystem.TextType.EXP, "Failed to parse exp value", Game.instance.player.GetPosition(), 1f);
                return;
            }
            string text = string.Format(format, itemValue);

            CombatTextSystem.current.ShowText(CombatTextSystem.TextType.EXP, text, Game.instance.player.GetPosition(), 1f);
            playerData.progression.xp.Value += itemValue;
        }
    }
}
