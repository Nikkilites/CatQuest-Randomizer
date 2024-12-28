using CatQuest_Randomizer.Extentions;
using CatQuest_Randomizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatQuest_Randomizer.Archipelago
{
    public class ItemHandler
    {
        public void ReceiveItem(string id, string name, string player)
		{
            Item item = new(id, name, player);

            switch (item.GetItemType())
            {
                case ItemType.skill:
                    SkillExtensions skillExtensions = new();
                    skillExtensions.AddSkill(item);
                    break;
                case ItemType.art:
                    UnlockableExtensions unlockableExtensions1 = new();
                    UnlockableExtensions unlockableExtensions = unlockableExtensions1;
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

            string msg = $"You received your {name} from {player}";
            //Write pretty things in the UI here
        }
    }
}
