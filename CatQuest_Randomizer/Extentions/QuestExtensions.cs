using HarmonyLib;
using System.Collections.Generic;

namespace CatQuest_Randomizer.Extentions
{
    public static class QuestExtensions
    {
        public static void RemoveQuestRewards()
        {
            SlotDataHandler slotData = Randomizer.SlotDataHandler;

            QuestManager questManager = Game.instance.questManager;

            Randomizer.Logger.LogInfo($"Will remove quest rewards of type gold: {!slotData.includeQuestRewardCoins}, exp: {!slotData.includeQuestRewardExp}");

            var quests = CatQuestDataHelper.GetQuests(questManager);

            foreach (KeyValuePair<string, Quest> keyValuePair in quests)
            {
                var quest = keyValuePair.Value;

                if (!slotData.includeQuestRewardCoins)
                    quest.reward._gold = 0;

                if (!slotData.includeQuestRewardExp)
                    quest.reward.exp = 0;

                Randomizer.Logger.LogInfo($"Obtaining Quest Rewards for Quest {quest.questId} was disabled");
            }
        }
    }
}
