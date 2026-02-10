using CatQuest_Randomizer.Model;
using System.Linq;
using UniRx;

namespace CatQuest_Randomizer
{
    public class LocationHandler
    {
        public void CheckLocation(string locationId)
        {
            Randomizer.Logger.LogInfo($"Will look for Location with id {locationId} on list");

            Location location = Randomizer.DataStorageHandler.locations.FirstOrDefault(location => location.Id == locationId);

            if (location != null)
            {
                Randomizer.Logger.LogInfo($"Location was found on list of Locations");

                Randomizer.ConnectionHandler.SendLocation(location);
                Randomizer.GoalHandler.CheckIfGoal(locationId);
            }
            else
            {
                Randomizer.Logger.LogError($"Location was not found on list of Locations");
            }
        }
    }
}
