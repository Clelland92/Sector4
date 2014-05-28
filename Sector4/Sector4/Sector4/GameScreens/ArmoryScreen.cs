

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
    /// Lists the rangedweapons available to the player.
    /// </summary>
    class ArmoryScreen : ListScreen<RangedWeapon>
    {
        #region Graphics Data


        private readonly Vector2 rangedweaponDescriptionPosition = new Vector2(200, 550);
        private readonly Vector2 warningMessagePosition = new Vector2(200, 580);


        #endregion


        #region Columns


        private string nameColumnText = "Name";
        private const int nameColumnInterval = 80;

        private string levelColumnText = "Level";
        private const int levelColumnInterval = 240;

        private string powerColumnText = "Power (min, max)";
        private const int powerColumnInterval = 110;

        //private string ammoCostColumnText = "Ammo";
        private const int ammoCostColumnInterval = 380;


        #endregion


        #region Data Access


        /// <summary>
        /// The FightingCharacter object whose rangedweapons are displayed.
        /// </summary>
        private FightingCharacter fightingCharacter;


        /// <summary>
        /// The statistics of the character, for calculating the eligibility of rangedweapons.
        /// </summary>
        /// <remarks>
        /// Needed because combat statistics override character statistics.
        /// </remarks>
        private StatisticsValue statistics;


        /// <summary>
        /// Get the list that this screen displays.
        /// </summary>
        public override ReadOnlyCollection<RangedWeapon> GetDataList()
        {
            return fightingCharacter.RangedWeapons.AsReadOnly();
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Creates a new ArmoryScreen object for the given player and statistics.
        /// </summary>
        public ArmoryScreen(FightingCharacter fightingCharacter,
            StatisticsValue statistics)
            : base()
        {
            // check the parameter
            if (fightingCharacter == null)
            {
                throw new ArgumentNullException("fightingCharacter");
            }
            this.fightingCharacter = fightingCharacter;
            this.statistics = statistics;

            // sort the player's rangedweapon
            this.fightingCharacter.RangedWeapons.Sort(
                delegate(RangedWeapon rangedweapon1, RangedWeapon rangedweapon2)
                {
                    // handle null values
                    if (rangedweapon1 == null)
                    {
                        return (rangedweapon2 == null ? 0 : 1);
                    }
                    else if (rangedweapon2 == null)
                    {
                        return -1;
                    }

                    // sort by name
                    return rangedweapon1.Name.CompareTo(rangedweapon2.Name);
                });

            // configure the menu text
            titleText = "Guns";
            selectButtonText = "Shoot";
            backButtonText = "Back";
            xButtonText = String.Empty;
            yButtonText = String.Empty;
            leftTriggerText = String.Empty;
            rightTriggerText = String.Empty;
        }


        #endregion


        #region Input Handling


        /// <summary>
        /// Delegate for rangedweapon-selection events.
        /// </summary>
        public delegate void RangedWeaponSelectedHandler(RangedWeapon rangedweapon);


        /// <summary>
        /// Responds when an rangedweapon is selected by this menu.
        /// </summary>
        /// <remarks>
        /// Typically used by the calling menu, like the combat HUD menu, 
        /// to respond to selection.
        /// </remarks>
        public event RangedWeaponSelectedHandler RangedWeaponSelected;


        /// <summary>
        /// Respond to the triggering of the Select action (and related key).
        /// </summary>
        protected override void SelectTriggered(RangedWeapon entry)
        {
            // check the parameter
            if (entry == null)
            {
                return;
            }

            // make sure the rangedweapon can be selected
            if (!CanSelectEntry(entry))
            {
                return;
            }

            // if the event is valid, fire it and exit this screen
            if (RangedWeaponSelected != null)
            {
                RangedWeaponSelected(entry);
                ExitScreen();
                return;
            }
        }


        /// <summary>
        /// Returns true if the specified rangedweapon can be selected.
        /// </summary>
        private bool CanSelectEntry(RangedWeapon entry)
        {
            if (entry == null)
            {
                return false;
            }

            return (statistics.AmmoPoints >= entry.AmmoPointCost) &&
                (!entry.IsOffensive || CombatEngine.IsActive);
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draw the rangedweapon at the given position in the list.
        /// </summary>
        
        protected override void DrawEntry(RangedWeapon entry, Vector2 position,
            bool isSelected)
        {
            // check the parameter
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 drawPosition = position;
            Color color = isSelected ? Fonts.HighlightColor : Fonts.DisplayColor;

            // draw the icon
            spriteBatch.Draw(entry.IconTexture, drawPosition + iconOffset, Color.White);

            // draw the name
            drawPosition.Y += listLineSpacing / 4;
            drawPosition.X += nameColumnInterval;
            spriteBatch.DrawString(Fonts.GearInfoFont, entry.Name, drawPosition, color);

            // draw the level
            drawPosition.X += levelColumnInterval;
            spriteBatch.DrawString(Fonts.GearInfoFont, entry.Level.ToString(), 
                drawPosition, color);
            
            // draw the power
            drawPosition.X += powerColumnInterval;
            string powerText = entry.GetPowerText();
            Vector2 powerTextSize = Fonts.GearInfoFont.MeasureString(powerText);
            Vector2 powerPosition = drawPosition;
            powerPosition.Y -= (float)Math.Ceiling((powerTextSize.Y - 30f) / 2);
            spriteBatch.DrawString(Fonts.GearInfoFont, powerText,
                powerPosition, color);

            // draw the quantity
            drawPosition.X += ammoCostColumnInterval;
            spriteBatch.DrawString(Fonts.GearInfoFont, entry.AmmoPointCost.ToString(),
                drawPosition, color);

            // draw the shoot button if needed
            if (isSelected)
            {
                selectButtonText = (CanSelectEntry(entry) && (RangedWeaponSelected != null)) ?
                    "Shoot" : String.Empty;
            }
        }


        /// <summary>
        /// Draw the description of the selected item.
        /// </summary>
        protected override void DrawSelectedDescription(RangedWeapon entry)
        {
            // check the parameter
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 position = descriptionTextPosition;

            // draw the insufficient-mp warning
            if (CombatEngine.IsActive && (entry.AmmoPointCost > statistics.AmmoPoints))
            {
                // draw the insufficient-mp warning
                spriteBatch.DrawString(Fonts.DescriptionFont,
                   "Not enough Ammo to fire weapon", warningMessagePosition,
                   Color.Red);
            }

            // draw the description
            spriteBatch.DrawString(Fonts.DescriptionFont, 
                Fonts.BreakTextIntoLines(entry.Description, 90, 3), 
                rangedweaponDescriptionPosition, Fonts.DescriptionColor);
        }


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

            position.X += levelColumnInterval;
            if (!String.IsNullOrEmpty(levelColumnText))
            {
                spriteBatch.DrawString(Fonts.CaptionFont, levelColumnText, position,
                    Fonts.CaptionColor);
            }

            position.X += powerColumnInterval;
            if (!String.IsNullOrEmpty(powerColumnText))
            {
                spriteBatch.DrawString(Fonts.CaptionFont, powerColumnText, position,
                    Fonts.CaptionColor);
            }

            
        }


        #endregion
    }
}