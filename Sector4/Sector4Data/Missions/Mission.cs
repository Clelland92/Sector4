

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Sector4Data
{
    /// <summary>
    /// A mission that the party can embark on, with goals and rewards.
    /// </summary>
    public class Mission : ContentObject
#if WINDOWS
, ICloneable
#endif
    {
        #region Mission Stage


        /// <summary>
        /// The possible stages of a mission.
        /// </summary>
        public enum MissionStage
        {
            NotStarted,
            InProgress,
            RequirementsMet,
            Completed
        };

        /// <summary>
        /// The current stage of this mission.
        /// </summary>
        private MissionStage stage = MissionStage.NotStarted;

        /// <summary>
        /// The current stage of this mission.
        /// </summary>
        [ContentSerializerIgnore]
        public MissionStage Stage
        {
            get { return stage; }
            set { stage = value; }
        }
        

        #endregion


        #region Description


        /// <summary>
        /// The name of the mission.
        /// </summary>
        private string name;

        /// <summary>
        /// The name of the mission.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        /// <summary>
        /// A description of the mission.
        /// </summary>
        private string description;

        /// <summary>
        /// A description of the mission.
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }


        /// <summary>
        /// A message describing the objective of the mission, 
        /// presented when the player receives the mission.
        /// </summary>
        private string objectiveMessage;

        /// <summary>
        /// A message describing the objective of the mission, 
        /// presented when the player receives the mission.
        /// </summary>
        public string ObjectiveMessage
        {
            get { return objectiveMessage; }
            set { objectiveMessage = value; }
        }


        /// <summary>
        /// A message announcing the completion of the mission, 
        /// presented when the player reaches the goals of the mission.
        /// </summary>
        private string completionMessage;

        public string CompletionMessage
        {
            get { return completionMessage; }
            set { completionMessage = value; }
        }


        #endregion


        #region Requirements


        /// <summary>
        /// The gear that the player must have to finish the mission.
        /// </summary>
        private List<MissionRequirement<Gear>> gearRequirements =
            new List<MissionRequirement<Gear>>();

        /// <summary>
        /// The gear that the player must have to finish the mission.
        /// </summary>
        public List<MissionRequirement<Gear>> GearRequirements
        {
            get { return gearRequirements; }
            set { gearRequirements = value; }
        }


        /// <summary>
        /// The monsters that must be killed to finish the mission.
        /// </summary>
        private List<MissionRequirement<Monster>> monsterRequirements =
            new List<MissionRequirement<Monster>>();

        /// <summary>
        /// The monsters that must be killed to finish the mission.
        /// </summary>
        public List<MissionRequirement<Monster>> MonsterRequirements
        {
            get { return monsterRequirements; }
            set { monsterRequirements = value; }
        }


        /// <summary>
        /// Returns true if all requirements for this mission have been met.
        /// </summary>
        public bool AreRequirementsMet
        {
            get
            {
                foreach (MissionRequirement<Gear> gearRequirement in gearRequirements)
                {
                    if (gearRequirement.CompletedCount < gearRequirement.Count)
                    {
                        return false;
                    }
                }
                foreach (MissionRequirement<Monster> monsterRequirement 
                    in monsterRequirements)
                {
                    if (monsterRequirement.CompletedCount < monsterRequirement.Count)
                    {
                        return false;
                    }
                }
                return true;
            }
        }


        #endregion


        #region Mission Content


        /// <summary>
        /// The fixed combat encounters added to the world when this mission is active.
        /// </summary>
        private List<WorldEntry<FixedCombat>> fixedCombatEntries = 
            new List<WorldEntry<FixedCombat>>();

        /// <summary>
        /// The fixed combat encounters added to the world when this mission is active.
        /// </summary>
        public List<WorldEntry<FixedCombat>> FixedCombatEntries
        {
            get { return fixedCombatEntries; }
            set { fixedCombatEntries = value; }
        }


        /// <summary>
        /// The chests added to thew orld when this mission is active.
        /// </summary>
        private List<WorldEntry<Chest>> chestEntries = new List<WorldEntry<Chest>>();

        /// <summary>
        /// The chests added to thew orld when this mission is active.
        /// </summary>
        public List<WorldEntry<Chest>> ChestEntries
        {
            get { return chestEntries; }
            set { chestEntries = value; }
        }


        #endregion


        #region Destination


        /// <summary>
        /// The map with the destination Npc, if any.
        /// </summary>
        private string destinationMapContentName;

        /// <summary>
        /// The map with the destination Npc, if any.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public string DestinationMapContentName
        {
            get { return destinationMapContentName; }
            set { destinationMapContentName = value; }
        }


        /// <summary>
        /// The Npc that the party must visit to finish the mission, if any.
        /// </summary>
        private string destinationNpcContentName;

        /// <summary>
        /// The Npc that the party must visit to finish the mission, if any.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public string DestinationNpcContentName
        {
            get { return destinationNpcContentName; }
            set { destinationNpcContentName = value; }
        }


        /// <summary>
        /// The message shown when the party is eligible to complete the mission, if any.
        /// </summary>
        private string destinationObjectiveMessage;

        /// <summary>
        /// The message shown when the party is eligible to complete the mission, if any.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public string DestinationObjectiveMessage
        {
            get { return destinationObjectiveMessage; }
            set { destinationObjectiveMessage = value; }
        }


        #endregion


        #region Reward


        /// <summary>
        /// The number of experience points given to each party member as a reward.
        /// </summary>
        private int experienceReward;

        /// <summary>
        /// The number of experience points given to each party member as a reward.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public int ExperienceReward
        {
            get { return experienceReward; }
            set { experienceReward = value; }
        }


        /// <summary>
        /// The amount of money given to the party as a reward.
        /// </summary>
        private int moneyReward;

        /// <summary>
        /// The amount of money given to the party as a reward.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public int MoneyReward
        {
            get { return moneyReward; }
            set { moneyReward = value; }
        }


        /// <summary>
        /// The content names of the gear given to the party as a reward.
        /// </summary>
        private List<string> gearRewardContentNames = new List<string>();

        /// <summary>
        /// The content names of the gear given to the party as a reward.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public List<string> GearRewardContentNames
        {
            get { return gearRewardContentNames; }
            set { gearRewardContentNames = value; }
        }


        /// <summary>
        /// The gear given to the party as a reward.
        /// </summary>
        private List<Gear> gearRewards = new List<Gear>();

        /// <summary>
        /// The gear given to the party as a reward.
        /// </summary>
        [ContentSerializerIgnore]
        public List<Gear> GearRewards
        {
            get { return gearRewards; }
            set { gearRewards = value; }
        }


        #endregion


        #region Content Type Reader


        /// <summary>
        /// Reads a Mission object from the content pipeline.
        /// </summary>
        public class MissionReader : ContentTypeReader<Mission>
        {
            /// <summary>
            /// Reads a Mission object from the content pipeline.
            /// </summary>
            protected override Mission Read(ContentReader input, Mission existingInstance)
            {
                Mission mission = existingInstance;
                if (mission == null)
                {
                    mission = new Mission();
                }

                mission.AssetName = input.AssetName;

                mission.Name = input.ReadString();
                mission.Description = input.ReadString();
                mission.ObjectiveMessage = input.ReadString();
                mission.CompletionMessage = input.ReadString();

                mission.GearRequirements.AddRange(
                    input.ReadObject<List<MissionRequirement<Gear>>>());
                mission.MonsterRequirements.AddRange(
                    input.ReadObject<List<MissionRequirement<Monster>>>());

                // load the fixed combat entries
                Random random = new Random();
                mission.FixedCombatEntries.AddRange(
                    input.ReadObject<List<WorldEntry<FixedCombat>>>());
                foreach (WorldEntry<FixedCombat> fixedCombatEntry in
                    mission.FixedCombatEntries)
                {
                    fixedCombatEntry.Content =
                        input.ContentManager.Load<FixedCombat>(
                        System.IO.Path.Combine(@"Maps\FixedCombats",
                        fixedCombatEntry.ContentName));
                    // clone the map sprite in the entry, as there may be many entries
                    // per FixedCombat
                    fixedCombatEntry.MapSprite =
                        fixedCombatEntry.Content.Entries[0].Content.MapSprite.Clone()
                        as AnimatingSprite;
                    // play the idle animation
                    fixedCombatEntry.MapSprite.PlayAnimation("Idle",
                        fixedCombatEntry.Direction);
                    // advance in a random amount so the animations aren't synchronized
                    fixedCombatEntry.MapSprite.UpdateAnimation(
                        4f * (float)random.NextDouble());
                }

                mission.ChestEntries.AddRange(
                    input.ReadObject<List<WorldEntry<Chest>>>());
                foreach (WorldEntry<Chest> chestEntry in mission.ChestEntries)
                {
                    chestEntry.Content = input.ContentManager.Load<Chest>(
                        System.IO.Path.Combine(@"Maps\Chests",
                        chestEntry.ContentName)).Clone() as Chest;
                }

                mission.DestinationMapContentName = input.ReadString();
                mission.DestinationNpcContentName = input.ReadString();
                mission.DestinationObjectiveMessage = input.ReadString();

                mission.experienceReward = input.ReadInt32();
                mission.moneyReward = input.ReadInt32();

                mission.GearRewardContentNames.AddRange(
                    input.ReadObject<List<string>>());
                foreach (string contentName in mission.GearRewardContentNames)
                {
                    mission.GearRewards.Add(input.ContentManager.Load<Gear>(
                        Path.Combine("Gear", contentName)));
                }

                return mission;
            }
        }


        #endregion


        #region ICloneable Members


        public object Clone()
        {
            Mission mission = new Mission();

            mission.AssetName = AssetName;
            foreach (WorldEntry<Chest> chestEntry in chestEntries)
            {
                WorldEntry<Chest> worldEntry = new WorldEntry<Chest>();
                worldEntry.Content = chestEntry.Content.Clone() as Chest;
                worldEntry.ContentName = chestEntry.ContentName;
                worldEntry.Count = chestEntry.Count;
                worldEntry.Direction = chestEntry.Direction;
                worldEntry.MapContentName = chestEntry.MapContentName;
                worldEntry.MapPosition = chestEntry.MapPosition;
                mission.chestEntries.Add(worldEntry);
            }
            mission.completionMessage = completionMessage;
            mission.description = description;
            mission.destinationMapContentName = destinationMapContentName;
            mission.destinationNpcContentName = destinationNpcContentName;
            mission.destinationObjectiveMessage = destinationObjectiveMessage;
            mission.experienceReward = experienceReward;
            mission.fixedCombatEntries.AddRange(fixedCombatEntries);
            mission.gearRequirements.AddRange(gearRequirements);
            mission.gearRewardContentNames.AddRange(gearRewardContentNames);
            mission.gearRewards.AddRange(gearRewards);
            mission.moneyReward = moneyReward;
            mission.monsterRequirements.AddRange(monsterRequirements);
            mission.name = name;
            mission.objectiveMessage = objectiveMessage;
            mission.stage = stage;

            return mission;
        }


        #endregion
    }
}
