using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

namespace CatQuest_Randomizer.Extentions
{
    public class SkillExtensions
    {
        public void AddSkill(string id)
        {
			Skill skill = Game.instance.skillManager.GetSkill(id);
			if (!Game.instance.gameData.player.skills.obtained.Contains(skill))
			{
				Game.instance.skillManager.Obtain(id);
				skill.level = 1;
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
}
