

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Sector4Data;
#endregion

namespace Sector4
{
    class Session
    {
        #region Singleton


        /// <summary>
        /// The single Session instance that can be active at a time.
        /// </summary>
        private static Session singleton;


        #endregion


        #region Party


        /// <summary>
        /// The party that is playing the game right now.
        /// </summary>
        private Party party;

        /// <summary>
        /// The party that is playing the game right now.
        /// </summary>
        public static Party Party
        {
            get { return (singleton == null ? null : singleton.party); }
        }


        #endregion


        #region Map


        /// <summary>
        /// Change the current map, arriving at the given portal if any.
        /// </summary>
        /// <param name="contentName">The asset name of the new map.</param>
        /// <param name="originalPortal">The portal from the previous map.</param>
        public static void ChangeMap(string contentName, Portal originalPortal)
        {
            // make sure the content name is valid
            string mapContentName = contentName;
            if (!mapContentName.StartsWith(@"Maps\"))
            {
                mapContentName = Path.Combine(@"Maps", mapContentName);
            }

            // check for trivial movement - typically intra-map portals
            if ((TileEngine.Map != null) && (TileEngine.Map.AssetName == mapContentName))
            {
                TileEngine.SetMap(TileEngine.Map, originalPortal == null ? null :
                    TileEngine.Map.FindPortal(originalPortal.DestinationMapPortalName));
            }

            // load the map
            ContentManager content = singleton.screenManager.Game.Content;
            Map map = content.Load<Map>(mapContentName).Clone() as Map;

            // modify the map based on the world changes (removed chests, etc.).
            singleton.ModifyMap(map);

            // start playing the music for the new map
            AudioManager.PlayMusic(map.MusicCueName);

            // set the new map into the tile engine
            TileEngine.SetMap(map, originalPortal == null ? null :
                map.FindPortal(originalPortal.DestinationMapPortalName));
        }


        /// <summary>
        /// Perform any actions associated withe the given tile.
        /// </summary>
        /// <param name="mapPosition">The tile to check.</param>
        /// <returns>True if anything was encountered, false otherwise.</returns>
        public static bool EncounterTile(Point mapPosition)
        {
            // look for fixed-combats from the mission
            if ((singleton.mission != null) &&
                ((singleton.mission.Stage == Mission.MissionStage.InProgress) ||
                 (singleton.mission.Stage == Mission.MissionStage.RequirementsMet)))
            {
                MapEntry<FixedCombat> fixedCombatEntry =
                    singleton.mission.FixedCombatEntries.Find(
                        delegate(WorldEntry<FixedCombat> worldEntry)
                        {
                            return 
                                TileEngine.Map.AssetName.EndsWith(
                                    worldEntry.MapContentName) && 
                                worldEntry.MapPosition == mapPosition;
                        });
                if (fixedCombatEntry != null)
                {
                    Session.EncounterFixedCombat(fixedCombatEntry);
                    return true;
                }
            }

            // look for fixed-combats from the map
            MapEntry<FixedCombat> fixedCombatMapEntry = 
                TileEngine.Map.FixedCombatEntries.Find(
                    delegate(MapEntry<FixedCombat> mapEntry)
                    {
                        return mapEntry.MapPosition == mapPosition;
                    });
            if (fixedCombatMapEntry != null)
            {
                Session.EncounterFixedCombat(fixedCombatMapEntry);
                return true;
            }

            // look for chests from the current mission
            if (singleton.mission != null)
            {
                MapEntry<Chest> chestEntry = singleton.mission.ChestEntries.Find(
                    delegate(WorldEntry<Chest> worldEntry)
                    {
                        return
                            TileEngine.Map.AssetName.EndsWith(
                                worldEntry.MapContentName) &&
                            worldEntry.MapPosition == mapPosition;
                    });
                if (chestEntry != null)
                {
                    Session.EncounterChest(chestEntry);
                    return true;
                }
            }

            // look for chests from the map
            MapEntry<Chest> chestMapEntry =
                TileEngine.Map.ChestEntries.Find(delegate(MapEntry<Chest> mapEntry)
                {
                    return mapEntry.MapPosition == mapPosition;
                });
            if (chestMapEntry != null)
            {
                Session.EncounterChest(chestMapEntry);
                return true;
            }

            // look for player NPCs from the map
            MapEntry<Player> playerNpcEntry =
                TileEngine.Map.PlayerNpcEntries.Find(delegate(MapEntry<Player> mapEntry)
                {
                    return mapEntry.MapPosition == mapPosition;
                });
            if (playerNpcEntry != null)
            {
                Session.EncounterPlayerNpc(playerNpcEntry);
                return true;
            }

            // look for mission NPCs from the map
            MapEntry<MissionNpc> missionNpcEntry =
                TileEngine.Map.MissionNpcEntries.Find(delegate(MapEntry<MissionNpc> mapEntry)
                {
                    return mapEntry.MapPosition == mapPosition;
                });
            if (missionNpcEntry != null)
            {
                Session.EncounterMissionNpc(missionNpcEntry);
                return true;
            }

            // look for portals from the map
            MapEntry<Portal> portalEntry =
                TileEngine.Map.PortalEntries.Find(delegate(MapEntry<Portal> mapEntry)
                {
                    return mapEntry.MapPosition == mapPosition;
                });
            if (portalEntry != null)
            {
                Session.EncounterPortal(portalEntry);
                return true;
            }

            // look for inns from the map
            //MapEntry<Inn> innEntry =
                //TileEngine.Map.InnEntries.Find(delegate(MapEntry<Inn> mapEntry)
                //{
                    //return mapEntry.MapPosition == mapPosition;
                //});
            //if (innEntry != null)
            //{
                //Session.EncounterInn(innEntry);
                //return true;
            //}

            // look for stores from the map
            //MapEntry<Store> storeEntry =
                //TileEngine.Map.StoreEntries.Find(delegate(MapEntry<Store> mapEntry)
                //{
                    //return mapEntry.MapPosition == mapPosition;
                //});
            //if (storeEntry != null)
            //{
                //Session.EncounterStore(storeEntry);
                //return true;
            //}

            // nothing encountered
            return false;
        }


        /// <summary>
        /// Performs the actions associated with encountering a FixedCombat entry.
        /// </summary>
        public static void EncounterFixedCombat(MapEntry<FixedCombat> fixedCombatEntry)
        {
            // check the parameter
            if ((fixedCombatEntry == null) || (fixedCombatEntry.Content == null))
            {
                throw new ArgumentNullException("fixedCombatEntry");
            }

            if (!CombatEngine.IsActive)
            {
                // start combat
                CombatEngine.StartNewCombat(fixedCombatEntry);
            }
        }


        /// <summary>
        /// Performs the actions associated with encountering a Chest entry.
        /// </summary>
        public static void EncounterChest(MapEntry<Chest> chestEntry)
        {
            // check the parameter
            if ((chestEntry == null) || (chestEntry.Content == null))
            {
                throw new ArgumentNullException("chestEntry");
            }

            // add the chest screen
            singleton.screenManager.AddScreen(new ChestScreen(chestEntry));
        }


        /// <summary>
        /// Performs the actions associated with encountering a player-NPC entry.
        /// </summary>
        public static void EncounterPlayerNpc(MapEntry<Player> playerEntry)
        {
            // check the parameter
            if ((playerEntry == null) || (playerEntry.Content == null))
            {
                throw new ArgumentNullException("playerEntry");
            }

            // add the player-NPC screen
            singleton.screenManager.AddScreen(new PlayerNpcScreen(playerEntry));
        }


        /// <summary>
        /// Performs the actions associated with encountering a MissionNpc entry.
        /// </summary>
        public static void EncounterMissionNpc(MapEntry<MissionNpc> missionNpcEntry)
        {
            // check the parameter
            if ((missionNpcEntry == null) || (missionNpcEntry.Content == null))
            {
                throw new ArgumentNullException("missionNpcEntry");
            }

            // add the mission-NPC screen
            singleton.screenManager.AddScreen(new MissionNpcScreen(missionNpcEntry));
        }


        /// <summary>
        /// Performs the actions associated with encountering an Inn entry.
        /// </summary>
        //public static void EncounterInn(MapEntry<Inn> innEntry)
        //{
            // check the parameter
            //if ((innEntry == null) || (innEntry.Content == null))
            //{
                //throw new ArgumentNullException("innEntry");
            //}

            // add the inn screen
            //singleton.screenManager.AddScreen(new InnScreen(innEntry.Content));
        //}


        /// <summary>
        /// Performs the actions associated with encountering a Store entry.
        /// </summary>
        //public static void EncounterStore(MapEntry<Store> storeEntry)
        //{
            // check the parameter
            //if ((storeEntry == null) || (storeEntry.Content == null))
            //{
                //throw new ArgumentNullException("storeEntry");
            //}

            // add the store screen
            //singleton.screenManager.AddScreen(new StoreScreen(storeEntry.Content));
        //}
        

        /// <summary>
        /// Performs the actions associated with encountering a Portal entry.
        /// </summary>
        public static void EncounterPortal(MapEntry<Portal> portalEntry)
        {
            // check the parameter
            if ((portalEntry == null) || (portalEntry.Content == null))
            {
                throw new ArgumentNullException("portalEntry");
            }

            // change to the new map
            ChangeMap(portalEntry.Content.DestinationMapContentName, 
                portalEntry.Content);
        }


        /// <summary>
        /// Check if a random combat should start.  If so, start combat immediately.
        /// </summary>
        /// <returns>True if combat was started, false otherwise.</returns>
        public static bool CheckForRandomCombat(RandomCombat randomCombat)
        {
            // check the parameter
            if ((randomCombat == null) || (randomCombat.CombatProbability <= 0))
            {
                return false;
            }

            // check to see if combat has already started
            if (CombatEngine.IsActive)
            {
                return false;
            }

            // check to see if the random combat starts
            int randomCombatCheck = random.Next(100);
            if (randomCombatCheck < randomCombat.CombatProbability)
            {
                // start combat immediately
                CombatEngine.StartNewCombat(randomCombat);
                return true;
            }

            // combat not started
            return false;
        }


        #endregion
        

        #region Missions


        /// <summary>
        /// The main mission line for this session.
        /// </summary>
        private MissionLine missionLine;

        /// <summary>
        /// The main mission line for this session.
        /// </summary>
        public static MissionLine MissionLine
        {
            get { return (singleton == null ? null : singleton.missionLine); }
        }


        /// <summary>
        /// If true, the main mission line for this session is complete.
        /// </summary>
        public static bool IsMissionLineComplete
        {
            get
            {
                if ((singleton == null) || (singleton.missionLine == null) ||
                    (singleton.missionLine.MissionContentNames == null))
                {
                    return false;
                }
                return singleton.currentMissionIndex >= 
                    singleton.missionLine.MissionContentNames.Count;
            }
        }


        /// <summary>
        /// The current mission in this session.
        /// </summary>
        private Mission mission;

        /// <summary>
        /// The current mission in this session.
        /// </summary>
        public static Mission Mission
        {
            get { return (singleton == null ? null : singleton.mission); }
        }


        /// <summary>
        /// The index of the current mission into the mission line.
        /// </summary>
        private int currentMissionIndex = 0;

        /// <summary>
        /// The index of the current mission into the mission line.
        /// </summary>
        public static int CurrentMissionIndex
        {
            get { return (singleton == null ? -1 : singleton.currentMissionIndex); }
        }


        /// <summary>
        /// Update the current mission and mission line for this session.
        /// </summary>
        public void UpdateMission()
        {
            // check the singleton's state to see if we should care about missions
            if ((party == null) || (missionLine == null))
            {
                return;
            }

            // if we don't have a mission, then take the next one from teh list
            if ((mission == null) && (missionLine.Missions.Count > 0) && 
                !Session.IsMissionLineComplete)
            {
                mission = missionLine.Missions[currentMissionIndex];
                mission.Stage = Mission.MissionStage.NotStarted;
                // clear the monster-kill record
                party.MonsterKills.Clear();
                // clear the modified-mission lists
                modifiedMissionChests.Clear();
                removedMissionChests.Clear();
                removedMissionFixedCombats.Clear();
            }

            // handle mission-stage transitions
            if ((mission != null) && !Session.IsMissionLineComplete)
            {
                switch (mission.Stage)
                {
                    case Mission.MissionStage.NotStarted:
                        // start the new mission
                        mission.Stage = Mission.MissionStage.InProgress;
                        if (!mission.AreRequirementsMet)
                        {
                            // show the announcement of the mission and the requirements
                            ScreenManager.AddScreen(new MissionLogScreen(mission));
                        }
                        break;

                    case Mission.MissionStage.InProgress:
                        // update monster requirements
                        foreach (MissionRequirement<Monster> monsterRequirement in
                            mission.MonsterRequirements)
                        {
                            monsterRequirement.CompletedCount = 0;
                            Monster monster = monsterRequirement.Content;
                            if (party.MonsterKills.ContainsKey(monster.AssetName))
                            {
                                monsterRequirement.CompletedCount =
                                    party.MonsterKills[monster.AssetName];
                            }
                        }
                        // update gear requirements
                        foreach (MissionRequirement<Gear> gearRequirement in
                            mission.GearRequirements)
                        {
                            gearRequirement.CompletedCount = 0;
                            foreach (ContentEntry<Gear> entry in party.Inventory)
                            {
                                if (entry.Content == gearRequirement.Content)
                                {
                                    gearRequirement.CompletedCount += entry.Count;
                                }
                            }
                        }
                        // check to see if the requirements have been met
                        if (mission.AreRequirementsMet)
                        {
                            // immediately remove the gear
                            foreach (MissionRequirement<Gear> gearRequirement in
                                mission.GearRequirements)
                            {
                                Gear gear = gearRequirement.Content;
                                party.RemoveFromInventory(gear,
                                    gearRequirement.Count);
                            }
                            // check to see if there is a destination
                            if (String.IsNullOrEmpty(
                                mission.DestinationMapContentName))
                            {
                                // complete the mission
                                mission.Stage = Mission.MissionStage.Completed;
                                // show the completion dialogue
                                if (!String.IsNullOrEmpty(mission.CompletionMessage))
                                {
                                    DialogueScreen dialogueScreen = new DialogueScreen();
                                    dialogueScreen.TitleText = "Mission Complete";
                                    dialogueScreen.BackText = String.Empty;
                                    dialogueScreen.DialogueText =
                                        mission.CompletionMessage;
                                    ScreenManager.AddScreen(dialogueScreen);
                                }
                            }
                            else
                            {
                                mission.Stage = Mission.MissionStage.RequirementsMet;
                                // remind the player about the destination
                                screenManager.AddScreen(new MissionLogScreen(mission));
                            }
                        }
                        break;

                    case Mission.MissionStage.RequirementsMet:
                        break;

                    case Mission.MissionStage.Completed:
                        // show the mission rewards screen
                        RewardsScreen rewardsScreen =
                            new RewardsScreen(RewardsScreen.RewardScreenMode.Mission,
                            Mission.ExperienceReward, Mission.MoneyReward, Mission.GearRewards);
                        screenManager.AddScreen(rewardsScreen);
                        // advance to the next mission
                        currentMissionIndex++;
                        mission = null;
                        break;
                }
            }
        }


        #endregion


        #region Modified/Removed Content


        /// <summary>
        /// The chests removed from the map asset by player actions.
        /// </summary>
        private List<WorldEntry<Chest>> removedMapChests = 
            new List<WorldEntry<Chest>>();

        /// <summary>
        /// The chests removed from the current mission asset by player actions.
        /// </summary>
        private List<WorldEntry<Chest>> removedMissionChests = 
            new List<WorldEntry<Chest>>();

        /// <summary>
        /// Remove the given chest entry from the current map or mission.
        /// </summary>
        public static void RemoveChest(MapEntry<Chest> mapEntry)
        {
            // check the parameter
            if (mapEntry == null)
            {
                return;
            }

            // check the map for the item first
            if (TileEngine.Map != null)
            {
                int removedEntries = TileEngine.Map.ChestEntries.RemoveAll(
                    delegate(MapEntry<Chest> entry)
                    {
                        return ((entry.ContentName == mapEntry.ContentName) &&
                            (entry.MapPosition == mapEntry.MapPosition));
                    });
                if (removedEntries > 0)
                {
                    WorldEntry<Chest> worldEntry = new WorldEntry<Chest>();
                    worldEntry.Content = mapEntry.Content;
                    worldEntry.ContentName = mapEntry.ContentName;
                    worldEntry.Count = mapEntry.Count;
                    worldEntry.Direction = mapEntry.Direction;
                    worldEntry.MapContentName = TileEngine.Map.AssetName;
                    worldEntry.MapPosition = mapEntry.MapPosition;
                    singleton.removedMapChests.Add(worldEntry);
                    return;
                }
            }

            // look for the map entry within the mission
            if (singleton.mission != null)
            {
                int removedEntries = singleton.mission.ChestEntries.RemoveAll(
                    delegate(WorldEntry<Chest> entry)
                    {
                        return ((entry.ContentName == mapEntry.ContentName) &&
                            (entry.MapPosition == mapEntry.MapPosition) &&
                            TileEngine.Map.AssetName.EndsWith(entry.MapContentName));
                    });
                if (removedEntries > 0)
                {
                    WorldEntry<Chest> worldEntry = new WorldEntry<Chest>();
                    worldEntry.Content = mapEntry.Content;
                    worldEntry.ContentName = mapEntry.ContentName;
                    worldEntry.Count = mapEntry.Count;
                    worldEntry.Direction = mapEntry.Direction;
                    worldEntry.MapContentName = TileEngine.Map.AssetName;
                    worldEntry.MapPosition = mapEntry.MapPosition;
                    singleton.removedMissionChests.Add(worldEntry);
                    return;
                }
            }
        }


        /// <summary>
        /// The fixed-combats removed from the map asset by player actions.
        /// </summary>
        private List<WorldEntry<FixedCombat>> removedMapFixedCombats =
            new List<WorldEntry<FixedCombat>>();

        /// <summary>
        /// The fixed-combats removed from the current mission asset by player actions.
        /// </summary>
        private List<WorldEntry<FixedCombat>> removedMissionFixedCombats =
            new List<WorldEntry<FixedCombat>>();

        /// <summary>
        /// Remove the given fixed-combat entry from the current map or mission.
        /// </summary>
        public static void RemoveFixedCombat(MapEntry<FixedCombat> mapEntry)
        {
            // check the parameter
            if (mapEntry == null)
            {
                return;
            }

            // check the map for the item first
            if (TileEngine.Map != null)
            {
                int removedEntries = TileEngine.Map.FixedCombatEntries.RemoveAll(
                    delegate(MapEntry<FixedCombat> entry)
                    {
                        return ((entry.ContentName == mapEntry.ContentName) &&
                            (entry.MapPosition == mapEntry.MapPosition));
                    });
                if (removedEntries > 0)
                {
                    WorldEntry<FixedCombat> worldEntry = new WorldEntry<FixedCombat>();
                    worldEntry.Content = mapEntry.Content;
                    worldEntry.ContentName = mapEntry.ContentName;
                    worldEntry.Count = mapEntry.Count;
                    worldEntry.Direction = mapEntry.Direction;
                    worldEntry.MapContentName = TileEngine.Map.AssetName;
                    worldEntry.MapPosition = mapEntry.MapPosition;
                    singleton.removedMapFixedCombats.Add(worldEntry);
                    return;
                }
            }

            // look for the map entry within the mission
            if (singleton.mission != null)
            {
                int removedEntries = singleton.mission.FixedCombatEntries.RemoveAll(
                    delegate(WorldEntry<FixedCombat> entry)
                    {
                        return ((entry.ContentName == mapEntry.ContentName) &&
                            (entry.MapPosition == mapEntry.MapPosition) &&
                            TileEngine.Map.AssetName.EndsWith(entry.MapContentName));
                    });
                if (removedEntries > 0)
                {
                    WorldEntry<FixedCombat> worldEntry = new WorldEntry<FixedCombat>();
                    worldEntry.Content = mapEntry.Content;
                    worldEntry.ContentName = mapEntry.ContentName;
                    worldEntry.Count = mapEntry.Count;
                    worldEntry.Direction = mapEntry.Direction;
                    worldEntry.MapContentName = TileEngine.Map.AssetName;
                    worldEntry.MapPosition = mapEntry.MapPosition;
                    singleton.removedMissionFixedCombats.Add(worldEntry);
                    return;
                }
            }
        }


        /// <summary>
        /// The player NPCs removed from the map asset by player actions.
        /// </summary>
        private List<WorldEntry<Player>> removedMapPlayerNpcs =
            new List<WorldEntry<Player>>();

        /// <summary>
        /// Remove the given player NPC entry from the current map or mission.
        /// </summary>
        public static void RemovePlayerNpc(MapEntry<Player> mapEntry)
        {
            // check the parameter
            if (mapEntry == null)
            {
                return;
            }

            // check the map for the item
            if (TileEngine.Map != null)
            {
                int removedEntries = TileEngine.Map.PlayerNpcEntries.RemoveAll(
                    delegate(MapEntry<Player> entry)
                    {
                        return ((entry.ContentName == mapEntry.ContentName) &&
                            (entry.MapPosition == mapEntry.MapPosition));
                    });
                if (removedEntries > 0)
                {
                    WorldEntry<Player> worldEntry = new WorldEntry<Player>();
                    worldEntry.Content = mapEntry.Content;
                    worldEntry.ContentName = mapEntry.ContentName;
                    worldEntry.Count = mapEntry.Count;
                    worldEntry.Direction = mapEntry.Direction;
                    worldEntry.MapContentName = TileEngine.Map.AssetName;
                    worldEntry.MapPosition = mapEntry.MapPosition;
                    singleton.removedMapPlayerNpcs.Add(worldEntry);
                    return;
                }
            }

            // missions don't have a list of player NPCs
        }


        /// <summary>
        /// The chests that have been modified, but not emptied, by player action.
        /// </summary>
        private List<ModifiedChestEntry> modifiedMapChests = 
            new List<ModifiedChestEntry>();


        /// <summary>
        /// The chests belonging to the current mission that have been modified,
        /// but not emptied, by player action.
        /// </summary>
        private List<ModifiedChestEntry> modifiedMissionChests = 
            new List<ModifiedChestEntry>();


        /// <summary>
        /// Stores the entry for a chest on the current map or mission that has been
        /// modified but not emptied.
        /// </summary>
        public static void StoreModifiedChest(MapEntry<Chest> mapEntry)
        {
            // check the parameter
            if ((mapEntry == null) || (mapEntry.Content == null))
            {
                throw new ArgumentNullException("mapEntry");
            }

            Predicate<ModifiedChestEntry> checkModifiedChests = 
                delegate(ModifiedChestEntry entry)
                {
                    return
                        (TileEngine.Map.AssetName.EndsWith(
                            entry.WorldEntry.MapContentName) &&
                        (entry.WorldEntry.ContentName == mapEntry.ContentName) &&
                        (entry.WorldEntry.MapPosition == mapEntry.MapPosition));
                };

            // check the map for the item first
            if ((TileEngine.Map != null) && TileEngine.Map.ChestEntries.Exists(
                delegate(MapEntry<Chest> entry)
                {
                    return ((entry.ContentName == mapEntry.ContentName) &&
                        (entry.MapPosition == mapEntry.MapPosition));
                }))
            {
                singleton.modifiedMapChests.RemoveAll(checkModifiedChests);
                ModifiedChestEntry modifiedChestEntry = new ModifiedChestEntry();
                modifiedChestEntry.WorldEntry.Content = mapEntry.Content;
                modifiedChestEntry.WorldEntry.ContentName = mapEntry.ContentName;
                modifiedChestEntry.WorldEntry.Count = mapEntry.Count;
                modifiedChestEntry.WorldEntry.Direction = mapEntry.Direction;
                modifiedChestEntry.WorldEntry.MapContentName = 
                    TileEngine.Map.AssetName;
                modifiedChestEntry.WorldEntry.MapPosition = mapEntry.MapPosition;
                Chest chest = mapEntry.Content;
                modifiedChestEntry.ChestEntries.AddRange(chest.Entries);
                modifiedChestEntry.Money = chest.Money;
                singleton.modifiedMapChests.Add(modifiedChestEntry);
                return;
            }
            

            // look for the map entry within the mission
            if ((singleton.mission != null) && singleton.mission.ChestEntries.Exists(
                delegate(WorldEntry<Chest> entry)
                {
                    return ((entry.ContentName == mapEntry.ContentName) &&
                        (entry.MapPosition == mapEntry.MapPosition) &&
                        TileEngine.Map.AssetName.EndsWith(entry.MapContentName));
                }))
            {
                singleton.modifiedMissionChests.RemoveAll(checkModifiedChests);
                ModifiedChestEntry modifiedChestEntry = new ModifiedChestEntry();
                modifiedChestEntry.WorldEntry.Content = mapEntry.Content;
                modifiedChestEntry.WorldEntry.ContentName = mapEntry.ContentName;
                modifiedChestEntry.WorldEntry.Count = mapEntry.Count;
                modifiedChestEntry.WorldEntry.Direction = mapEntry.Direction;
                modifiedChestEntry.WorldEntry.MapContentName = TileEngine.Map.AssetName;
                modifiedChestEntry.WorldEntry.MapPosition = mapEntry.MapPosition;
                Chest chest = mapEntry.Content;
                modifiedChestEntry.ChestEntries.AddRange(chest.Entries);
                modifiedChestEntry.Money = chest.Money;
                singleton.modifiedMissionChests.Add(modifiedChestEntry);
                return;
            }
        }


        /// <summary>
        /// Remove the specified content from the map, typically from an earlier session.
        /// </summary>
        private void ModifyMap(Map map)
        {
            // check the parameter
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }

            // remove all chests that were emptied already
            if ((removedMapChests != null) && (removedMapChests.Count > 0))
            {
                // check each removed-content entry
                map.ChestEntries.RemoveAll(delegate(MapEntry<Chest> mapEntry)
                {
                    return (removedMapChests.Exists(
                        delegate(WorldEntry<Chest> removedEntry)
                        {
                            return 
                                (map.AssetName.EndsWith(removedEntry.MapContentName) &&
                                (removedEntry.ContentName == mapEntry.ContentName) &&
                                (removedEntry.MapPosition == mapEntry.MapPosition));
                        }));
                });
            }

            // remove all fixed-combats that were defeated already
            if ((removedMapFixedCombats != null) && (removedMapFixedCombats.Count > 0))
            {
                // check each removed-content entry
                map.FixedCombatEntries.RemoveAll(delegate(MapEntry<FixedCombat> mapEntry)
                {
                    return (removedMapFixedCombats.Exists(
                        delegate(WorldEntry<FixedCombat> removedEntry)
                        {
                            return
                                (map.AssetName.EndsWith(removedEntry.MapContentName) &&
                                (removedEntry.ContentName == mapEntry.ContentName) &&
                                (removedEntry.MapPosition == mapEntry.MapPosition));
                        }));
                });
            }

            // remove the player NPCs that have already joined the party
            if ((removedMapPlayerNpcs != null) && (removedMapPlayerNpcs.Count > 0))
            {
                // check each removed-content entry
                map.PlayerNpcEntries.RemoveAll(delegate(MapEntry<Player> mapEntry)
                {
                    return (removedMapPlayerNpcs.Exists(
                        delegate(WorldEntry<Player> removedEntry)
                        {
                            return
                                (map.AssetName.EndsWith(removedEntry.MapContentName) &&
                                (removedEntry.ContentName == mapEntry.ContentName) &&
                                (removedEntry.MapPosition == mapEntry.MapPosition));
                        }));
                });
            }

            // replace the chest entries of modified chests - they are already clones
            if ((modifiedMapChests != null) && (modifiedMapChests.Count > 0))
            {
                foreach (MapEntry<Chest> entry in map.ChestEntries)
                {
                    ModifiedChestEntry modifiedEntry = modifiedMapChests.Find(
                        delegate(ModifiedChestEntry modifiedTestEntry)
                        {
                            return
                                (map.AssetName.EndsWith(
                                    modifiedTestEntry.WorldEntry.MapContentName) &&
                                (modifiedTestEntry.WorldEntry.ContentName == 
                                    entry.ContentName) &&
                                (modifiedTestEntry.WorldEntry.MapPosition == 
                                    entry.MapPosition));
                        });
                    // if the chest has been modified, apply the changes
                    if (modifiedEntry != null)
                    {
                        ModifyChest(entry.Content, modifiedEntry);
                    }
                }
            }
        }


        /// <summary>
        /// Remove the specified content from the map, typically from an earlier session.
        /// </summary>
        private void ModifyMission(Mission mission)
        {
            // check the parameter
            if (mission == null)
            {
                throw new ArgumentNullException("mission");
            }

            // remove all chests that were emptied arleady
            if ((removedMissionChests != null) && (removedMissionChests.Count > 0))
            {
                // check each removed-content entry
                mission.ChestEntries.RemoveAll(
                    delegate(WorldEntry<Chest> worldEntry)
                    {
                        return (removedMissionChests.Exists(
                            delegate(WorldEntry<Chest> removedEntry)
                            {
                                return
                                    (removedEntry.MapContentName.EndsWith(
                                        worldEntry.MapContentName) &&
                                    (removedEntry.ContentName == 
                                        worldEntry.ContentName) &&
                                    (removedEntry.MapPosition == 
                                        worldEntry.MapPosition));
                            }));
                    });
            }

            // remove all of the fixed-combats that have already been defeated
            if ((removedMissionFixedCombats != null) &&
                (removedMissionFixedCombats.Count > 0))
            {
                // check each removed-content entry
                mission.FixedCombatEntries.RemoveAll(
                    delegate(WorldEntry<FixedCombat> worldEntry)
                    {
                        return (removedMissionFixedCombats.Exists(
                            delegate(WorldEntry<FixedCombat> removedEntry)
                            {
                                return
                                    (removedEntry.MapContentName.EndsWith(
                                        worldEntry.MapContentName) &&
                                    (removedEntry.ContentName == 
                                        worldEntry.ContentName) &&
                                    (removedEntry.MapPosition == 
                                        worldEntry.MapPosition));
                            }));
                    });
            }

            // replace the chest entries of modified chests - they are already clones
            if ((modifiedMissionChests != null) && (modifiedMissionChests.Count > 0))
            {
                foreach (WorldEntry<Chest> entry in mission.ChestEntries)
                {
                    ModifiedChestEntry modifiedEntry = modifiedMissionChests.Find(
                        delegate(ModifiedChestEntry modifiedTestEntry)
                        {
                            return
                                ((modifiedTestEntry.WorldEntry.MapContentName == 
                                    entry.MapContentName) &&
                                (modifiedTestEntry.WorldEntry.ContentName == 
                                    entry.ContentName) &&
                                (modifiedTestEntry.WorldEntry.MapPosition == 
                                    entry.MapPosition));
                        });
                    // if the chest has been modified, apply the changes
                    if (modifiedEntry != null)
                    {
                        ModifyChest(entry.Content, modifiedEntry);
                    }
                }
            }
        }


        /// <summary>
        /// Modify a Chest object based on the data in a ModifiedChestEntry object.
        /// </summary>
        private void ModifyChest(Chest chest, ModifiedChestEntry modifiedChestEntry)
        {
            // check the parameters
            if ((chest == null) || (modifiedChestEntry == null))
            {
                return;
            }

            // set the new money amount
            chest.Money = modifiedChestEntry.Money;

            // remove all contents not found in the modified version
            chest.Entries.RemoveAll(delegate(ContentEntry<Gear> contentEntry)
            {
                return !modifiedChestEntry.ChestEntries.Exists(
                    delegate(ContentEntry<Gear> modifiedTestEntry)
                    {
                        return (contentEntry.ContentName ==
                            modifiedTestEntry.ContentName);
                    });
            });

            // set the new counts on the remaining content items
            foreach (ContentEntry<Gear> contentEntry in chest.Entries)
            {
                ContentEntry<Gear> modifiedGearEntry =
                    modifiedChestEntry.ChestEntries.Find(
                        delegate(ContentEntry<Gear> modifiedTestEntry)
                        {
                            return (contentEntry.ContentName ==
                                modifiedTestEntry.ContentName);
                        });
                if (modifiedGearEntry != null)
                {
                    contentEntry.Count = modifiedGearEntry.Count;
                }
            }
        }


        #endregion


        #region User Interface Data


        /// <summary>
        /// The ScreenManager used to manage all UI in the game.
        /// </summary>
        private ScreenManager screenManager;

        /// <summary>
        /// The ScreenManager used to manage all UI in the game.
        /// </summary>
        public static ScreenManager ScreenManager
        {
            get { return (singleton == null ? null : singleton.screenManager); }
        }


        /// <summary>
        /// The GameplayScreen object that created this session.
        /// </summary>
        private GameplayScreen gameplayScreen;


        /// <summary>
        /// The heads-up-display menu shown on the map and combat screens.
        /// </summary>
        private Hud hud;

        /// <summary>
        /// The heads-up-display menu shown on the map and combat screens.
        /// </summary>
        public static Hud Hud
        {
            get { return (singleton == null ? null : singleton.hud); }
        }

        
        #endregion


        #region State Data


        /// <summary>
        /// Returns true if there is an active session.
        /// </summary>
        public static bool IsActive
        {
            get { return singleton != null; }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Private constructor of a Session object.
        /// </summary>
        /// <remarks>
        /// The lack of public constructors forces the singleton model.
        /// </remarks>
        private Session(ScreenManager screenManager, GameplayScreen gameplayScreen)
        {
            // check the parameter
            if (screenManager == null)
            {
                throw new ArgumentNullException("screenManager");
            }
            if (gameplayScreen == null)
            {
                throw new ArgumentNullException("gameplayScreen");
            }

            // assign the parameter
            this.screenManager = screenManager;
            this.gameplayScreen = gameplayScreen;

            // create the HUD interface
            this.hud = new Hud(screenManager);
            this.hud.LoadContent();
        }


        #endregion


        #region Updating


        /// <summary>
        /// Update the session for this frame.
        /// </summary>
        /// <remarks>This should only be called if there are no menus in use.</remarks>
        public static void Update(GameTime gameTime)
        {
            // check the singleton
            if (singleton == null)
            {
                return;
            }

            if (CombatEngine.IsActive)
            {
                CombatEngine.Update(gameTime);
            }
            else
            {
                singleton.UpdateMission();
                TileEngine.Update(gameTime);
            }
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draws the session environment to the screen
        /// </summary>
        public static void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = singleton.screenManager.SpriteBatch;

            if (CombatEngine.IsActive)
            {
                // draw the combat background
                if (TileEngine.Map.CombatTexture != null)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(TileEngine.Map.CombatTexture, Vector2.Zero, 
                        Color.White);
                    spriteBatch.End();
                }

                // draw the combat itself
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                CombatEngine.Draw(gameTime);
                spriteBatch.End();
            }
            else
            {
                singleton.DrawNonCombat(gameTime);
            }

            singleton.hud.Draw();
        }


        /// <summary>
        /// Draws everything related to the non-combat part of the screen
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void DrawNonCombat(GameTime gameTime)
        {
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            // draw the background
            spriteBatch.Begin();
            if (TileEngine.Map.Texture != null)
            {
                // draw the ground layer
                TileEngine.DrawLayers(spriteBatch, true, true, false);
                // draw the character shadows
                DrawShadows(spriteBatch);
            }
            spriteBatch.End();

            // Sort the object layer and player according to depth 
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // draw the party leader
            {
                Player player = party.Players[0];
                Vector2 position = TileEngine.PartyLeaderPosition.ScreenPosition;
                player.Direction = TileEngine.PartyLeaderPosition.Direction;
                player.ResetAnimation(TileEngine.PartyLeaderPosition.IsMoving);
                switch (player.State)
                {
                    case Character.CharacterState.Idle:
                        if (player.MapSprite != null)
                        {
                            player.MapSprite.UpdateAnimation(elapsedSeconds);
                            player.MapSprite.Draw(spriteBatch, position,
                                1f - position.Y / (float)TileEngine.Viewport.Height);
                        }
                        break;

                    case Character.CharacterState.Walking:
                        if (player.WalkingSprite != null)
                        {
                            player.WalkingSprite.UpdateAnimation(elapsedSeconds);
                            player.WalkingSprite.Draw(spriteBatch, position,
                                1f - position.Y / (float)TileEngine.Viewport.Height);
                        }
                        else if (player.MapSprite != null)
                        {
                            player.MapSprite.UpdateAnimation(elapsedSeconds);
                            player.MapSprite.Draw(spriteBatch, position,
                                1f - position.Y / (float)TileEngine.Viewport.Height);
                        }
                        break;
                }
            }

            // draw the player NPCs
            foreach (MapEntry<Player> playerEntry in TileEngine.Map.PlayerNpcEntries)
            {
                if (playerEntry.Content == null)
                {
                    continue;
                }
                Vector2 position = 
                    TileEngine.GetScreenPosition(playerEntry.MapPosition);
                playerEntry.Content.ResetAnimation(false);
                switch (playerEntry.Content.State)
                {
                    case Character.CharacterState.Idle:
                        if (playerEntry.Content.MapSprite != null)
                        {
                            playerEntry.Content.MapSprite.UpdateAnimation(
                                elapsedSeconds);
                            playerEntry.Content.MapSprite.Draw(spriteBatch, position,
                                1f - position.Y / (float)TileEngine.Viewport.Height);
                        }
                        break;

                    case Character.CharacterState.Walking:
                        if (playerEntry.Content.WalkingSprite != null)
                        {
                            playerEntry.Content.WalkingSprite.UpdateAnimation(
                                elapsedSeconds);
                            playerEntry.Content.WalkingSprite.Draw(spriteBatch, position,
                                1f - position.Y / (float)TileEngine.Viewport.Height);
                        }
                        else if (playerEntry.Content.MapSprite != null)
                        {
                            playerEntry.Content.MapSprite.UpdateAnimation(
                                elapsedSeconds);
                            playerEntry.Content.MapSprite.Draw(spriteBatch, position,
                                1f - position.Y / (float)TileEngine.Viewport.Height);
                        }
                        break;
                }
            }

            // draw the mission NPCs
            foreach (MapEntry<MissionNpc> missionNpcEntry in TileEngine.Map.MissionNpcEntries)
            {
                if (missionNpcEntry.Content == null)
                {
                    continue;
                }
                Vector2 position = 
                    TileEngine.GetScreenPosition(missionNpcEntry.MapPosition);
                missionNpcEntry.Content.ResetAnimation(false);
                switch (missionNpcEntry.Content.State)
                {
                    case Character.CharacterState.Idle:
                        if (missionNpcEntry.Content.MapSprite != null)
                        {
                            missionNpcEntry.Content.MapSprite.UpdateAnimation(
                                elapsedSeconds);
                            missionNpcEntry.Content.MapSprite.Draw(spriteBatch, position,
                                1f - position.Y / (float)TileEngine.Viewport.Height);
                        }
                        break;

                    case Character.CharacterState.Walking:
                        if (missionNpcEntry.Content.WalkingSprite != null)
                        {
                            missionNpcEntry.Content.WalkingSprite.UpdateAnimation(
                                elapsedSeconds);
                            missionNpcEntry.Content.WalkingSprite.Draw(spriteBatch, 
                                position,
                                1f - position.Y / (float)TileEngine.Viewport.Height);
                        }
                        else if (missionNpcEntry.Content.MapSprite != null)
                        {
                            missionNpcEntry.Content.MapSprite.UpdateAnimation(
                                elapsedSeconds);
                            missionNpcEntry.Content.MapSprite.Draw(spriteBatch, position,
                                1f - position.Y / (float)TileEngine.Viewport.Height);
                        }
                        break;
                }
            }

            // draw the fixed-combat monsters NPCs from the TileEngine.Map
            // -- since there may be many of the same FixedCombat object 
            //    on the TileEngine.Map, but their animations are handled differently
            foreach (MapEntry<FixedCombat> fixedCombatEntry in 
                TileEngine.Map.FixedCombatEntries)
            {
                if ((fixedCombatEntry.Content == null) || 
                    (fixedCombatEntry.Content.Entries.Count <= 0))
                {
                    continue;
                }
                Vector2 position = 
                    TileEngine.GetScreenPosition(fixedCombatEntry.MapPosition);
                fixedCombatEntry.MapSprite.UpdateAnimation(elapsedSeconds);
                fixedCombatEntry.MapSprite.Draw(spriteBatch, position,
                    1f - position.Y / (float)TileEngine.Viewport.Height);
            }

            // draw the fixed-combat monsters NPCs from the current mission
            // -- since there may be many of the same FixedCombat object 
            //    on the TileEngine.Map, their animations are handled differently
            if ((mission != null) && ((mission.Stage == Mission.MissionStage.InProgress) ||
                (mission.Stage == Mission.MissionStage.RequirementsMet)))
            {
                foreach (WorldEntry<FixedCombat> fixedCombatEntry 
                    in mission.FixedCombatEntries)
                {
                    if ((fixedCombatEntry.Content == null) || 
                        (fixedCombatEntry.Content.Entries.Count <= 0) ||
                        !TileEngine.Map.AssetName.EndsWith(
                            fixedCombatEntry.MapContentName))
                    {
                        continue;
                    }
                    Vector2 position =
                        TileEngine.GetScreenPosition(fixedCombatEntry.MapPosition);
                    fixedCombatEntry.MapSprite.UpdateAnimation(elapsedSeconds);
                    fixedCombatEntry.MapSprite.Draw(spriteBatch, position,
                        1f - position.Y / (float)TileEngine.Viewport.Height);
                }
            }

            // draw the chests from the TileEngine.Map
            foreach (MapEntry<Chest> chestEntry in TileEngine.Map.ChestEntries)
            {
                if (chestEntry.Content == null)
                {
                    continue;
                }
                Vector2 position = TileEngine.GetScreenPosition(chestEntry.MapPosition);
                spriteBatch.Draw(chestEntry.Content.Texture,
                    position, null, Color.White, 0f, Vector2.Zero, 1f,
                    SpriteEffects.None,
                    MathHelper.Clamp(1f - position.Y / 
                        (float)TileEngine.Viewport.Height, 0f, 1f));
            }

            // draw the chests from the current mission
            if ((mission != null) && ((mission.Stage == Mission.MissionStage.InProgress) ||
                (mission.Stage == Mission.MissionStage.RequirementsMet)))
            {
                foreach (WorldEntry<Chest> chestEntry in mission.ChestEntries)
                {
                    if ((chestEntry.Content == null) || 
                        !TileEngine.Map.AssetName.EndsWith(chestEntry.MapContentName))
                    {
                        continue;
                    }
                    Vector2 position = 
                        TileEngine.GetScreenPosition(chestEntry.MapPosition);
                    spriteBatch.Draw(chestEntry.Content.Texture,
                        position, null, Color.White, 0f, Vector2.Zero, 1f,
                        SpriteEffects.None,
                        MathHelper.Clamp(1f - position.Y / 
                            (float)TileEngine.Viewport.Height, 0f, 1f));
                }
            }

            spriteBatch.End();

            // draw the foreground
            spriteBatch.Begin();
            if (TileEngine.Map.Texture != null)
            {
                TileEngine.DrawLayers(spriteBatch, false, false, true);
            }
            spriteBatch.End();
        }


        /// <summary>
        /// Draw the shadows that appear under all characters.
        /// </summary>
        private void DrawShadows(SpriteBatch spriteBatch)
        {
            // draw the shadow of the party leader
            Player player = party.Players[0];
            if (player.ShadowTexture != null)
            {
                spriteBatch.Draw(player.ShadowTexture, 
                    TileEngine.PartyLeaderPosition.ScreenPosition, null, Color.White, 0f,
                    new Vector2(
                        (player.ShadowTexture.Width - TileEngine.Map.TileSize.X) / 2,
                        (player.ShadowTexture.Height - TileEngine.Map.TileSize.Y) / 2 - 
                            player.ShadowTexture.Height / 6), 
                    1f, SpriteEffects.None, 1f);
            }

            // draw the player NPCs' shadows
            foreach (MapEntry<Player> playerEntry in TileEngine.Map.PlayerNpcEntries)
            {
                if (playerEntry.Content == null)
                {
                    continue;
                }
                if (playerEntry.Content.ShadowTexture != null)
                {
                    Vector2 position = 
                        TileEngine.GetScreenPosition(playerEntry.MapPosition);
                    spriteBatch.Draw(playerEntry.Content.ShadowTexture, position,
                        null, Color.White, 0f, 
                        new Vector2(
                        (playerEntry.Content.ShadowTexture.Width - 
                            TileEngine.Map.TileSize.X) / 2,
                        (playerEntry.Content.ShadowTexture.Height - 
                            TileEngine.Map.TileSize.Y) / 2 - 
                            playerEntry.Content.ShadowTexture.Height / 6),
                        1f, SpriteEffects.None, 1f);
                }
            }

            // draw the mission NPCs' shadows
            foreach (MapEntry<MissionNpc> missionNpcEntry in TileEngine.Map.MissionNpcEntries)
            {
                if (missionNpcEntry.Content == null)
                {
                    continue;
                }
                if (missionNpcEntry.Content.ShadowTexture != null)
                {
                    Vector2 position = 
                        TileEngine.GetScreenPosition(missionNpcEntry.MapPosition);
                    spriteBatch.Draw(missionNpcEntry.Content.ShadowTexture, position,
                        null, Color.White, 0f,
                        new Vector2(
                            (missionNpcEntry.Content.ShadowTexture.Width - 
                                TileEngine.Map.TileSize.X) / 2,
                            (missionNpcEntry.Content.ShadowTexture.Height - 
                                TileEngine.Map.TileSize.Y) / 2 - 
                                missionNpcEntry.Content.ShadowTexture.Height / 6),
                        1f, SpriteEffects.None, 1f);
                }
            }

            // draw the fixed-combat monsters NPCs' shadows
            foreach (MapEntry<FixedCombat> fixedCombatEntry in 
                TileEngine.Map.FixedCombatEntries)
            {
                if ((fixedCombatEntry.Content == null) || 
                    (fixedCombatEntry.Content.Entries.Count <= 0))
                {
                    continue;
                }
                Monster monster = fixedCombatEntry.Content.Entries[0].Content;
                if (monster.ShadowTexture != null)
                {
                    Vector2 position = 
                        TileEngine.GetScreenPosition(fixedCombatEntry.MapPosition);
                    spriteBatch.Draw(monster.ShadowTexture, position,
                        null, Color.White, 0f,
                        new Vector2(
                        (monster.ShadowTexture.Width - TileEngine.Map.TileSize.X) / 2,
                        (monster.ShadowTexture.Height - TileEngine.Map.TileSize.Y) / 2 - 
                            monster.ShadowTexture.Height / 6),
                        1f, SpriteEffects.None, 1f);
                }
            }
        }


        #endregion


        #region Starting a New Session


        /// <summary>
        /// Start a new session based on the data provided.
        /// </summary>
        public static void StartNewSession(GameStartDescription gameStartDescription, 
            ScreenManager screenManager, GameplayScreen gameplayScreen)
        {
            // check the parameters
            if (gameStartDescription == null)
            {
                throw new ArgumentNullException("gameStartDescripton");
            }
            if (screenManager == null)
            {
                throw new ArgumentNullException("screenManager");
            }
            if (gameplayScreen == null)
            {
                throw new ArgumentNullException("gameplayScreen");
            }

            // end any existing session
            EndSession();

            // create a new singleton
            singleton = new Session(screenManager, gameplayScreen);

            // set up the initial map
            ChangeMap(gameStartDescription.MapContentName, null);

            // set up the initial party
            ContentManager content = singleton.screenManager.Game.Content;
            singleton.party = new Party(gameStartDescription, content);

            // load the mission line
            singleton.missionLine = content.Load<MissionLine>(
                Path.Combine(@"Missions\MissionLines", 
                gameStartDescription.MissionLineContentName)).Clone() as MissionLine;
        }


        #endregion


        #region Ending a Session


        /// <summary>
        /// End the current session.
        /// </summary>
        public static void EndSession()
        {
            // exit the gameplay screen
            // -- store the gameplay session, for re-entrance
            if (singleton != null)
            {
                GameplayScreen gameplayScreen = singleton.gameplayScreen;
                singleton.gameplayScreen = null;

                // pop the music
                AudioManager.PopMusic();

                // clear the singleton
                singleton = null;

                if (gameplayScreen != null)
                {
                    gameplayScreen.ExitScreen();
                }
            }
        }


        #endregion


        #region Loading a Session


       
        #endregion


        #region Storage


        /// <summary>
        /// The stored StorageDevice object.
        /// </summary>
        private static StorageDevice storageDevice;

        /// <summary>
        /// The container name used for save games.
        /// </summary>
        public static string SaveGameContainerName = "Sector4";


        /// <summary>
        /// A delegate for receiving StorageDevice objects.
        /// </summary>
        public delegate void StorageDeviceDelegate(StorageDevice storageDevice);

        /// <summary>
        /// Asynchronously retrieve a storage device.
        /// </summary>
        /// <param name="retrievalDelegate">
        /// The delegate called when the device is available.
        /// </param>
        /// <remarks>
        /// If there is a suitable cached storage device, 
        /// the delegate may be called directly by this function.
        /// </remarks>
        public static void GetStorageDevice(StorageDeviceDelegate retrievalDelegate)
        {
            // check the parameter
            if (retrievalDelegate == null)
            {
                throw new ArgumentNullException("retrievalDelegate");
            }

            // check the stored storage device
            if ((storageDevice != null) && storageDevice.IsConnected)
            {
                retrievalDelegate(storageDevice);
                return;
            }

            // the storage device must be retrieved
            if (!Guide.IsVisible)
            {
                // Reset the device
                storageDevice = null;
                StorageDevice.BeginShowSelector(GetStorageDeviceResult, retrievalDelegate);
            }


        }


        /// <summary>
        /// Asynchronous callback to the guide's BeginShowStorageDeviceSelector call.
        /// </summary>
        /// <param name="result">The IAsyncResult object with the device.</param>
        private static void GetStorageDeviceResult(IAsyncResult result)
        {
            // check the parameter
            if ((result == null) || !result.IsCompleted)
            {
                return;
            }

            // retrieve and store the storage device
            storageDevice = StorageDevice.EndShowSelector(result);

            // check the new storage device 
            if ((storageDevice != null) && storageDevice.IsConnected)
            {
                // it passes; call the stored delegate
                StorageDeviceDelegate func = result.AsyncState as StorageDeviceDelegate;
                if (func != null)
                {
                    func(storageDevice);
                }
            }
        }

        /// <summary>
        /// Synchronously opens storage container
        /// </summary>
        private static StorageContainer OpenContainer(StorageDevice storageDevice)
        {
            IAsyncResult result =
                storageDevice.BeginOpenContainer(Session.SaveGameContainerName, null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = storageDevice.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            return container;
        }


        #endregion

    
        #region Random


        /// <summary>
        /// The random-number generator used with game events.
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// The random-number generator used with game events.
        /// </summary>
        public static Random Random
        {
            get { return random; }
        }


        #endregion
    }
}
