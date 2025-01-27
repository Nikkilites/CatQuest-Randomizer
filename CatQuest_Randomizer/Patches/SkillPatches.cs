using HarmonyLib;
using System.Collections.Generic;

namespace CatQuest_Randomizer.Patches
{
    [HarmonyPatch(typeof(WorldButton), nameof(WorldButton.ShowButton))]
    public class DisableBuySkillPatch
    {
        static bool Prefix(GameTrigger trigger, ref bool canEnter)
        {
            ArcaneAltarTrigger arcaneAltarTrigger = trigger as ArcaneAltarTrigger;
            if (arcaneAltarTrigger != null)
            {
                Skill skill = Game.instance.skillManager.GetSkill(arcaneAltarTrigger.skillId);

                if (!skill.isLearned) //|| setting is on)
                {
                    canEnter = false;
                    Randomizer.Logger.LogInfo($"Entering purchase button for {arcaneAltarTrigger.skillId} was set to false");
                }
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(GiveBlock), nameof(GiveBlock.Start))]
    public class DisableObtainingSkillPatch
    {
        private static bool obtained;

        static void Prefix(GiveBlock __instance)
        {
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
                Randomizer.Logger.LogInfo($"Obtaining skill {__instance.id} was disabled");
            }
        }
    }


    [HarmonyPatch(typeof(SkillManager), nameof(SkillManager.LoadSkillData))]
    public class ListAllSkillsPatch
    {
        static void Postfix(SkillManager __instance)
        {
            Randomizer.Logger.LogInfo("Will list all Skills");

            var skillsField = AccessTools.Field(typeof(SkillManager), "skillData");

            if (skillsField == null)
            {
                Randomizer.Logger.LogError("Could not find the 'skills' field in SkillManager.");
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

                    Randomizer.Logger.LogInfo($"Loaded Skill: ID = {skillId}, Name = {name}");
                }
            }
            else
            {
                Randomizer.Logger.LogError("No Skills found to load.");
            }
        }
    }
}
