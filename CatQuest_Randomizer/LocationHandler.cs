using CatQuest_Randomizer.Model;
using System.Linq;

namespace CatQuest_Randomizer
{
    public class LocationHandler
    {
        public void CheckedQuestLocation(string questId)
        {
            Randomizer.Logger.LogInfo($"Will look for Location with id {questId} on list");

            Location location = Randomizer.DataStorageHandler.locations.FirstOrDefault(location => location.Id == questId);

            if (location != null)
            {
                Randomizer.Logger.LogInfo($"Location was found on list of Locations");

                Randomizer.ConnectionHandler.SendLocation(location);
                Randomizer.GoalHandler.CheckIfGoal(questId);
            }
            else
            {
                Randomizer.Logger.LogError($"Location was not found on list of Locations");
            }
        }
    }
}
