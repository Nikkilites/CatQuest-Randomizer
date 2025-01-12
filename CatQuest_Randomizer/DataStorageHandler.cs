using System;
using System.Collections.Generic;
using CatQuest_Randomizer.Model;
using Newtonsoft.Json;
using System.IO;

namespace CatQuest_Randomizer
{
    public class DataStorageHandler
    {
        public List<Item> availableItems;
        public IEnumerable<Location> locations;
        private readonly string itemStoragePath = $"{Environment.CurrentDirectory}\\ArchipelagoRandomizer\\DataStorage\\Items.json";

        public DataStorageHandler()
        {
            availableItems = LoadItems();
            locations = LoadLocations();
        }

        private List<Item> LoadItems()
        {
            Randomizer.Logger.LogInfo($"Will load item data from {itemStoragePath}");

            if (!File.Exists(itemStoragePath))
                throw new FileNotFoundException("Failed to load Item data", itemStoragePath);

            string json = File.ReadAllText(itemStoragePath);
            return JsonConvert.DeserializeObject<List<Item>>(json);
        }

        private IEnumerable<Location> LoadLocations()
        {
            string locationsPath = $"{Environment.CurrentDirectory}\\ArchipelagoRandomizer\\DataStorage\\Locations.json";

            Randomizer.Logger.LogInfo($"Will load location data from {locationsPath}");

            if (!File.Exists(locationsPath))
                throw new FileNotFoundException("Failed to load location data", locationsPath);

            string json = File.ReadAllText(locationsPath);
            return JsonConvert.DeserializeObject<List<Location>>(json);
        }
    }
}
