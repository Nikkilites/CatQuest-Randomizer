namespace CatQuest_Randomizer
{
    public class GoalHandler
    {
        public void CheckIfGoal(string locationId)
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
