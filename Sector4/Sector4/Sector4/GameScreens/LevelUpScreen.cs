

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Sector4Data;
#endregion

namespace Sector4
{
    /// <summary>
    /// Displays when players that have leveled up
    /// </summary>
    class LevelUpScreen : GameScreen
    {
        private int index;
        private List<Player> leveledUpPlayers;
        private List<RangedWeapon> rangedweaponList = new List<RangedWeapon>();


        #region Graphics content


        private Texture2D backTexture;
        private Texture2D selectIconTexture;
        private Texture2D portraitBackTexture;
        private Texture2D headerTexture;
        private Texture2D lineTexture;
        private Texture2D scrollUpTexture;
        private Texture2D scrollDownTexture;
        private Texture2D fadeTexture;
        private Color color;
        private Color colorName = new Color(241, 173, 10);
        private Color colorClass = new Color(207, 130, 42);
        private Color colorText = new Color(76, 49, 8);


        #endregion


        #region Positions


        private Vector2 backgroundPosition;
        private Vector2 textPosition;
        private Vector2 levelPosition;
        private Vector2 iconPosition;
        private Vector2 linePosition;
        private Vector2 selectPosition;
        private Vector2 selectIconPosition;
        private Vector2 screenSize;
        private Vector2 titlePosition;
        private Vector2 scrollUpPosition;
        private Vector2 scrollDownPosition;
        private Vector2 rangedweaponUpgradePosition;
        private Vector2 portraitPosition;
        private Vector2 playerNamePosition;
        private Vector2 playerLvlPosition;
        private Vector2 playerClassPosition;
        private Vector2 topLinePosition;
        private Vector2 playerDamagePosition;
        private Vector2 headerPosition;
        private Vector2 backPosition;
        private Rectangle fadeDest;


        #endregion


        #region Dialog Strings


        private readonly string titleText = "Level Up";
        private readonly string selectString = "Continue";


        #endregion


        #region Scrolling Text Navigation


        private int startIndex;
        private int endIndex;
        private const int maxLines = 3;
        private const int lineSpacing = 74;


        #endregion


        #region Initialization

        
        /// <summary>
        /// Constructs a new LevelUpScreen object.
        /// </summary>
        /// <param name="leveledUpPlayers"></param>
        public LevelUpScreen(List<Player> leveledUpPlayers)
        {
            if ((leveledUpPlayers == null) || (leveledUpPlayers.Count <= 0))
            {
                throw new ArgumentNullException("leveledUpPlayers");
            }

            this.IsPopup = true;
            this.leveledUpPlayers = leveledUpPlayers;

            index = 0;

            GetRangedWeaponList();

            AudioManager.PushMusic("LevelUp");
            this.Exiting += new EventHandler(LevelUpScreen_Exiting);
        }


        void LevelUpScreen_Exiting(object sender, EventArgs e)
        {
            AudioManager.PopMusic();
        }


        /// <summary>
        /// Load the graphics content
        /// </summary>
        
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            backTexture = 
                content.Load<Texture2D>(@"Textures\GameScreens\PopupScreen");
            selectIconTexture =
                content.Load<Texture2D>(@"Textures\Buttons\facebutton_a");
            portraitBackTexture = 
                content.Load<Texture2D>(@"Textures\GameScreens\PlayerSelected");
            headerTexture = 
                content.Load<Texture2D>(@"Textures\GameScreens\Caption");
            lineTexture =
                content.Load<Texture2D>(@"Textures\GameScreens\SeparationLine");
            scrollUpTexture = 
                content.Load<Texture2D>(@"Textures\GameScreens\ScrollUp");
            scrollDownTexture = 
                content.Load<Texture2D>(@"Textures\GameScreens\ScrollDown");
            fadeTexture = 
                content.Load<Texture2D>(@"Textures\GameScreens\FadeScreen");

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            backgroundPosition.X = (viewport.Width - backTexture.Width) / 2;
            backgroundPosition.Y = (viewport.Height - backTexture.Height) / 2;

            screenSize = new Vector2(viewport.Width, viewport.Height);
            fadeDest = new Rectangle(0, 0, viewport.Width, viewport.Height);

            titlePosition.X = (screenSize.X -
                Fonts.HeaderFont.MeasureString(titleText).X) / 2;
            titlePosition.Y = backgroundPosition.Y + lineSpacing;

            selectIconPosition.X = screenSize.X / 2 + 260;
            selectIconPosition.Y = backgroundPosition.Y + 530f;
            selectPosition.X = selectIconPosition.X -
                Fonts.ButtonNamesFont.MeasureString(selectString).X - 10f;
            selectPosition.Y = selectIconPosition.Y;

