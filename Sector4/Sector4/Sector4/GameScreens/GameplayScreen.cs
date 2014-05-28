

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sector4Data;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Sector4
{
    
    class GameplayScreen : GameScreen
    {
        #region Initialization

        GameStartDescription gameStartDescription = null;
        //SaveGameDescription saveGameDescription = null;


        /// <summary>
        /// Create a new GameplayScreen 
        /// </summary>
        private GameplayScreen()
            : base()
        {
            CombatEngine.ClearCombat();
            this.Exiting += new EventHandler(GameplayScreen_Exiting);
        }


        
        public GameplayScreen(GameStartDescription gameStartDescription) 
            : this()
        {
            this.gameStartDescription = gameStartDescription;
            //this.saveGameDescription = null;
        }


        
        
        

        /// <summary>
        /// Handle the closing of this screen.
        /// </summary>
        void GameplayScreen_Exiting(object sender, EventArgs e)
        {
            
            Session.EndSession();
        }
        
        
        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (gameStartDescription != null)
            {
                Session.StartNewSession(gameStartDescription, ScreenManager, this);
            }
            //else if (saveGameDescription != null)
            {
                //Session.LoadSession(saveGameDescription, ScreenManager, this);
            }

            
            ScreenManager.Game.ResetElapsedTime();
        }


        #endregion


        #region Update and Draw


        
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive && !coveredByOtherScreen)
            {
                Session.Update(gameTime);
            }
        }


        /// <summary>
        /// Lets the game respond to player input. 
        /// </summary>
        public override void HandleInput()
        {
            if (InputManager.IsActionTriggered(InputManager.Action.MainMenu))
            {
                ScreenManager.AddScreen(new MainMenuScreen());
                return;
            }

            if (InputManager.IsActionTriggered(InputManager.Action.ExitGame))
            {
                // add a confirmation message box
                const string message = 
                    "Are you sure you want to exit? ";
                MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);
                confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
                ScreenManager.AddScreen(confirmExitMessageBox);
                return;
            }

            if (!CombatEngine.IsActive &&
                InputManager.IsActionTriggered(InputManager.Action.CharacterManagement))
            {
                ScreenManager.AddScreen(new StatisticsScreen(Session.Party.Players[0]));
                return;
            }
        }


        /// <summary>
        /// Event handler for when the user selects Yes 
        /// on the "Are you sure?" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }                


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Session.Draw(gameTime);
        }


        #endregion
    }
}
