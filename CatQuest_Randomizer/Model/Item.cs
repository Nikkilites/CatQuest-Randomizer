using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatQuest_Randomizer.Model
{
    public class Item
    {
        public int Id { get; }
        public string Name { get; }
        public string Player { get; }

        public Item(int id, string name, string player)
        {
            Id = id;
            Name = name;
            Player = player;
        }
	}
}
