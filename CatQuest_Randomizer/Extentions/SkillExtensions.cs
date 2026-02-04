using CatQuest_Randomizer.Model;

namespace CatQuest_Randomizer.Extentions
{
    public class SkillExtensions
    {
        public void AddOrUpdateSkill(Item item)
        {
			string gameSkillId = item.GetItemValue();
            ItemType gameSkillType = item.GetItemType();


            Skill skill = Game.instance.skillManager.GetSkill(gameSkillId);

			if (!skill.isLearned && gameSkillType == ItemType.skill)
            {
                AddSkill(gameSkillId, skill);
            }
            else if (skill.isLearned)
            {
                Randomizer.Logger.LogInfo($"Will level up Skill {gameSkillId}");
                skill.level++;
            }

            if (gameSkillType == ItemType.skillupgrade)
            {
                Randomizer.SlotDataHandler.AddReceivedSkillUpgrades(gameSkillId);
            }

            CombatTextSystem.current.ShowText(CombatTextSystem.TextType.DAMAGE, $"{item.Name} obtained from {item.Player}", Game.instance.player.GetPosition(), 1f);
		}

        private void AddSkill(string gameSkillId, Skill skill)
        {
            Randomizer.Logger.LogInfo($"Will add Skill {gameSkillId} to player");

            Game.instance.skillManager.Obtain(gameSkillId);
            skill.level = 1;

            if (Randomizer.SlotDataHandler.skillUpgrade == SkillUpgrade.skillswithupgrades)
            {
                skill.level += Randomizer.SlotDataHandler.GetReceivedSkillUpgrades(gameSkillId);
            }

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
