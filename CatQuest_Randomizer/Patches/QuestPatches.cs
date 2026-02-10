using HarmonyLib;
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
                Randomizer.LocationHandler.CheckLocation(__instance.questId);
            }
        }
    }

    [HarmonyPatch(typeof(Quest), nameof(Quest.SetIsComplete))]
    public class QuestCheckOnReloadPatch
    {
        static void Postfix(Quest __instance, bool complete)
        {
            if (complete)
            {
                Randomizer.Logger.LogInfo($"Quest {__instance.questId} was completed, send check from QuestCheckOnReloadPatch");
                Randomizer.LocationHandler.CheckLocation(__instance.questId);
            }
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
            //RemoveQuestRewards(__instance);

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
