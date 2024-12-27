using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatQuest_Randomizer.Patches
{
    [BepInPlugin("enable.logger", "EnableLogger", "1.0.0")]
    public class EnableLogger : BaseUnityPlugin
    {
        private static ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger; // Initialize the Logger
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
            // Initialize Logger in this patch
            Logger = BepInEx.Logging.Logger.CreateLogSource("QuestEndPatch");
        }

        static void Postfix(Quest __instance)
        {
            // Log quest completion message
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
            // Initialize Logger in this patch
            Logger = BepInEx.Logging.Logger.CreateLogSource("ListAllQuests");
        }

        static void Postfix(QuestManager __instance)
        {
            // Use reflection to access the private 'quests' field
            var questsField = AccessTools.Field(typeof(QuestManager), "quests");
            if (questsField == null)
            {
                Logger.LogError("Could not find the 'quests' field on QuestManager.");
                return;
            }

            var quests = questsField.GetValue(__instance) as Dictionary<string, Quest>;

            // Assuming quests are stored in __instance.quests as a dictionary
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

                    // Log everything in one line
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
