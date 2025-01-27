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
            { "MainQuest_002", (idx) => Clamp(idx, 7, 23) },
            { "MainQuest_003", (idx) => Clamp(idx, 17, 33) },
            { "MainQuest_004", (idx) => Clamp(idx, 20, 30) },
            { "MainQuest_005", (idx) => Clamp(idx, 35, 56) },
            { "MainQuest_008", (idx) => Clamp(idx, 10, 30) },
            { "MainQuest_011", (idx) => Clamp(idx, 56, 72) },
            { "MainQuest_012", (idx) => Clamp(idx, 12, 19) },
            { "the_whisperer_five", (idx) => Clamp(idx, 10, 25) },
            { "distraction", (idx) => Clamp(idx, 16, 37) },
            { "faded_king_five", (idx) => Clamp(idx, 6, 75) },
            { "waters_four", (idx) => Clamp(idx, 11, 54) },
            { "sanctuary_four", (idx) => Clamp(idx, 22, 46) },
            { "west_four", (idx) => Clamp(idx, 36, 61) },
            { "darkpast_two", (idx) => Clamp(idx, 21, 27) },
            { "kitmas_two", (idx) => Clamp(idx, 53, 82) },
            { "magesold_two", (idx) => Clamp(idx, 20, 45) },
            { "darkpast_four", (idx) => Clamp(idx, 3, 23) },
            { "magesold_four", (idx) => Clamp(idx, 38, 81) },
            { "greatspirit_four", (idx) => Clamp(idx, 17, 81) },
            { "the_heirloom", (idx) => Clamp(idx, 38, 52) },
            { "kitmas_four", (idx) => Clamp(idx, 10, 39) },
            { "slashy_one", (idx) => Clamp(idx, 84, 155) },
        };

        static Dictionary<string, Func<int, int>> multiQuestLogic = new Dictionary<string, Func<int, int>>
        {
            { "MainQuest_009", (idx) => ClampMulti(idx, 4, 19, 22, 33) },
            { "kitmas_five", (idx) => ClampMulti(idx, 0, 33, 66, 102) },
        };

        static void Prefix(Quest __instance, ref int index)
        {
            index = GetUnblockedIndex(__instance.questId, index);
        }

        static private int GetUnblockedIndex(string questId, int index)
        {
            int newIndex;

            if (multiQuestLogic.ContainsKey(questId))
            {
                newIndex = multiQuestLogic[questId](index);
            }
            else if (questLogic.ContainsKey(questId))
            {
                newIndex = questLogic[questId](index);
            }
            else
            {
                newIndex = index;
            }

            if (index != newIndex)
            {
                Randomizer.Logger.LogInfo($"Quest {questId} will start at index {newIndex} instead of index {index}");
            }

            return newIndex;
        }

        static private int Clamp(int index, int min, int max)
        {
            return (index > min && index <= max) ? min : index;
        }

        static private int ClampMulti(int index, int min1, int max1, int min2, int max2)
        {
            if (index > min1 && index <= max1)
                return min1;

            if (index > min2 && index <= max2)
                return min2;

            return index;
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

            var questPrerequisites = new Dictionary<string, string[]>
            {
                { "greatspirit_one", new[] { "waters_five" } },
                { "greatspirit_four", new[] { "MainQuest_004" } },
                { "missing_one", new[] { "faded_king_five" } },
                { "wyvern_attack", new[] { "faded_king_five" } },
                { "the_heirloom", new[] { "faded_king_five", "waters_five", "the_whisperer_five", "MainQuest_006" } },
                { "faded_king_one", new[] { "MainQuest_002" } },
                { "furbidden_mystery", new[] { "MainQuest_007" } },
                { "ultimate_dragonsbane", new[] { "MainQuest_010" } },
                { "the_whisperer_one", new[] { "MainQuest_002" } },
                { "pawtato_one", new[] { "MainQuest_007", "waters_five" } },
                { "waters_one", new[] { "MainQuest_005" } },
                { "advertising_one", new[] { "faded_king_five", "waters_five", "magesold_four", "MainQuest_008" } },
                { "slashy_one", new[] { "waters_five" } },
                { "west_four", new[] { "MainQuest_003" } },
                { "magesold_one", new[] { "MainQuest_006" } }
            };

            if (questPrerequisites.TryGetValue(__instance.questId, out var prerequisites))
            {
                RemovePrerequisites(__instance, prerequisites);
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
