using Archipelago.MultiClient.Net.Helpers;
using CatQuest_Randomizer.Extentions;
using CatQuest_Randomizer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CatQuest_Randomizer
{
    public class ItemHandler
    {
        private List<Item> availableItems;
        private List<Item> foundItems;
        //private readonly string DATA_PATH = Path.Combine(Environment.CurrentDirectory, "Modding", "data", "Randomizer");

        public ItemHandler()
        {
            availableItems = LoadItems();
        }

        private List<Item> LoadItems()
        {
            string dataStoragePath = @"E:\Nikki\Programmering\Archipelago\Cat Quest\Randomizer\CatQuest_Randomizer\DataStorage\Items.json";

            Randomizer.Logger.LogInfo($"Will load item data from {dataStoragePath}");

            if (!File.Exists(dataStoragePath))
                throw new FileNotFoundException("Failed to load Item data", dataStoragePath);

            string json = File.ReadAllText(dataStoragePath);
            return JsonConvert.DeserializeObject<List<Item>>(json);
        }

        public void OnItemReceived(ReceivedItemsHelper helper)
		{
            Randomizer.Logger.LogInfo($"Item with name {helper.PeekItem().ItemName} was received");

            Item availableItem = availableItems.FirstOrDefault(item => helper.PeekItem().ItemName == item.Name);

            if (availableItem != null)
            {
                Randomizer.Logger.LogInfo($"Item was found on list of available items");

                Item item = new(availableItem.Id, availableItem.Name, helper.PeekItem().Player.Name);

                GiveItem(item);
            }
            else
            {
                Randomizer.Logger.LogError($"Item could not be found on list of items");
            }

            helper.DequeueItem();
        }

        private void GiveItem(Item item)
        {
            switch (item.GetItemType())
            {
                case ItemType.skill:
                    SkillExtensions skillExtensions = new();
                    skillExtensions.AddSkill(item);
                    break;
                case ItemType.art:
                    UnlockableExtensions unlockableExtensions = new();
                    unlockableExtensions.AddRoyalArt(item);
                    break;
                case ItemType.key:
                    //Not Implemented
                    break;
                default:
                    CollectableExtentions collectableExtentions = new();
                    collectableExtentions.AddCollectable(item);
                    break;
            }

            foundItems.Add(item);
            Randomizer.Logger.LogInfo($"Received {item.Name} from {item.Player}");
        }
    }
}
