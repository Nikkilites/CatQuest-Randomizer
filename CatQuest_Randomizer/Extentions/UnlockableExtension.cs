using CatQuest_Randomizer.Model;

namespace CatQuest_Randomizer.Extentions
{
	public class UnlockableExtensions
	{
		public void AddRoyalArt(Item item)
		{
			Randomizer.Logger.LogInfo($"Will add Royal Art {item.GetItemValue()} to player");

			Unlockables unlockable = (item.GetItemValue() == "flight" ? Unlockables.Flying : Unlockables.WaterWalking);

			Game.instance.Unlock(unlockable);

			CombatTextSystem.current.ShowText(CombatTextSystem.TextType.DAMAGE, $"{item.Name} obtained from {item.Player}", Game.instance.player.GetPosition(), 1f);
		}
	}
}
