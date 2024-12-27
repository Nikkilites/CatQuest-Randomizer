using CatQuest_Randomizer.Model;

namespace CatQuest_Randomizer.Extentions
{
    public class CollectableExtention
    {
        private readonly PlayerData playerData = Game.instance.gameData.player;
        private readonly string format = "+{0}";

        public void AddGold(Item item)
        {
            if (!int.TryParse(item.GetItemValue(), out int itemValue))
            {
                CombatTextSystem.current.ShowText(CombatTextSystem.TextType.GOLD, "Failed to parse coin value", Game.instance.player.GetPosition(), 1f);
                return;
            }
            string text = string.Format(format, itemValue);

            CombatTextSystem.current.ShowText(CombatTextSystem.TextType.GOLD, text, Game.instance.player.GetPosition(), 1f);
            playerData.currency.gold.Value += itemValue;
        }

        public void AddExp(Item item)
        {
            if (!int.TryParse(item.GetItemValue(), out int itemValue))
            {
                CombatTextSystem.current.ShowText(CombatTextSystem.TextType.EXP, "Failed to parse coin value", Game.instance.player.GetPosition(), 1f);
                return;
            }
            string text = string.Format(format, itemValue);

            CombatTextSystem.current.ShowText(CombatTextSystem.TextType.EXP, text, Game.instance.player.GetPosition(), 1f);
            playerData.progression.xp.Value += itemValue;
        }
    }
}
