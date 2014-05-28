

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Sector4
{
    
    
    class LoadingScreen : GameScreen
    {
        #region Screens Data

        bool loadingIsSlow;
        bool otherScreensAreGone;

        GameScreen[] screensToLoad;


        #endregion


        #region Graphics Data


        private Texture2D loadingTexture;
        private Vector2 loadingPosition;

        private Texture2D loadingBlackTexture;
        private Rectangle loadingBlackTextureDestination;


        #endregion


        #region Initialization


        
        private LoadingScreen(ScreenManager screenManager, bool loadingIsSlow,
                              GameScreen[] screensToLoad)
        {
            this.loadingIsSlow = loadingIsSlow;
            this.screensToLoad = screensToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
        }


        /// <summary>
        /// Activates the loading screen.
        /// </summary>
        public static void Load(ScreenManager screenManager, bool loadingIsSlow,
                                params GameScreen[] screensToLoad)
        {
            // Tell all the current screens to transition off.
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            LoadingScreen loadingScreen = new LoadingScreen(screenManager,
                                                            loadingIsSlow,
                                                            screensToLoad);

            screenManager.AddScreen(loadingScreen);
        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            loadingTexture = content.Load<Texture2D>(@"Textures\MainMenu\Background");
            loadingBlackTexture =
                content.Load<Texture2D>(@"Textures\GameScreens\FadeScreen");
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            loadingBlackTextureDestination = new Rectangle(viewport.X, viewport.Y, 
                viewport.Width, viewport.Height);
            loadingPosition = new Vector2(
                viewport.X + (float)Math.Floor((viewport.Width - 
                    loadingTexture.Width) / 2f),
                viewport.Y + (float)Math.Floor((viewport.Height - 
                    loadingTexture.Height) / 2f));

            base.LoadContent();
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            
            {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen);
                    }
                }

                
                ScreenManager.Game.ResetElapsedTime();
            }
        }


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            
            if ((ScreenState == ScreenState.Active) &&
                (ScreenManager.GetScreens().Length == 1))
            {
                otherScreensAreGone = true;
            }

            
            if (loadingIsSlow)
            {
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

                // Center the text in the viewport.
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);

                Color color = new Color(255, 255, 255, TransitionAlpha);

                spriteBatch.Begin();
                spriteBatch.Draw(loadingBlackTexture, loadingBlackTextureDestination,
                    Color.White);
                spriteBatch.Draw(loadingTexture, loadingPosition, Color.White);
                spriteBatch.End();
            }
        }


        #endregion
    }
}
