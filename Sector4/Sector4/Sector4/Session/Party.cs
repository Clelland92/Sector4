

#region Using Statements
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Sector4Data;
#endregion

namespace Sector4
{
    
    public class Party
    {
        #region Party Members


        /// <summary>
        /// list of players in the party.
        /// </summary>
        
        private List<Player> players = new List<Player>();

        
        [ContentSerializerIgnore]
        public List<Player> Players
        {
            get { return players; }
            set { players = value; }
        }

        

        
        public void JoinParty(Player player)
        {
            // check the parameter
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            if (players.Contains(player))
            {
                throw new ArgumentException("The player was already in the party.");
            }

            // add the new player to the list
            players.Add(player);

            // add the initial money
            partyMoney += player.Money;
            // only as NPCs are players allowed to have their own money
            player.Money = 0;

            // add the player's inventory items
            foreach (ContentEntry<Gear> contentEntry in player.Inventory)
            {
                AddToInventory(contentEntry.Content, contentEntry.Count);
            }
            // only as NPCs are players allowed to have their own non-equipped gear
            player.Inventory.Clear();
        }


        /// <summary>
        /// Gives the experience amount specified to all party members.
        /// </summary>
        public void GiveExperience(int experience)
        {
            // check the parameters
            if (experience < 0)
            {
                throw new ArgumentOutOfRangeException("experience");
            }
            else if (experience == 0)
            {
                return;
            }

            List<Player> leveledUpPlayers = null;
            foreach (Player player in players)
            {
                int oldLevel = player.CharacterLevel;
                player.Experience += experience;
                if (player.CharacterLevel > oldLevel)
                {
                    if (leveledUpPlayers == null)
                    {
                        leveledUpPlayers = new List<Player>();
                    }
                    leveledUpPlayers.Add(player);
                }
            }

            if ((leveledUpPlayers != null) && (leveledUpPlayers.Count > 0))
            {
                Session.ScreenManager.AddScreen(new LevelUpScreen(leveledUpPlayers));
            }
        }


        #endregion


        #region Inventory


        /// <summary>
        /// The items held by the party.
        /// </summary>
        private List<ContentEntry<Gear>> inventory = new List<ContentEntry<Gear>>();

        /// <summary>
        /// The items held by the party.
        /// </summary>
        [ContentSerializerIgnore]
        public ReadOnlyCollection<ContentEntry<Gear>> Inventory
        {
            get { return inventory.AsReadOnly(); }
        }


        /// <summary>
        /// Add the given gear, in the given quantity, to the party's inventory.
        /// </summary>
        public void AddToInventory(Gear gear, int count)
        {
            // check the parameters
            if ((gear == null) || (count <= 0))
            {
                return;
            }

            // search for an existing entry
            ContentEntry<Gear> existingEntry = inventory.Find(
                delegate(ContentEntry<Gear> entry)
                {
                    return (entry.Content == gear);
                });
            // increment the existing entry, if any
            if (existingEntry != null)
            {
                existingEntry.Count += count;
                return;
            }

            // no existing entry - create a new entry
            ContentEntry<Gear> newEntry = new ContentEntry<Gear>();
            newEntry.Content = gear;
            newEntry.Count = count;
            newEntry.ContentName = gear.AssetName;
            if (newEntry.ContentName.StartsWith(@"Gear\"))
            {
                newEntry.ContentName = newEntry.ContentName.Substring(5);
            }
            inventory.Add(newEntry);
        }


        /// <summary>
        /// Remove the given quantity of the given gear from the party's inventory.
        /// </summary>
        /// <returns>True if the quantity specified could be removed.</returns>
        public bool RemoveFromInventory(Gear gear, int count)
        {
            // check the parameters
            if (gear == null)
            {
                throw new ArgumentNullException("gear");
            }
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            // search for an existing entry
            ContentEntry<Gear> existingEntry = inventory.Find(
                delegate(ContentEntry<Gear> entry)
                {
                    return (entry.Content == gear);
                });
            // no existing entry, so this is moot
            if (existingEntry == null)
            {
                return false;
            }

            // decrement the existing entry
            existingEntry.Count -= count;
            bool fullRemoval = (existingEntry.Count >= 0);

            // if the entry is empty, then remove it
            if (existingEntry.Count <= 0)
            {
                inventory.Remove(existingEntry);
            }

            return fullRemoval;
        }


        
        private int partyMoney;

        
        [ContentSerializer(Optional = true)]
        public int PartyMoney
        {
            get { return partyMoney; }
            set { partyMoney = value; }
        }


        #endregion


        #region Mission Data

        
        
        private Dictionary<string, int> monsterKills = new Dictionary<string, int>();

       
        public Dictionary<string, int> MonsterKills
        {
            get { return monsterKills; }
        }


        
        public void AddMonsterKill(Monster monster)
        {
            if (monsterKills.ContainsKey(monster.AssetName))
            {
                monsterKills[monster.AssetName]++;
            }
            else
            {
                monsterKills.Add(monster.AssetName, 1);
            }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Creates a new Party object from the game-start description.
        /// </summary>
        public Party(GameStartDescription gameStartDescription, 
            ContentManager contentManager)
        {
            // check the parameters
            if (gameStartDescription == null)
            {
                throw new ArgumentNullException("gameStartDescription");
            }
            if (contentManager == null)
            {
                throw new ArgumentNullException("contentManager");
            }

            // load the players
            foreach (string contentName in gameStartDescription.PlayerContentNames)
            {
                JoinParty(contentManager.Load<Player>(
                    Path.Combine(@"Characters\Players", contentName)).Clone()
                    as Player);
            }
        }


        


        #endregion
    }
}
