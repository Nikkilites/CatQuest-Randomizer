using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace CatQuest_Randomizer
{
    public class CatQuestDataHelper
    {
        private readonly PlayerData playerData = Game.instance.gameData.player;

        public static List<string> GetSkillIds()
        {
            var skillIdsField = AccessTools.Field(typeof(SkillManager), "skillIds");

            if (skillIdsField == null)
            {
                Randomizer.Logger.LogError("Could not find the 'skillIds' field in SkillManager.");
                return null;
            }

            return skillIdsField.GetValue(Game.instance.skillManager) as List<string>;
        }

        public static Dictionary<string, Quest> GetQuests(QuestManager __instance)
        {
            var questsField = AccessTools.Field(typeof(QuestManager), "quests");

            if (questsField == null)
            {
                Randomizer.Logger.LogError("Could not find the 'quests' field in QuestManager.");
                return null;
            }

            Dictionary<string, Quest> quests = questsField.GetValue(__instance) as Dictionary<string, Quest>;

            if (quests != null)
            {
                return quests;
            }
            else
            {
                Randomizer.Logger.LogError("No quests found to load.");
                return null;
            }
        }

        public ReactiveCollection<string> GetCompletedQuests()
        {
            return playerData.meta.questsCompleted;
        }

        public IntReactiveProperty GetCompletedMainQuests()
        {
            return playerData.stats.mainQuestsCompleted;
        }

        public ReactiveCollection<CollectionData> GetCompletedCaves()
        {
            return playerData.meta.cavesCompleted;
        }

        public List<CollectionData> GetCollectedChests()
        {
            return playerData.meta.chestsCollected;
        }

        public ReactiveCollection<CollectionData> GetVisitedMonuments()
        {
            return playerData.meta.monumentsVisited;
        }

        public IntReactiveProperty GetClearedDungeons()
        {
            return playerData.stats.dungeonsCleared;
        }

        public ReactiveCollection<Skill> GetObtainedSkills()
        {
            return playerData.skills.obtained;
        }

        public ReactiveCollection<Skill> GetEquippedSkills()
        {
            return playerData.skills.equipped;
        }

        public int GetCurrentSkillLevel(Skill skill)
        {
            int level = 0;

            foreach (Skill obtainedSkill in playerData.skills.obtained)
            {
                if (skill == obtainedSkill)
                {
                    level = obtainedSkill.level;
                }
            }

            return level;
        }

        public IntReactiveProperty GetCurrentLevel()
        {
            return playerData.progression.level;
        }

        public IntReactiveProperty GetCurrentExp()
        {
            return playerData.progression.xp;
        }

        public IntReactiveProperty GetCurrentGold()
        {
            return playerData.currency.gold;
        }

        public ReactiveProperty<Unlockables> GetCurrentUnlocks()
        {
            return playerData.meta.unlocks;
        }

        public ReactiveCollection<InventoryItemData> GetCurrentInventory()
        {
            return playerData.inventory.items;
        }

        public ReactiveDictionary<int, Equipment> GetCurrentEquipment()
        {
            return playerData.equipment.obtained;
        }
    }
}
