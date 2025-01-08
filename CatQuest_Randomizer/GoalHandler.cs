namespace CatQuest_Randomizer
{
    public class GoalHandler
    {
        public void CheckIfGoal(string questId)
        {
            if (questId == "MainQuest_012")
                OnGoalConditionMet();
        }

        private void OnGoalConditionMet()
        {
            Randomizer.ConnectionHandler.SendGoal();
        }
    }
}
