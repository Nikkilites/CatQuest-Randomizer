using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

namespace CatQuest_Randomizer.Patches
{
    [BepInPlugin("enable.logger", "EnableLogger", "1.0.0")]
    public class EnableLogger : BaseUnityPlugin
    {
        private static ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;
            Harmony harmony = new Harmony("enable.logger");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(Quest), nameof(Quest.End))]
    public class QuestEndPatch
    {
        private static ManualLogSource Logger;

        static void Prefix()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource("QuestEndPatch");
        }

        static void Postfix(Quest __instance)
        {
            Logger.LogInfo($"Quest {__instance.questId} completed, send check");
            //Send quest id to ConnectionHandler
        }
    }

    [HarmonyPatch(typeof(QuestManager), nameof(QuestManager.LoadData))]
    public class ListAllQuests
    {
        private static ManualLogSource Logger;

        static void Prefix()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource("ListAllQuests");
        }

        static void Postfix(QuestManager __instance)
        {
            var questsField = AccessTools.Field(typeof(QuestManager), "quests");
            if (questsField == null)
            {
                Logger.LogError("Could not find the 'quests' field on QuestManager.");
                return;
            }

            var quests = questsField.GetValue(__instance) as Dictionary<string, Quest>;

            if (quests != null)
            {
                foreach (KeyValuePair<string, Quest> keyValuePair in quests)
                {
                    var quest = keyValuePair.Value;
                    string questId = quest.questId;
                    LocalizedString title = quest.title;

                    // Attempt to retrieve prereq.level, fallback to "N/A" if not found
                    string prereqLevel = quest.prereq?.level.ToString() ?? "None";

                    // Attempt to retrieve prereq.quests, fallback to an empty list if not found
                    string prereqQuests = quest.prereq?.quests != null
                        ? string.Join(", ", quest.prereq.quests)
                        : "None";

                    Logger.LogInfo($"Loaded Quest: ID = {questId}, Title = {title}, Prereq Level = {prereqLevel}, Prereq Quests = {prereqQuests}");

                    if (prereqLevel != "None")
                    {
                        keyValuePair.Value.prereq.level = 0;
                    }
                }
            }
            else
            {
                Logger.LogInfo("No quests found to load.");
            }
        }
    }
}
