

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Sector4Data;
#endregion

namespace Sector4
{
    /// <summary>
    /// Shows the help screen, explaining the basic game idea to the user.
    /// </summary>
    class CreditScreen : GameScreen
    {
        #region Fields


        private Texture2D backgroundTexture;

        private Texture2D plankTexture;
        private Vector2 plankPosition;
        private Vector2 titlePosition;

        private string helpText =
            "Sector 4, A game made by Brian Hannah, Jonathan Dillon, Marc Clelland and Martin Clark " +
            " " +
            " " +
            " " +
            " " +
            " ";

        private List<string> textLines;

        private Texture2D scrollUpTexture;
        private readonly Vector2 scrollUpPosition = new Vector2(980, 200);
        private Texture2D scrollDownTexture;
        private readonly Vector2 scrollDownPosition = new Vector2(980, 460);

        
        

        private Texture2D backTexture;
        private readonly Vector2 backPosition = new Vector2(225, 610);

        private int startIndex;
        private const int maxLineDisplay = 7;


        #endregion


        #region Initialization

        public CreditScreen() 
            : base() 
        {
            textLines = Fonts.BreakTextIntoList(helpText, Fonts.DescriptionFont, 590);
        }

        /// <summary>
        /// Loads the graphics content for this screen
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            ContentManager content = ScreenManager.Game.Content;

            backgroundTexture = content.Load<Texture2D>(@"Textures\MainMenu\CreditScreen");
            plankTexture =
                content.Load<Texture2D>(@"Textures\MainMenu\MainMenuPlank");
            backTexture =
                content.Load<Texture2D>(@"Textures\Buttons\facebutton_b");
            //scrollUpTexture =
                content.Load<Texture2D>(@"Textures\GameScreens\ScrollUp");
            //scrollDownTexture =
                content.Load<Texture2D>(@"Textures\GameScreens\ScrollDown");
            

            plankPosition.X = backgroundTexture.Width / 2 - plankTexture.Width / 2;
            plankPosition.Y = 60;

            titlePosition.X = plankPosition.X + (plankTexture.Width -
                Fonts.HeaderFont.MeasureString("Game Credits").X) / 2;
            titlePosition.Y = plankPosition.Y + (plankTexture.Height -
                Fonts.HeaderFont.MeasureString("Game Credits").Y) / 2;
        }


        #endregion


        #region Updating


        /// <summary>
        /// Handles user input.
        /// </summary>
        public override void HandleInput()
        {
            // exits the screen
            if (InputManager.IsActionTriggered(InputManager.Action.Back))
            {
                ExitScreen();
                return;
            }
            // scroll down
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorDown))
            {
                // Traverse down the help text
                if (startIndex + maxLineDisplay < textLines.Count)
                {
                    startIndex += 1;
                }
            }
            // scroll up
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorUp))
            {
                // Traverse up the help text
                if (startIndex > 0)
                {
                    startIndex -= 1;
                }
            }
        }


        #endregion

        
        #region Drawing


        /// <summary>
        /// Draws the help screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
            spriteBatch.Draw(plankTexture, plankPosition, Color.White);
            spriteBatch.Draw(backTexture, backPosition, Color.White);

            
            spriteBatch.DrawString(Fonts.ButtonNamesFont, "Back",
                new Vector2(backPosition.X + 55, backPosition.Y + 5), Color.White);

            

            spriteBatch.DrawString(Fonts.HeaderFont, "Game Credits", titlePosition,
                Fonts.TitleColor);

            for (int i = 0; i < maxLineDisplay; i++)
            {
                
            }

            spriteBatch.End();
        }


        #endregion
    }
}