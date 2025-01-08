using CatQuest_Randomizer.Model;

namespace CatQuest_Randomizer.Extentions
{
    public class CollectableExtentions
    {
        private readonly PlayerData playerData = Game.instance.gameData.player;
        private readonly string format = "+{0}";

        public void AddCollectable(Item item)
        {
            Randomizer.Logger.LogInfo($"Will add collectable to player");

            Randomizer.Logger.LogInfo($"Try to parse Collectable Item Value");

            if (!int.TryParse(item.GetItemValue(), out int itemValue))
            {
                Randomizer.Logger.LogError($"Could not parsed to player");

                CombatTextSystem.current.ShowText(CombatTextSystem.TextType.DAMAGE, "Failed to parse gold value", Game.instance.player.GetPosition(), 1f);
                return;
            }
            else
            {
                Randomizer.Logger.LogInfo($"Item Value parsed as {itemValue}");

                if (item.GetItemType() == ItemType.gold)
                    AddGold(itemValue);
                else
                    AddExp(itemValue);
            }
        }

        private void AddGold(int itemValue)
        {
            Randomizer.Logger.LogInfo($"Adding gold to player");

            string text = string.Format(format, itemValue);

            CombatTextSystem.current.ShowText(CombatTextSystem.TextType.GOLD, text, Game.instance.player.GetPosition(), 1f);
            playerData.currency.gold.Value += itemValue;
        }

        private void AddExp(int itemValue)
        {
            Randomizer.Logger.LogInfo($"Adding exp to player");

            string text = string.Format(format, itemValue);

            CombatTextSystem.current.ShowText(CombatTextSystem.TextType.EXP, text, Game.instance.player.GetPosition(), 1f);
            playerData.progression.xp.Value += itemValue;
        }
    }
}
