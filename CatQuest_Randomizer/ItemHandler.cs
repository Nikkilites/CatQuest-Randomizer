using Archipelago.MultiClient.Net.Helpers;
using CatQuest_Randomizer.Extentions;
using CatQuest_Randomizer.Model;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace CatQuest_Randomizer
{
    public class ItemHandler
    {
        private static readonly object itemLock = new();
        private readonly Queue<Item> itemQueue = new();

        public bool CanReceiveItem 
        { 
            get
            {
                return (
                    Game.instance.player.canMove 
                    && !Game.instance.IsPaused 
                    && !Game.instance.player.IsDead
                );
            }
        }

        public void OnItemReceived(IReceivedItemsHelper helper)
        {
            Randomizer.Logger.LogInfo($"Received item from server");

            lock (itemLock)
            {
                string itemName = helper.PeekItem().ItemName;
                Item availableItem = Randomizer.DataStorageHandler.availableItems.FirstOrDefault(item => itemName == item.Name);

                if (availableItem != null)
                {
                    if (helper.Index > SaveDataHandler.ItemIndex)
                    {
                        itemQueue.Enqueue(new Item(availableItem.Id, availableItem.Name, helper.PeekItem().Player.Name));

                        Randomizer.Logger.LogInfo($"Enqueued item with name: {itemName}");
                    }
                    else
                    {
                        Randomizer.Logger.LogError($"Item was already received");
                    }
                }
                else
                {
                    Randomizer.Logger.LogError($"Item could not be found on list of items");
                }

                helper.DequeueItem();
            }
        }

        public void OnFrameUpdate()
        {
            lock (itemLock)
            {
                if (itemQueue.Count > 0 && CanReceiveItem)
                    GiveItem(itemQueue.Dequeue());
            }
        }

        private void GiveItem(Item item)
        {
            SkillExtensions skillExtensions = new();

            switch (item.GetItemType())
            {
                case ItemType.skill:
                case ItemType.skillupgrade:
                    skillExtensions.AddOrUpdateSkill(item);
                    break;
                case ItemType.art:
                    UnlockableExtensions.AddRoyalArt(item);
                    break;
                case ItemType.key:
                    UnlockableExtensions.AddKey(item);
                    break;
                default:
                    CollectableExtentions collectableExtentions = new();
                    collectableExtentions.AddCollectable(item);
                    break;
            }

            SaveDataHandler.ItemIndex += 1;

            Randomizer.Logger.LogInfo($"Processed item with name: {item.Name}");

            SaveArchipelago();
        }

        private void SaveArchipelago()
        {
            var saveManager = Game.instance.saveManager;
            var currentSaveObject = AccessTools.Field(typeof(SaveManager), "currentSaveObject").GetValue(saveManager);
            var updatePlayerProgressMethod = AccessTools.Method(typeof(SaveManager), "UpdatePlayerProgress");
            var updateSkillProgressMethod = AccessTools.Method(typeof(SaveManager), "UpdateSkillProgress");
            var updateEquipmentProgressMethod = AccessTools.Method(typeof(SaveManager), "UpdateEquipmentProgress");
            var updateMetaProgressMethod = AccessTools.Method(typeof(SaveManager), "UpdateMetaProgress");
            var flushMethod = AccessTools.Method(typeof(SaveManager), "Flush");

            if (currentSaveObject != null
                & updatePlayerProgressMethod != null
                & updateSkillProgressMethod != null
                & updateEquipmentProgressMethod != null 
                & updateMetaProgressMethod != null 
                & flushMethod != null)
            {
                updatePlayerProgressMethod.Invoke(saveManager, new[] { currentSaveObject });
                updateSkillProgressMethod.Invoke(saveManager, new[] { currentSaveObject });
                updateEquipmentProgressMethod.Invoke(saveManager, new[] { currentSaveObject });
                updateMetaProgressMethod.Invoke(saveManager, new[] { currentSaveObject });
                flushMethod.Invoke(saveManager, null);
                Randomizer.Logger.LogInfo($"Saved the game");
            }
        }
    }
}
