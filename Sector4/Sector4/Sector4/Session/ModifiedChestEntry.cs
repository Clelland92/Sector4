

#region Using Statements
using System;
using System.Collections.Generic;
using Sector4Data;
#endregion

namespace Sector4
{
    
    public class ModifiedChestEntry
    {
        /// <summary>
        /// The map and position of the chest.
        /// </summary>
        public WorldEntry<Chest> WorldEntry = new WorldEntry<Chest>();

        /// <summary>
        /// chest contents
        /// </summary>
        public List<ContentEntry<Gear>> ChestEntries = new List<ContentEntry<Gear>>();

        
        public int Money = 0;
    }
}
