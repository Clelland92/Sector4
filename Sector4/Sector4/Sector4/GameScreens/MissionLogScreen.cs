

#region Using Statements
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Sector4Data;
#endregion

namespace Sector4
{
    /// <summary>
    /// Displays all of the missions completed 
    /// </summary>
    class MissionLogScreen : ListScreen<Mission>
    {
        #region Initial Detail Mission


        
        private Mission initialDetailMission;


        #endregion


        #region Columns


        protected string nameColumnText = "Name";
        private const int nameColumnInterval = 20;

        protected string stageColumnText = "Stage";
        private const int stageColumnInterval = 450;


        #endregion


        #region Data Access


        /// <summary>
        /// Get the list that this screen displays.
        /// </summary>
        public override ReadOnlyCollection<Mission> GetDataList()
        {
            List<Mission> missions = new List<Mission>();
            for (int i = 0; i <= Session.CurrentMissionIndex; i++)
            {
                if (i < Session.MissionLine.Missions.Count)
                {
                    missions.Add(Session.MissionLine.Missions[i]);
                }
            }

            return missions.AsReadOnly();
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Creates a new EquipmentScreen object for the  player.
        /// </summary>
        public MissionLogScreen(Mission initialDetailMission) 
            : base()
        {
            
            this.initialDetailMission = initialDetailMission;

            // configure the menu text
            titleText = Session.MissionLine.Name;
            selectButtonText = "Select";
            backButtonText = "Back";
            xButtonText = String.Empty;
            yButtonText = String.Empty;
            leftTriggerText = "Equipment";
            rightTriggerText = "Statistics";

            // select the current mission
            SelectedIndex = Session.CurrentMissionIndex;
        }


        #endregion


        #region Input Handling


        /// <summary>
        /// Handle user input.
        /// </summary>
        public override void HandleInput()
        {
            // open the initial MissionDetailScreen, if any
           
            if (initialDetailMission != null)
            {
                ScreenManager.AddScreen(new MissionDetailsScreen(initialDetailMission));
                // if the selected mission is in the list
                SelectedIndex = Session.MissionLine.Missions.IndexOf(initialDetailMission);
                // only open the screen once
                initialDetailMission = null;
            }
            
            base.HandleInput();
        }


        /// <summary>
        /// Respond to the pressing of the X button (and related key).
        /// </summary>
        protected override void SelectTriggered(Mission entry)
        {
            ScreenManager.AddScreen(new MissionDetailsScreen(entry));
        }


        /// <summary>
        /// Switch to the screen to the "left"
        /// </summary>
        protected override void PageScreenLeft()
        {
            ExitScreen();
            ScreenManager.AddScreen(new InventoryScreen(false));
        }


        /// <summary>
        /// Switch to the screen to the "right" 
        /// </summary>
        protected override void PageScreenRight()
        {
            ExitScreen();
            ScreenManager.AddScreen(new StatisticsScreen(Session.Party.Players[0]));
        }

        
        #endregion


        #region Drawing


        
        protected override void DrawEntry(Mission entry, Vector2 position,
            bool isSelected)
        {
            // check the parameter
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 drawPosition = position;

            // draw the name
            Color color = isSelected ? Fonts.HighlightColor : Fonts.DisplayColor;
            drawPosition.Y += listLineSpacing / 4;
            drawPosition.X += nameColumnInterval;
            spriteBatch.DrawString(Fonts.GearInfoFont, entry.Name, drawPosition, color);

            // draw the stage
            drawPosition.X += stageColumnInterval;
            string stageText = String.Empty;
            switch (entry.Stage)
            {
                case Mission.MissionStage.Completed:
                    stageText = "Completed";
                    break;

                case Mission.MissionStage.InProgress:
                    stageText = "In Progress";
                    break;

                case Mission.MissionStage.NotStarted:
                    stageText = "Not Started";
                    break;

                case Mission.MissionStage.RequirementsMet:
                    stageText = "Requirements Met";
                    break;
            }
            spriteBatch.DrawString(Fonts.GearInfoFont, stageText, drawPosition, color);

            // turn on or off the select button
            if (isSelected)
            {
                selectButtonText = "Select";
            }
        }


        /// <summary>
        /// Draw the description of the selected item.
        /// </summary>
        protected override void DrawSelectedDescription(Mission entry) { }


        /// <summary>
        /// Draw the column headers above the list.
        /// </summary>
        protected override void DrawColumnHeaders()
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 position = listEntryStartPosition;

            position.X += nameColumnInterval;
            if (!String.IsNullOrEmpty(nameColumnText))
            {
                spriteBatch.DrawString(Fonts.CaptionFont, nameColumnText, position,
                    Fonts.CaptionColor);
            }

            position.X += stageColumnInterval;
            if (!String.IsNullOrEmpty(stageColumnText))
            {
                spriteBatch.DrawString(Fonts.CaptionFont, stageColumnText, position,
                    Fonts.CaptionColor);
            }
        }


        #endregion
    }
}
