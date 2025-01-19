using Archipelago.MultiClient.Net.Helpers;
using CatQuest_Randomizer.Extentions;
using CatQuest_Randomizer.Model;
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

            SaveDataHandler.ItemIndex += 1;

            Randomizer.Logger.LogInfo($"Processed item with name: {item.Name}");
        }
    }
}