            portraitPosition = backgroundPosition + new Vector2(143f, 155f);
            backPosition = backgroundPosition + new Vector2(140f, 135f);

            playerNamePosition = backgroundPosition + new Vector2(230f, 160f);
            playerClassPosition = backgroundPosition + new Vector2(230f, 185f);
            playerLvlPosition = backgroundPosition + new Vector2(230f, 205f);

            topLinePosition = backgroundPosition + new Vector2(380f, 160f);
            textPosition = backgroundPosition + new Vector2(335f, 320f);
            levelPosition = backgroundPosition + new Vector2(540f, 320f);
            iconPosition = backgroundPosition + new Vector2(155f, 303f);
            linePosition = backgroundPosition + new Vector2(142f, 285f);

            scrollUpPosition = backgroundPosition + new Vector2(810f, 300f);
            scrollDownPosition = backgroundPosition + new Vector2(810f, 480f);

            playerDamagePosition = backgroundPosition + new Vector2(560f, 160f);
            rangedweaponUpgradePosition = backgroundPosition + new Vector2(380f, 265f);

            headerPosition = backgroundPosition + new Vector2(120f, 248f);
        }


        #endregion


        #region Updating


        /// <summary>
        /// Handles user input.
        /// </summary>
        public override void HandleInput()
        {
            // exit 
            if (InputManager.IsActionTriggered(InputManager.Action.Back))
            {
                ExitScreen();
            }
            // advance to the next player to have leveled up
            else if (InputManager.IsActionTriggered(InputManager.Action.Ok))
            {
                if (leveledUpPlayers.Count <= 0)
                {
                    // no players at all
                    ExitScreen();
                    return;
                }
                if (index < leveledUpPlayers.Count - 1)
                {
                    // move to the next player
                    index++;
                    GetRangedWeaponList();
                }
                else
                {
                    // no more players
                    ExitScreen();
                    return;
                }
            }
            // Scroll up
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorUp))
            {
                if (startIndex > 0)
                {
                    startIndex--;
                    endIndex--;
                }
            }
            // Scroll down
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorDown))
            {
                if (startIndex < rangedweaponList.Count - maxLines)
                {
                    endIndex++;
                    startIndex++;
                }
            }
        }


        /// <summary>
        /// Get the rangedweapon list
        /// </summary>
        private void GetRangedWeaponList()
        {
            rangedweaponList.Clear();

            if ((leveledUpPlayers.Count > 0) &&
                (leveledUpPlayers[index].CharacterLevel <=
                    leveledUpPlayers[index].CharacterClass.LevelEntries.Count))
            {
                List<RangedWeapon> newRangedWeapons = 
                    leveledUpPlayers[index].CharacterClass.LevelEntries[
                        leveledUpPlayers[index].CharacterLevel - 1].RangedWeapons;
                if ((newRangedWeapons == null) || (newRangedWeapons.Count <= 0))
                {
                    startIndex = 0;
                    endIndex = 0;
                }
                else
                {
                    rangedweaponList.AddRange(leveledUpPlayers[index].RangedWeapons);
                    rangedweaponList.RemoveAll(delegate(RangedWeapon rangedweapon)
                    {
                        return !newRangedWeapons.Exists(delegate(RangedWeapon newRangedWeapon)
                        {
                            return rangedweapon.AssetName == newRangedWeapon.AssetName;
                        });
                    });
                    startIndex = 0;
                    endIndex = Math.Min(maxLines, rangedweaponList.Count);
                }
            }
            else
            {
                startIndex = 0;
                endIndex = 0;
            }
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draw the screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Vector2 currentTextPosition = textPosition;
            Vector2 currentIconPosition = iconPosition;
            Vector2 currentLinePosition = linePosition;
            Vector2 currentLevelPosition = levelPosition;

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // Draw the fading screen
            spriteBatch.Draw(fadeTexture, fadeDest, Color.White);

            // Draw the popup background
            spriteBatch.Draw(backTexture, backgroundPosition, Color.White);

            // Draw the title
            spriteBatch.DrawString(Fonts.HeaderFont, titleText, titlePosition,
                Fonts.TitleColor);

            DrawPlayerStats();

             //Draw the rangedweapon upgrades caption
            spriteBatch.Draw(headerTexture, headerPosition, Color.White);
            spriteBatch.DrawString(Fonts.PlayerNameFont, "You may now use",
                rangedweaponUpgradePosition, colorClass);

            // Draw the horizontal separating lines
            for (int i = 0; i <= maxLines - 1; i++)
            {
                currentLinePosition.Y += lineSpacing;
                spriteBatch.Draw(lineTexture, currentLinePosition, Color.White);
            }

            // Draw the rangedweapon upgrade details
            for (int i = startIndex; i < endIndex; i++)
            {
                // Draw the rangedweapon icon
                spriteBatch.Draw(rangedweaponList[i].IconTexture, currentIconPosition,
                    Color.White);

                // Draw the rangedweapon name
                spriteBatch.DrawString(Fonts.GearInfoFont, rangedweaponList[i].Name,
                    currentTextPosition, Fonts.CountColor);

                // Draw the rangedweapon level
                spriteBatch.DrawString(Fonts.GearInfoFont, "Weapon level " +
                    rangedweaponList[i].Level.ToString(),
                    currentLevelPosition, Fonts.CountColor);

                // Increment to next line position
                currentTextPosition.Y += lineSpacing;
                currentLevelPosition.Y += lineSpacing;
                currentIconPosition.Y += lineSpacing;
            }

            // Draw the scroll bars
            spriteBatch.Draw(scrollUpTexture, scrollUpPosition, Color.White);
            spriteBatch.Draw(scrollDownTexture, scrollDownPosition, Color.White);

            // Draw the select button and its corresponding text
            spriteBatch.DrawString(Fonts.ButtonNamesFont, selectString, selectPosition,
                Color.White);
            spriteBatch.Draw(selectIconTexture, selectIconPosition, Color.White);

            spriteBatch.End();
        }


        /// <summary>
        /// Draw the player stats here
        /// </summary>
        private void DrawPlayerStats()
        {
            Vector2 position = topLinePosition;
            Vector2 posDamage = playerDamagePosition;
            Player player = leveledUpPlayers[index];
            int level = player.CharacterLevel;
            CharacterLevelingStatistics levelingStatistics =
                player.CharacterClass.LevelingStatistics;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Draw the portrait            
            spriteBatch.Draw(portraitBackTexture, backPosition, Color.White);

            spriteBatch.Draw(player.ActivePortraitTexture, portraitPosition,
                Color.White);

            // Print the character name
            spriteBatch.DrawString(Fonts.PlayerNameFont,
                player.Name, playerNamePosition, colorName);

            // Draw the Class Name
            spriteBatch.DrawString(Fonts.PlayerNameFont,
                player.CharacterClass.Name, playerClassPosition, colorClass);

            // Draw the character level
            spriteBatch.DrawString(Fonts.PlayerNameFont, "LEVEL: " +
                level.ToString(), playerLvlPosition, Color.Gray);

            // Draw the character Health Points
            SetColor(levelingStatistics.LevelsPerHealthPointsIncrease == 0 ? 0 :
                (level % levelingStatistics.LevelsPerHealthPointsIncrease) *
                levelingStatistics.HealthPointsIncrease);
            spriteBatch.DrawString(Fonts.PlayerStatisticsFont, "HP: " +
                player.CurrentStatistics.HealthPoints + "/" +
                player.CharacterStatistics.HealthPoints,
                position, color);

            // Draw the character Ammo Points
            position.Y += Fonts.GearInfoFont.LineSpacing;
            SetColor(levelingStatistics.LevelsPerAmmoPointsIncrease == 0 ? 0 :
                (level % levelingStatistics.LevelsPerAmmoPointsIncrease) *
                levelingStatistics.AmmoPointsIncrease);
            spriteBatch.DrawString(Fonts.PlayerStatisticsFont, "Ammo: " +
                player.CurrentStatistics.AmmoPoints + "/" +
                player.CharacterStatistics.AmmoPoints,
                position, color);

            // Draw the physical offense
            SetColor(levelingStatistics.LevelsPerPhysicalOffenseIncrease == 0 ? 0 :
                (level % levelingStatistics.LevelsPerPhysicalOffenseIncrease) *
                levelingStatistics.PhysicalOffenseIncrease);
            spriteBatch.DrawString(Fonts.PlayerStatisticsFont, "Phys Off: " +
                player.CurrentStatistics.PhysicalOffense, posDamage, color);

            // Draw the physical defense
            posDamage.Y += Fonts.PlayerStatisticsFont.LineSpacing;
            SetColor(levelingStatistics.LevelsPerPhysicalDefenseIncrease == 0 ? 0 :
                (level % levelingStatistics.LevelsPerPhysicalDefenseIncrease) *
                levelingStatistics.PhysicalDefenseIncrease);
            spriteBatch.DrawString(Fonts.PlayerStatisticsFont, "Phys Def: " +
                player.CurrentStatistics.PhysicalDefense, posDamage, color);

            
        }


       
        public void SetColor(int value)
        {
            if (value > 0)
            {
                color = Color.Green;
            }
            else if (value < 0)
            {
                color = Color.Red;
            }
            else
            {
                color = colorText;
            }
        }


        #endregion
    }
}