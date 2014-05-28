

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
    /// Draws the statistics for the Player.
    /// </summary>
    class StatisticsScreen : GameScreen
    {
        
        private Player player;


        #region Graphics Data


        private Texture2D statisticsScreen;
        
        private Texture2D backButton;
        private Texture2D dropButton;
        private Texture2D statisticsBorder;
        private Texture2D plankTexture;
        private Texture2D fadeTexture;
       
        private Texture2D scoreBoardTexture;
        private Texture2D rightTriggerButton;
        private Texture2D leftTriggerButton;
        
        private Texture2D heroPortrait;

        private Rectangle screenRectangle;
        private const int intervalBetweenEachInfo = 40;
        private int playerIndex = 0;
        


        #endregion


        #region Positions


        private readonly Vector2 playerTextPosition = new Vector2(515, 200);
        private Vector2 currentTextPosition;
        private readonly Vector2 scoreBoardPosition = new Vector2(1160, 354);
        private Vector2 placeTextMid;
        private Vector2 statisticsNamePosition;
        private readonly Vector2 shieldPosition = new Vector2(1124, 253);
        
        private readonly Vector2 dropButtonPosition = new Vector2(1150, 640);
        private readonly Vector2 statisticsBorderPosition = new Vector2(180, 147);
        
        private readonly Vector2 characterNamePosition = new Vector2(330, 180);
        private readonly Vector2 classNamePosition = new Vector2(330, 465);
        private Vector2 plankPosition;
        
        private readonly Vector2 weaponPosition = new Vector2(790, 220);
        private readonly Vector2 armorPosition = new Vector2(790, 346);
        private readonly Vector2 weaponTextPosition = new Vector2(790, 285);
        private readonly Vector2 leftTriggerPosition = new Vector2(340, 50);
        private readonly Vector2 rightTriggerPosition = new Vector2(900, 50);
        private readonly Vector2 iconPosition = new Vector2(100, 200);
        
        private readonly Vector2 backButtonPosition = new Vector2(80, 640);
        
        private readonly Vector2 heroPortraitPosition = new Vector2(200, 213);

        #endregion


        #region Initialization


        
        public StatisticsScreen() : this(Session.Party.Players[0]) { }


        /// <summary>
        /// Creates a new StatisticsScreen object for the given player.
        /// </summary>
        public StatisticsScreen(Player player)
            : base()
        {
            this.IsPopup = true;
            this.player = player;

           
        }


        /// <summary>
        /// Loads graphics content from the content manager.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            statisticsScreen =
                content.Load<Texture2D>(@"Textures\GameScreens\GameScreenBkgd");
            plankTexture =
                content.Load<Texture2D>(@"Textures\MainMenu\MainMenuPlank");
            scoreBoardTexture =
                content.Load<Texture2D>(@"Textures\GameScreens\CountShieldWithArrow");
            leftTriggerButton =
                content.Load<Texture2D>(@"Textures\Buttons\left_trigger");
            rightTriggerButton =
                content.Load<Texture2D>(@"Textures\Buttons\right_trigger");
            backButton =
                content.Load<Texture2D>(@"Textures\Buttons\facebutton_b");
            
            dropButton =
                content.Load<Texture2D>(@"Textures\Buttons\facebutton_y");
            statisticsBorder =
                content.Load<Texture2D>(@"Textures\GameScreens\StatsBorderTable");
            
            fadeTexture =
                content.Load<Texture2D>(@"Textures\GameScreens\FadeScreen");
            heroPortrait =
                content.Load<Texture2D>(@"Textures\GameScreens\HeroPortrait");
            

            screenRectangle = new Rectangle(viewport.X, viewport.Y,
                viewport.Width, viewport.Height);

            statisticsNamePosition.X = (viewport.Width -
                Fonts.HeaderFont.MeasureString("Statistics").X) / 2;
            statisticsNamePosition.Y = 90f;

            plankPosition.X = (viewport.Width - plankTexture.Width) / 2;
            plankPosition.Y = 67f;
        }


        #endregion


        #region Updating


        /// <summary>
        /// Handle user input.
        /// </summary>
        public override void HandleInput()
        {
            // exit the screen
            if (InputManager.IsActionTriggered(InputManager.Action.Back))
            {
                ExitScreen();
                return;
            }
            // shows the rangedweapons for this player
            else if (InputManager.IsActionTriggered(InputManager.Action.Ok))
            {
                ScreenManager.AddScreen(new ArmoryScreen(player,
                    player.CharacterStatistics));
                return;
            }
            // show the equipment for this player, allowing the user to unequip
            else if (InputManager.IsActionTriggered(InputManager.Action.TakeView))
            {
                ScreenManager.AddScreen(new EquipmentScreen(player));
                return;
            }
            else if (Session.Party.Players.Contains(player)) // player is in the party
            {
                // move to the previous screen
                if (InputManager.IsActionTriggered(InputManager.Action.PageLeft))
                {
                    ExitScreen();
                    ScreenManager.AddScreen(new MissionLogScreen(null));
                    return;
                }
                // move to the next screen
                else if (InputManager.IsActionTriggered(InputManager.Action.PageRight))
                {
                    ExitScreen();
                    ScreenManager.AddScreen(new InventoryScreen(true));
                    return;
                }
                // move to the previous party member
                else if (InputManager.IsActionTriggered(InputManager.Action.CursorUp))
                {
                    playerIndex--;
                    if (playerIndex < 0)
                    {
                        playerIndex = Session.Party.Players.Count - 1;
                    }
                    Player newPlayer = Session.Party.Players[playerIndex];
                    if (newPlayer != player)
                    {
                        player = newPlayer;
                        
                    }
                }
                // move to the next party member
                else if (InputManager.IsActionTriggered(InputManager.Action.CursorDown))
                {
                    playerIndex++;
                    if (playerIndex >= Session.Party.Players.Count)
                    {
                        playerIndex = 0;
                    }
                    Player newPlayer = Session.Party.Players[playerIndex];
                    if (newPlayer != player)
                    {
                        player = newPlayer;
                        
                    }
                }
            }
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draws the screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            
            ScreenManager.SpriteBatch.Begin();
            DrawStatistics();
            ScreenManager.SpriteBatch.End();
        }


        /// <summary>
        /// Draws the player statistics.
        /// </summary>
        private void DrawStatistics()
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Draw faded screen
            spriteBatch.Draw(fadeTexture, screenRectangle, Color.White);

            // Draw the Statistics Screen
            spriteBatch.Draw(statisticsScreen, screenRectangle, Color.White);
            spriteBatch.Draw(plankTexture, plankPosition, Color.White);
            spriteBatch.Draw(statisticsBorder, statisticsBorderPosition, Color.White);

            

            spriteBatch.DrawString(Fonts.HeaderFont, "Statistics",
                statisticsNamePosition, Fonts.TitleColor);
            DrawPlayerDetails();
            DrawButtons();
        }


        /// <summary>
        /// D
        /// </summary>
        private void DrawButtons()
        {
            if (!IsActive)
            {
                return;
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 position = new Vector2();

            if (Session.Party.Players.Contains(player))
            {
                // Left Trigger
                position = leftTriggerPosition;
                spriteBatch.Draw(leftTriggerButton, position, Color.White);

                // Draw Left Trigger Information
                position.Y += leftTriggerButton.Height;
                placeTextMid = Fonts.PlayerStatisticsFont.MeasureString("Mission");
                position.X += (leftTriggerButton.Width / 2) - placeTextMid.X / 2;
                spriteBatch.DrawString(Fonts.PlayerStatisticsFont, "Mission", position,
                    Color.Black);

                // Right Trigger
                position = rightTriggerPosition;
                spriteBatch.Draw(rightTriggerButton, position, Color.White);

                // Draw Right Trigger Information
                position.Y += rightTriggerButton.Height;
                placeTextMid = Fonts.PlayerStatisticsFont.MeasureString("Items");
                position.X += (leftTriggerButton.Width / 2) - placeTextMid.X / 2;
                spriteBatch.DrawString(Fonts.PlayerStatisticsFont, "Items", position,
                    Color.Black);
            }

            // Back Button
            spriteBatch.Draw(backButton, backButtonPosition, Color.White);

            

            // Draw Back
            position = backButtonPosition;
            position.X += backButton.Width + 10f;
            position.Y += 5;
            spriteBatch.DrawString(Fonts.ButtonNamesFont, "Back", position,
                Color.Black);

            // Draw drop Button    
            spriteBatch.Draw(dropButton, dropButtonPosition, Color.White);
            position = dropButtonPosition;
            position.X -= Fonts.ButtonNamesFont.MeasureString("Equipment").X + 10f;
            position.Y += 5;
            spriteBatch.DrawString(Fonts.ButtonNamesFont, "Equipment", position,
                Color.Black);

            // Draw drop Button    
            spriteBatch.Draw(heroPortrait, heroPortraitPosition, Color.White);
            position = heroPortraitPosition;
            position.X -= Fonts.ButtonNamesFont.MeasureString("Equipment").X + 10f;
            position.Y += 5;
           
            
        }


        /// <summary>
        /// Draws player information.
        /// </summary>
        private void DrawPlayerDetails()
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            if (player != null)
            {
                currentTextPosition.X = playerTextPosition.X;
                currentTextPosition.Y = playerTextPosition.Y;

                // Current Level
                spriteBatch.DrawString(Fonts.DescriptionFont, "Level: " +
                    player.CharacterLevel,
                    currentTextPosition, Color.White);

                // Health Points
                currentTextPosition.Y += intervalBetweenEachInfo;
                spriteBatch.DrawString(Fonts.DescriptionFont, "Health Points: " +
                    player.CurrentStatistics.HealthPoints + "/" +
                    player.CharacterStatistics.HealthPoints,
                    currentTextPosition, Color.White);

                

                // Experience Details
                currentTextPosition.Y += intervalBetweenEachInfo;
                if (player.IsMaximumCharacterLevel)
                {
                    spriteBatch.DrawString(Fonts.DescriptionFont, "Experience: Maximum",
                        currentTextPosition, Color.White);
                }
                else
                {
                    spriteBatch.DrawString(Fonts.DescriptionFont, "Experience: " +
                        player.Experience + "/" +
                        player.ExperienceForNextLevel,
                        currentTextPosition, Color.White);
                }

                DrawEquipmentInfo(player);

                DrawModifiers(player);

                DrawEquipmentStatistics(player);

                if (player == null)
                {
                    DrawPlayerCount();
                }
            }
            
        }


        /// <summary>
        /// Draws Current Selected Player Count to Total Player count
        /// </summary>
        private void DrawPlayerCount()
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 position = new Vector2();

            // Draw the ScoreBoard
            spriteBatch.Draw(scoreBoardTexture, shieldPosition,
                Color.White);

            position = scoreBoardPosition;
            position.X = scoreBoardPosition.X -
                Fonts.GearInfoFont.MeasureString((playerIndex + 1).ToString()).X / 2;

            // Draw Current Selected Player Count
            spriteBatch.DrawString(Fonts.GearInfoFont, (playerIndex + 1).ToString(),
                position, Fonts.CountColor);

            position.X = scoreBoardPosition.X - Fonts.GearInfoFont.MeasureString(
                Session.Party.Players.Count.ToString()).X / 2;
            position.Y += 30;

            // Draw Total Player count
            spriteBatch.DrawString(Fonts.GearInfoFont,
                Session.Party.Players.Count.ToString(), position, Fonts.CountColor);
        }


        /// <summary>
        /// Draw Equipment Info of the player selected 
        /// </summary>
        /// <param name="selectedPlayer">The selected player</param>
        private void DrawEquipmentInfo(Player selectedPlayer)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            // Character name
            currentTextPosition = characterNamePosition;

            currentTextPosition.X -=
                Fonts.HeaderFont.MeasureString(selectedPlayer.Name).X / 2;
            spriteBatch.DrawString(Fonts.HeaderFont, selectedPlayer.Name,
                currentTextPosition, Fonts.TitleColor);

            // Class name
            currentTextPosition = classNamePosition;
            currentTextPosition.X -= Fonts.GearInfoFont.MeasureString(
                selectedPlayer.CharacterClass.Name).X / 2;
            spriteBatch.DrawString(Fonts.GearInfoFont,
                selectedPlayer.CharacterClass.Name, currentTextPosition, Color.Black);
        }


        /// <summary>
        /// Draw Base Amount Plus any Modifiers
        /// </summary>
        /// <param name="selectedPlayer">The selected player</param>
        private void DrawModifiers(Player selectedPlayer)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            currentTextPosition.X = playerTextPosition.X;
            currentTextPosition.Y = playerTextPosition.Y + 150;

            // PO + Modifiers
            spriteBatch.DrawString(Fonts.DescriptionFont, "Physical Offense: " +
                selectedPlayer.CurrentStatistics.PhysicalOffense,
                currentTextPosition, Color.White);

            // PD + Modifiers
            currentTextPosition.Y += intervalBetweenEachInfo;
            spriteBatch.DrawString(Fonts.DescriptionFont, "Physical Defense: " +
                selectedPlayer.CurrentStatistics.PhysicalDefense,
                currentTextPosition, Color.White);

            
        }


        /// <summary>
        /// Draw the equipment statistics
        /// </summary>
        /// <param name="selectedPlayer">The selected Player</param>
        private void DrawEquipmentStatistics(Player selectedPlayer)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            Vector2 position = weaponPosition;
            Int32Range healthDamageRange = new Int32Range();
            healthDamageRange.Minimum = healthDamageRange.Maximum =
                selectedPlayer.CurrentStatistics.PhysicalOffense;
            Weapon weapon = selectedPlayer.GetEquippedWeapon();
            if (weapon != null)
            {
                weapon.DrawIcon(ScreenManager.SpriteBatch, position);
                healthDamageRange += weapon.TargetDamageRange;
            }

            position = armorPosition;
            Int32Range healthDefenseRange = new Int32Range();
            healthDefenseRange.Minimum = healthDefenseRange.Maximum =
                selectedPlayer.CurrentStatistics.PhysicalDefense;
            Int32Range ammoDefenseRange = new Int32Range();
            ammoDefenseRange.Minimum = ammoDefenseRange.Maximum =
                selectedPlayer.CurrentStatistics.AmmoalDefense;
            for (int i = 0; i < (int)4; i++)
            {
                Armor armor = selectedPlayer.GetEquippedArmor((Armor.ArmorSlot)i);
                if (armor != null)
                {
                    armor.DrawIcon(ScreenManager.SpriteBatch, position);
                    healthDefenseRange += armor.OwnerHealthDefenseRange;
                    ammoDefenseRange += armor.OwnerAmmoDefenseRange;
                }
                position.X += 68;
            }

            position = weaponTextPosition;
            spriteBatch.DrawString(Fonts.DescriptionFont, "Weapon Attack: " + "(" +
                healthDamageRange.Minimum + "," +
                healthDamageRange.Maximum + ")",
                position, Color.White);

            position.Y += 130;
            spriteBatch.DrawString(Fonts.DescriptionFont, "Weapon Defense: " + "(" +
                healthDefenseRange.Minimum + "," +
                healthDefenseRange.Maximum + ")",
                position, Color.White);

            
        }


        #endregion
    }
}