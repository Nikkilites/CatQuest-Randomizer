using CatQuest_Randomizer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CatQuest_Randomizer
{
    public class LocationHandler
    {
        private IEnumerable<Location> locations;

        public LocationHandler()
        {
            locations = LoadLocations();
        }

        private IEnumerable<Location> LoadLocations()
        {
            string locationsPath = @"E:\Nikki\Programmering\Archipelago\Cat Quest\Randomizer\CatQuest_Randomizer\DataStorage\Locations.json";

            Randomizer.Logger.LogInfo($"Will load location data from {locationsPath}");

            if (!File.Exists(locationsPath))
                throw new FileNotFoundException("Failed to load location data", locationsPath);

            string json = File.ReadAllText(locationsPath);
            return JsonConvert.DeserializeObject<List<Location>>(json);
        }

        public void CheckedQuestLocation(string questId)
        {
            Randomizer.Logger.LogInfo($"Will look for Location with id {questId} on list");

            Location location = locations.FirstOrDefault(location => location.Id == questId);

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
