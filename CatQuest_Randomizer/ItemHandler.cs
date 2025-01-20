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
                return Game.instance.player.canMove;
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
            UnlockableExtensions unlockableExtensions = new();

            switch (item.GetItemType())
            {
                case ItemType.skill:
                    SkillExtensions skillExtensions = new();
                    skillExtensions.AddSkill(item);
                    break;
                case ItemType.art:
                    unlockableExtensions.AddRoyalArt(item);
                    break;
                case ItemType.key:
                    unlockableExtensions.AddKey(item);
                    break;
                default:
                    CollectableExtentions collectableExtentions = new();
                    collectableExtentions.AddCollectable(item);
                    break;
            }

            SaveDataHandler.ItemIndex += 1;

            Randomizer.Logger.LogInfo($"Processed item with name: {item.Name}");

            var saveManager = Game.instance.saveManager;
            var currentSaveObject = AccessTools.Field(typeof(SaveManager), "currentSaveObject").GetValue(saveManager);
            var updateSaveMethod = AccessTools.Method(typeof(SaveManager), "UpdateSave");
            var flushMethod = AccessTools.Method(typeof(SaveManager), "Flush");

            if (updateSaveMethod != null & flushMethod != null)
            {
                updateSaveMethod.Invoke(saveManager, new[] { currentSaveObject });
                flushMethod.Invoke(saveManager, null);
                Randomizer.Logger.LogInfo($"Saved the game");
            }
        }
    }
}
