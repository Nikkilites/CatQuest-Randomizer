using UniRx;
using System.Linq;
using System;

namespace CatQuest_Randomizer
{
    public class GoalHandler
    {
        public void CheckIfGoal(string locationId)

        public void CheckIfLocationIsGoal(string locationId)
        {
            if (Randomizer.SlotDataHandler.goal == Goal.main_quest)
            {
                if (locationId == "MainQuest_012")
                {
                    Randomizer.Logger.LogInfo("Player has beaten the main quest and goaled!");
                    OnGoalConditionMet();
                }
            }

            if (Randomizer.SlotDataHandler.goal == Goal.max_level_and_main_quest)
            {
                if (locationId == "MainQuest_012")
                {
                    if (_dataHelper.GetCurrentLevel().Value >= 99)
                    {
                        Randomizer.Logger.LogInfo("Player reached level 99 and completed the main quest and goaled!");
                        OnGoalConditionMet();
                    }
                    else { Randomizer.Logger.LogInfo("Player has completed the main quest, but still needs lvl 99 to goal"); }
                }
            }
        }

        {
            if (locationId == "MainQuest_012")
                OnGoalConditionMet();
        }

        private void OnGoalConditionMet()
        {
            Randomizer.ConnectionHandler.SendGoal();
        }
    }
}
