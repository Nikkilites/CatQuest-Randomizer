using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CatQuest_Randomizer.Patches
{
    [HarmonyPatch(typeof(Quest), nameof(Quest.End))]
    public class QuestCheckPatch
    {
        static void Postfix(Quest __instance)
        {
            if (__instance.isComplete)
            {
                Randomizer.Logger.LogInfo($"Quest {__instance.questId} completed, send check from QuestCheckPatch");
                Randomizer.LocationHandler.CheckedQuestLocation(__instance.questId);
            }
        }
    }

    [HarmonyPatch(typeof(Quest), nameof(Quest.SetIsComplete))]
    public class QuestCheckOnReloadPatch
    {
        static void Postfix(Quest __instance, bool complete)
        {
            if (complete)
                Randomizer.Logger.LogInfo($"Quest {__instance.questId} was completed, send check from QuestCheckOnReloadPatch");
                Randomizer.LocationHandler.CheckedQuestLocation(__instance.questId);
        }
    }


    [HarmonyPatch(typeof(Quest), nameof(Quest.StartAt))]
    public class QuestUnblockBorderPatch
    {
        static Dictionary<string, Func<int, int>> questLogic = new Dictionary<string, Func<int, int>>
            {
                { "MainQuest_002", (idx) => (idx > 7 && idx <= 23) ? 7 : idx },
                { "MainQuest_003", (idx) => (idx > 17 && idx <= 33) ? 17 : idx },
                { "MainQuest_004", (idx) => (idx > 20 && idx <= 30) ? 20 : idx },
                { "the_whisperer_five", (idx) => (idx > 10 && idx <= 25) ? 10 : idx },
                { "distraction", (idx) => (idx > 16 && idx <= 37) ? 16 : idx },
                { "faded_king_five", (idx) => (idx > 6 && idx <= 75) ? 6 : idx },
            };

        static void Prefix(Quest __instance, ref int index)
        {
            index = GetUnblockedIndex(__instance.questId, index);
        }

        static private int GetUnblockedIndex(string questId, int index)
        {
            int newIndex = questLogic.ContainsKey(questId) ? questLogic[questId](index) : index;

            if (index != newIndex)
            {
                Randomizer.Logger.LogInfo($"Quest {questId} will start at index {newIndex} instead of index {index}");
            }

            return newIndex;
        }
    }


    [HarmonyPatch(typeof(Quest), nameof(Quest.Next))]
    public class QuestLogIndexPatch
    {
        static void Postfix(Quest __instance)
        {
            Randomizer.Logger.LogInfo($"Quest {__instance.questId} Next. Index: {__instance.index}. SubIndex:{__instance.subIndex}");
        }
    }


    [HarmonyPatch(typeof(Quest), nameof(Quest.Init))]
    public class RemoveQuestRewardsAndPrereqsPatch
    {
        static void Postfix(Quest __instance)
        {
            RemoveQuestRewards(__instance);

            switch (__instance.questId)
            {
                case "greatspirit_one":
                    RemovePrerequisites(__instance, new[] { "waters_five" });
                    break;

                case "greatspirit_four":
                    RemovePrerequisites(__instance, new[] { "MainQuest_004" });
                    break;

                case "missing_one":
                    RemovePrerequisites(__instance, new[] { "faded_king_five" });
                    break;

                case "wyvern_attack":
                    RemovePrerequisites(__instance, new[] { "faded_king_five" });
                    break;

                case "the_heirloom":
                    RemovePrerequisites(__instance, new[] { "faded_king_five", "waters_five", "the_whisperer_five", "MainQuest_006" });
                    break;

                case "faded_king_one":
                    RemovePrerequisites(__instance, new[] { "MainQuest_002" });
                    break;

                case "furbidden_mystery":
                    RemovePrerequisites(__instance, new[] { "MainQuest_007" });
                    break;

                case "ultimate_dragonsbane":
                    RemovePrerequisites(__instance, new[] { "MainQuest_010" });
                    break;

                case "the_whisperer_one":
                    RemovePrerequisites(__instance, new[] { "MainQuest_002" });
                    break;

                case "pawtato_one":
                    RemovePrerequisites(__instance, new[] { "MainQuest_007", "waters_five" });
                    break;

                case "waters_one":
                    RemovePrerequisites(__instance, new[] { "MainQuest_005" });
                    break;

                case "advertising_one":
                    RemovePrerequisites(__instance, new[] { "faded_king_five", "waters_five", "magesold_four", "MainQuest_008" });
                    break;

                case "slashy_one":
                    RemovePrerequisites(__instance, new[] { "waters_five" });
                    break;

                case "west_four":
                    RemovePrerequisites(__instance, new[] { "MainQuest_003" });
                    break;

                case "magesold_one":
                    RemovePrerequisites(__instance, new[] { "MainQuest_006" });
                    break;

                default:
                    break;
            }
        }

        static void RemoveQuestRewards(Quest __instance)
        {
            __instance.reward._gold = 0;
            __instance.reward.exp = 0;

            Randomizer.Logger.LogInfo($"Obtaining Quest Rewards for Quest {__instance.questId} was disabled");
        }

        static void RemovePrerequisites(Quest __instance, string[] prereqsToRemove)
        {
            __instance.prereq.quests = __instance.prereq.quests.Where(q => !prereqsToRemove.Contains(q)).ToArray();

            if (__instance.prereq.quests.Length == 0)
            {
                __instance.prereq.quests = null;
            }

            string prereqsRemaining = __instance.prereq?.quests != null
            ? string.Join(", ", __instance.prereq.quests)
            : "None";

            Randomizer.Logger.LogInfo($"Removed prerequisites {string.Join(", ", prereqsToRemove)} from quest {__instance.questId}. Remaining Prereq's {prereqsRemaining}");
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
