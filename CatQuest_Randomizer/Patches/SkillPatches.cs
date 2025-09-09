using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CatQuest_Randomizer.Patches
{
    [HarmonyPatch(typeof(WorldButton), nameof(WorldButton.ShowButton))]
    public class DisableBuySkillButtonPatch
    {
        static bool Prefix(GameTrigger trigger, ref bool canEnter)
        {
            ArcaneAltarTrigger arcaneAltarTrigger = trigger as ArcaneAltarTrigger;
            if (arcaneAltarTrigger != null)
            {
                Skill skill = Game.instance.skillManager.GetSkill(arcaneAltarTrigger.skillId);

                if (!skill.isLearned)
                {
                    canEnter = false;
                    Randomizer.Logger.LogInfo($"Entering purchase button for {arcaneAltarTrigger.skillId} was set to false");
                    //arcaneAltarTrigger.gameObject.SetActive(false);
                    //arcaneAltarTrigger.altar.orb.enabled = false;
                    //arcaneAltarTrigger.altar.orb.color = new Color32(0, 0, 0, 125);
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ArcaneTemple), "Start")]
    public class DisableBuySkillPatch
    {
        static void Postfix(ArcaneTemple __instance)
        {
            Randomizer.Logger.LogInfo("Buying unlearned skills were disabled");

            Dictionary<string, ArcaneAltarTrigger> triggers = AccessTools.Field(typeof(ArcaneTemple), "triggers").GetValue(__instance) as Dictionary<string, ArcaneAltarTrigger>;

            string[] skillids = { "flamepurr", "healing_paw", "lightnyan", "cattrap", "purrserk", "astropaw", "freezepaw" };

            foreach (string skillId in skillids)
            {
                Skill skill = Game.instance.skillManager.GetSkill(skillId);
                if (triggers.TryGetValue(skillId, out ArcaneAltarTrigger arcaneAltarTrigger))
                {
                    if (!skill.isLearned)
                    {
                        arcaneAltarTrigger.gameObject.SetActive(false);
                        arcaneAltarTrigger.altar.orb.enabled = false;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(GiveBlock), nameof(GiveBlock.Start))]
    public class DisableObtainingSkillPatch
    {
        private static FieldInfo questIdField = typeof(GiveBlock).GetField("questId", BindingFlags.NonPublic | BindingFlags.Instance);

        static bool Prefix(GiveBlock __instance)
        {
            string questId = questIdField.GetValue(__instance) as string;
            Randomizer.Logger.LogInfo("Obtaining skill was disabled");

            Game.gameStream.Publish(new QuestEvent
            {
                type = QuestEvent.EventType.QUEST_BLOCK_COMPLETE,
                questId = questId,
                block = __instance
            });

            return false;
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
