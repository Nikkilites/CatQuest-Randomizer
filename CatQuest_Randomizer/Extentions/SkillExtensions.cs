using CatQuest_Randomizer.Model;

namespace CatQuest_Randomizer.Extentions
{
    public class SkillExtensions
    {
        public void AddSkill(Item item)
        {
			string gameSkillId = item.GetItemValue();

			Randomizer.Logger.LogInfo($"Will add Skill {gameSkillId} to player");

			Skill skill = Game.instance.skillManager.GetSkill(gameSkillId);
			Game.instance.skillManager.Obtain(gameSkillId);
			skill.level = 1;
			CombatTextSystem.current.ShowText(CombatTextSystem.TextType.DAMAGE, $"{item.Name} obtained from {item.Player}", Game.instance.player.GetPosition(), 1f);
			if (!Game.instance.gameData.player.skills.equipped.Contains(skill))
			{
				for (int i = 0; i < 4; i++)
				{
					if (Game.instance.gameData.player.skills.equipped[i] == null)
					{
						Game.instance.skillManager.Equip(skill, i);
						break;
					}
				}
			}
		}
    }
}
