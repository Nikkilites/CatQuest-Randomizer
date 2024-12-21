using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatQuest_Randomizer.Model
{
    public class CQItem : Item
    {
		public CQItem(int id, string name, string player, ItemType type)
			: base(id, name, player)
		{
			Type = type;
		}

		public ItemType Type { get; }

		public int GetCollectibleValue()
        {
			return 10;
        }
	}
}
