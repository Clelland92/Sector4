

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sector4Data;
#endregion

namespace Sector4
{
    /// <summary>
    /// The main menu screen 
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Graphics Data


        private Texture2D backgroundTexture;
        private Vector2 backgroundPosition;

        
        
        private Texture2D backTexture;
        private Vector2 backPosition;

        private Texture2D selectTexture;
        private Vector2 selectPosition;

        private Texture2D plankTexture1, plankTexture2, plankTexture3;


        #endregion


        #region Menu Entries


        MenuEntry newGameMenuEntry, exitGameMenuEntry; 
        
        MenuEntry controlsMenuEntry, creditsMenuEntry;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base()
        {
            // add the New Game entry
            newGameMenuEntry = new MenuEntry("Start Game");
            
            newGameMenuEntry.Font = Fonts.HeaderFont;
            newGameMenuEntry.Position = new Vector2(500, 0f);
            newGameMenuEntry.Selected += NewGameMenuEntrySelected;
            MenuEntries.Add(newGameMenuEntry);

            

            // add the Controls menu entry
            controlsMenuEntry = new MenuEntry("Controls");
            //controlsMenuEntry.Description = "View Game Controls";
            controlsMenuEntry.Font = Fonts.HeaderFont;
            controlsMenuEntry.Position = new Vector2(500, 0f);
            controlsMenuEntry.Selected += ControlsMenuEntrySelected;
            MenuEntries.Add(controlsMenuEntry);

            // add the Help menu entry
            creditsMenuEntry = new MenuEntry("Credits");
            //helpMenuEntry.Description = "View Game Help";
            creditsMenuEntry.Font = Fonts.HeaderFont;
            creditsMenuEntry.Position = new Vector2(500, 0f);
            creditsMenuEntry.Selected += CreditMenuEntrySelected;
            MenuEntries.Add(creditsMenuEntry);
            

            // create the Exit menu entry
            exitGameMenuEntry = new MenuEntry("Exit");
            exitGameMenuEntry.Description = "Quit the Game";
            exitGameMenuEntry.Font = Fonts.HeaderFont;
            exitGameMenuEntry.Position = new Vector2(500, 0f);
            exitGameMenuEntry.Selected += OnCancel;
            MenuEntries.Add(exitGameMenuEntry);

            // start the menu music
            AudioManager.PushMusic("MainMusic");
        }


        /// <summary>
        /// Load the graphics content for this screen.
        /// </summary>
        public override void LoadContent()
        {
            // load the textures
            ContentManager content = ScreenManager.Game.Content;
            backgroundTexture = content.Load<Texture2D>(@"Textures\MainMenu\TitleScreen");
           
            
            plankTexture1 = 
                content.Load<Texture2D>(@"Textures\MainMenu\MainMenuPlank");
            plankTexture2 = 
                content.Load<Texture2D>(@"Textures\MainMenu\MainMenuPlank");
            plankTexture3 = 
                content.Load<Texture2D>(@"Textures\MainMenu\MainMenuPlank");
            backTexture = content.Load<Texture2D>(@"Textures\Buttons\facebutton_b");
            selectTexture = content.Load<Texture2D>(@"Textures\Buttons\facebutton_a");

            // calculate the texture positions
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            backgroundPosition = new Vector2(
                (viewport.Width - backgroundTexture.Width) / 1,
                (viewport.Height - backgroundTexture.Height) / 1);                        
            backPosition = backgroundPosition + new Vector2(225, 610);
            selectPosition = backgroundPosition + new Vector2(1120, 610);

            // set the textures on each menu entry
            newGameMenuEntry.Texture = plankTexture3;            
            controlsMenuEntry.Texture = plankTexture2;
            creditsMenuEntry.Texture = plankTexture3;
            exitGameMenuEntry.Texture = plankTexture1;

             //now that they have textures, set the proper positions on the menu entries
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntries[i].Position = new Vector2(
                    MenuEntries[i].Position.X,
                   350f - ((MenuEntries[i].Texture.Height - 0) * 
                        (MenuEntries.Count - 1 - i)));
            }

            base.LoadContent();
        }

        #endregion


        #region Updating


        /// <summary>
        /// Handles user input.
        /// </summary>
        public override void HandleInput()
        {
            if (InputManager.IsActionTriggered(InputManager.Action.Back) &&
                Session.IsActive)
            {
                AudioManager.PopMusic();
                ExitScreen();
                return;
            }

            base.HandleInput();
        }


        /// <summary>
        /// Event handler for when the New Game menu entry is selected.
        /// </summary>
        void NewGameMenuEntrySelected(object sender, EventArgs e)
        {
            if (Session.IsActive)
            {
                ExitScreen();
            }

            ContentManager content = ScreenManager.Game.Content;
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen(
                content.Load<GameStartDescription>("MainGameDescription")));
        }


        
        void ControlsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new ControlsScreen());
        }

        
        /// <summary>
        /// Event handler for when the Help menu entry is selected.
        /// </summary>
        void CreditMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new CreditScreen());
        }

        
        /// <summary>
        /// When the user cancels the main menu,
        /// or when the Exit Game menu entry is selected.
        /// </summary>
        protected override void OnCancel()
        {
            // add a confirmation message box
            string message = String.Empty;
            if (Session.IsActive)
            {
                message = 
                    "Are you sure you want to exit?";
            }
            else
            {
                message = "Are you sure you want to exit?";
            }
            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);
            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
            ScreenManager.AddScreen(confirmExitMessageBox);
        }


        /// <summary>
        /// Event handler for when the user selects Yes 
        /// on the "Are you sure?" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }
        

        #endregion


        #region Drawing


        /// <summary>
        /// Draw this screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // draw the background images
            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);           
            

            // Draw each menu entry in turn.
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntry menuEntry = MenuEntries[i];
                bool isSelected = IsActive && (i == selectedEntry);
                menuEntry.Draw(this, isSelected, gameTime);
            }            

            // draw the select instruction
            spriteBatch.Draw(selectTexture, selectPosition, Color.White);
            spriteBatch.DrawString(Fonts.ButtonNamesFont, "Select",
                new Vector2(
                selectPosition.X - Fonts.ButtonNamesFont.MeasureString("Select").X - 1, 
                selectPosition.Y + 1), Color.White);

            // if we are in-game, draw the back instruction
            if (Session.IsActive)
            {
                spriteBatch.Draw(backTexture, backPosition, Color.White);
                spriteBatch.DrawString(Fonts.ButtonNamesFont, "Resume",
                    new Vector2(backPosition.X + 55, backPosition.Y + 5), Color.White);
            }

            spriteBatch.End();
        }

        #endregion
    }
}
