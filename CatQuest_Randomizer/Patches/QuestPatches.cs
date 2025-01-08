using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

namespace CatQuest_Randomizer.Patches
{
    [HarmonyPatch(typeof(Quest), nameof(Quest.End))]
    public class QuestEndPatch
    {
        static void Postfix(Quest __instance)
        {
            Randomizer.Logger.LogInfo($"Quest {__instance.questId} completed, send check from QuestEndPatch");
            Randomizer.LocationHandler.CheckedQuestLocation(__instance.questId);
        }
    }

    [HarmonyPatch(typeof(Quest), nameof(Quest.Init))]
    public class RemoveQuestRewardsPatch
    { 
        static void Postfix(Quest __instance)
        {
            __instance.reward._gold = 0;
            __instance.reward.exp = 0;

            Randomizer.Logger.LogInfo($"Obtaining Quest Rewards for Quest {__instance.questId} was disabled");
        }
    }

    [HarmonyPatch(typeof(QuestManager), nameof(QuestManager.LoadData))]
    public class ListAllQuestsPatch
    {
        static void Postfix(QuestManager __instance)
        {
            Randomizer.Logger.LogInfo("Will list all Quests.");

            var questsField = AccessTools.Field(typeof(QuestManager), "quests");

            if (questsField == null)
            {
                Randomizer.Logger.LogError("Could not find the 'quests' field in QuestManager.");
                return;
            }

            Dictionary<string, Quest> quests = questsField.GetValue(__instance) as Dictionary<string, Quest>;

            if (quests != null)
            {
                foreach (KeyValuePair<string, Quest> keyValuePair in quests)
                {
                    var quest = keyValuePair.Value;
                    string questId = quest.questId;
                    LocalizedString title = quest.title;

                    string prereqLevel = quest.prereq?.level.ToString() ?? "None";

                    string prereqQuests = quest.prereq?.quests != null
                        ? string.Join(", ", quest.prereq.quests)
                        : "None";

                    Randomizer.Logger.LogInfo($"Loaded Quest: ID = {questId}, Title = {title}, Prereq Level = {prereqLevel}, Prereq Quests = {prereqQuests}");
                }
            }
            else
            {
                Randomizer.Logger.LogError("No quests found to load.");
            }
        }
    }
}
