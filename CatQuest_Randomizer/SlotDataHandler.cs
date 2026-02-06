using System;
using System.Collections.Generic;


namespace CatQuest_Randomizer
{
    public class SlotDataHandler
    {
        public Goal goal;
        public SkillUpgrade skillUpgrade;
        public bool includeTemples;

        public void FillSlotData(Dictionary<string, object> slotData)
        {
            if (slotData.TryGetValue("goal", out var goalObj))
                goal = (Goal)Convert.ToInt32(goalObj);

            if (slotData.TryGetValue("skill_upgrade", out var skillObj))
                skillUpgrade = (SkillUpgrade)Convert.ToInt32(skillObj);

            if (slotData.TryGetValue("include_temples", out var templeObj))
                includeTemples = Convert.ToBoolean(templeObj);

            Randomizer.Logger.LogInfo($"SlotData was found. goal: {goal} skillUpgrade: {skillUpgrade} includeTemples: {includeTemples}");
        }

        public void AddReceivedSkillUpgrades(string gameSkillId)
        {
            Randomizer.ConnectionHandler.UpdateServerDataStorage(gameSkillId + "_upgrades", GetReceivedSkillUpgrades(gameSkillId)+1);
        }

        public int GetReceivedSkillUpgrades(string gameSkillId)
        {
            return Convert.ToInt32(Randomizer.ConnectionHandler.GetServerDataStorage(gameSkillId + "_upgrades"));
        }
    }

    public enum Goal
    {
        main_quest,
        all_quests,
        spellmeowstery,
    }

    public enum SkillUpgrade
    {
        coins,
        progressive_skills,
        upgrades,
        magic_levels,
    }
}