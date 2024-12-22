using CatQuest_Randomizer.Model;

namespace CatQuest_Randomizer.Extentions
{
    public class CollectableExtention
    {
        private readonly PlayerData playerData = Game.instance.gameData.player;
        private readonly string format = "+{0}";

        private void AddGold(CQItem item)
        {
            int value = item.GetCollectibleValue();
            string text = string.Format(format, value);

            CombatTextSystem.current.ShowText(CombatTextSystem.TextType.GOLD, text, Game.instance.player.GetPosition(), 1f);
            playerData.currency.gold.Value += value;
        }

        private void AddExp(CQItem item)
        {
            int value = item.GetCollectibleValue();
            string text = string.Format(format, value);

            CombatTextSystem.current.ShowText(CombatTextSystem.TextType.EXP, text, Game.instance.player.GetPosition(), 1f);
            playerData.progression.xp.Value += value;
        }
    }
}
