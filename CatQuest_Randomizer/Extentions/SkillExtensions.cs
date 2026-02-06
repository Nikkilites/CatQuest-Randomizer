using CatQuest_Randomizer.Model;
using HarmonyLib;
using System.Collections.Generic;

namespace CatQuest_Randomizer.Extentions
{
    public class SkillExtensions
    {
        public static void AddOrUpdateSkill(Item item)
        {
			string gameSkillId = item.GetItemValue();
            ItemType gameSkillType = item.GetItemType();

            if (gameSkillType != ItemType.magiclevel)
            {
                Skill skill = Game.instance.skillManager.GetSkill(gameSkillId);

                if (!skill.isLearned && gameSkillType == ItemType.skill)
                {
                    AddSkill(gameSkillId, skill);
                }
                else if (skill.isLearned)
                {
                    UpgradeSkill(gameSkillId, skill);
                }

                if (gameSkillType == ItemType.skillupgrade)
                {
                    Randomizer.SlotDataHandler.AddReceivedSkillUpgrades(gameSkillId);
                }
            }
            else
            {
                UpgradeSkills();
            }

            CombatTextSystem.current.ShowText(CombatTextSystem.TextType.DAMAGE, $"{item.Name} obtained from {item.Player}", Game.instance.player.GetPosition(), 1f);
		}

        private static void UpgradeSkills()
        {
            var skillIdsField = AccessTools.Field(typeof(SkillManager), "skillIds");

            if (skillIdsField == null)
            {
                Randomizer.Logger.LogError("Could not find the 'skillIds' field in SkillManager.");
                return;
            }

            List<string> skillIds = skillIdsField.GetValue(Game.instance.skillManager) as List<string>;


            foreach (string skillId in skillIds)
            {
                Skill skill = Game.instance.skillManager.GetSkill(skillId);

                if (skill.isLearned)
                {
                    UpgradeSkill(skillId, skill);
                }

                Randomizer.SlotDataHandler.AddReceivedSkillUpgrades(skillId);
            }
        }

        private static void AddSkill(string gameSkillId, Skill skill)
        {
            Randomizer.Logger.LogInfo($"Will add Skill {gameSkillId} to player");

            Game.instance.skillManager.Obtain(gameSkillId);
            skill.level = 1;

            if (Randomizer.SlotDataHandler.skillUpgrade == SkillUpgrade.upgrades || Randomizer.SlotDataHandler.skillUpgrade == SkillUpgrade.magic_levels)
            {
                int upgradesGotten = Randomizer.SlotDataHandler.GetReceivedSkillUpgrades(gameSkillId);

                Randomizer.Logger.LogInfo($"Will add {upgradesGotten} already received upgrades for {gameSkillId} to player");

                for (int i = 0; i < upgradesGotten; i++)
                {
                    UpgradeSkill(gameSkillId, skill);
                }
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

        private static void UpgradeSkill(string gameSkillId, Skill skill)
        {
            Randomizer.Logger.LogInfo($"Will level up skill {gameSkillId}");
            skill.level++;
        }
    }
}
