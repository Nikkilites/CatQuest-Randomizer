﻿using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

namespace CatQuest_Randomizer.Patches
{
    [HarmonyPatch(typeof(GiveBlock), nameof(GiveBlock.Start))]
    public class DisableObtainingSkillPatch
    {
        private static ManualLogSource Logger;
        private static bool obtained;

        static void Prefix(GiveBlock __instance)
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource("DisableObtainingSkillPatch");

            Skill skill = Game.instance.skillManager.GetSkill(__instance.id);
            if (!Game.instance.gameData.player.skills.obtained.Contains(skill))
            {
                obtained = false;
                Game.instance.gameData.player.skills.obtained.Add(skill);
            }
            else
                obtained = true;
        }

        static void Postfix(GiveBlock __instance)
        {
            if (!obtained)
            {
                Skill skill = Game.instance.skillManager.GetSkill(__instance.id);
                Game.instance.gameData.player.skills.obtained.Remove(skill);
                Logger.LogInfo($"Obtaining skill {__instance.id} has been disabled by DisableObtainingSkillPatch");
            }
        }
    }


    [HarmonyPatch(typeof(SkillManager), nameof(SkillManager.LoadSkillData))]
    public class ListAllSkillsPatch
    {
        private static ManualLogSource Logger;

        static void Prefix()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource("ListAllSkills");
        }

        static void Postfix(SkillManager __instance)
        {
            var skillsField = AccessTools.Field(typeof(SkillManager), "skillData");
            if (skillsField == null)
            {
                Logger.LogError("Could not find the 'skills' field in SkillManager.");
                return;
            }

            var skills = skillsField.GetValue(__instance) as Dictionary<string, StaticSkillData_v2>;

            if (skills != null)
            {
                foreach (KeyValuePair<string, StaticSkillData_v2> keyValuePair in skills)
                {
                    var skill = keyValuePair.Value;
                    string skillId = skill.skillId;
                    LocalizedString name = skill.name;

                    Logger.LogInfo($"Loaded Skill: ID = {skillId}, Name = {name}");
                }
            }
            else
            {
                Logger.LogInfo("No Skills found to load.");
            }
        }
    }
}
