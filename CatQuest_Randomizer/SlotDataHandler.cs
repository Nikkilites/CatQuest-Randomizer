using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace CatQuest_Randomizer
{
    public class SlotDataHandler
    {
        public Goal goal = 0;
        public SkillUpgrade skillUpgrade = 0;

        public void FillSlotData(Dictionary<string, object> slotData)
        {
            goal = (Goal)Convert.ToInt32(slotData["goal"]);
            skillUpgrade = (SkillUpgrade)Convert.ToInt32(slotData["skill_upgrade"]);
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
        beatgame,
        allquests,
        spellmeowstery,
    }

    public enum SkillUpgrade
    {
        coins,
        skills,
        skillswithupgrades,
        magiclevels,
    }
}