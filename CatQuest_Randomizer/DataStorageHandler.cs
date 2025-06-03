using System;
using System.Collections.Generic;
using CatQuest_Randomizer.Model;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace CatQuest_Randomizer
{
    public class DataStorageHandler
    {
        public List<Item> availableItems;
        public IEnumerable<Location> locations;
        public Sprite apLogoSprite;
        public Sprite apTitleSprite;
        private readonly string itemStoragePath = $"{Environment.CurrentDirectory}\\ArchipelagoRandomizer\\DataStorage\\Items.json";

        public DataStorageHandler()
        {
            availableItems = LoadItems();
            locations = LoadLocations();
            apLogoSprite = LoadAsSprite($"{Environment.CurrentDirectory}\\ArchipelagoRandomizer\\DataStorage\\Sprites\\ap-logo.png");
            apTitleSprite = LoadAsSprite($"{Environment.CurrentDirectory}\\ArchipelagoRandomizer\\DataStorage\\Sprites\\CatQuestArchiLogo.png");
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

        private Sprite LoadAsSprite(string spritePath)
        {
            Randomizer.Logger.LogInfo($"Will load sprite from {spritePath}");

            if (!File.Exists(spritePath))
                throw new FileNotFoundException("Failed to load location data", spritePath);

            byte[] imageData = File.ReadAllBytes(spritePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageData); // Load PNG into texture

            // Create sprite
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }
}
