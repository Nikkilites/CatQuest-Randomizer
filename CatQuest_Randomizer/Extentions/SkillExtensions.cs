using BepInEx;
using BepInEx.Logging;
using CatQuest_Randomizer.Model;
using HarmonyLib;
using System.Collections.Generic;

namespace CatQuest_Randomizer.Extentions
{
    public class SkillExtensions
    {
        public void AddSkill(Item item)
        {
			Skill skill = Game.instance.skillManager.GetSkill(item.Id);
			Game.instance.skillManager.Obtain(item.Id);
			skill.level = 1;
			CombatTextSystem.current.ShowText(CombatTextSystem.TextType.DAMAGE, $"{item.Name} obtained", Game.instance.player.GetPosition(), 1f);
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
