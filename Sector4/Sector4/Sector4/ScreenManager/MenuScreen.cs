

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Sector4
{
   
    abstract class MenuScreen : GameScreen
    {
        #region Fields

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        protected int selectedEntry = 0;

        #endregion


        #region Properties


        
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }


        protected MenuEntry SelectedMenuEntry
        {
            get
            {
                if ((selectedEntry < 0) || (selectedEntry >= menuEntries.Count))
                {
                    return null;
                }
                return menuEntries[selectedEntry];
            }
        }


        #endregion

        
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        #endregion


        #region Handle Input


        /// <summary>
        /// Responds to user input
        /// </summary>
        public override void HandleInput()
        {
            int oldSelectedEntry = selectedEntry;

            // Move to the previous menu entry?
            if (InputManager.IsActionTriggered(InputManager.Action.CursorUp))
            {
                selectedEntry--;
                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }

            // Move to the next menu entry?
            if (InputManager.IsActionTriggered(InputManager.Action.CursorDown))
            {
                selectedEntry++;
                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            // Accept or cancel the menu?
            if (InputManager.IsActionTriggered(InputManager.Action.Ok))
            {
                AudioManager.PlayCue("Continue");
                OnSelectEntry(selectedEntry);
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.Back) ||
                InputManager.IsActionTriggered(InputManager.Action.ExitGame))
            {
                OnCancel();
            }
            else if (selectedEntry != oldSelectedEntry)
            {
                AudioManager.PlayCue("MenuMove");
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            menuEntries[selectedEntry].OnSelectEntry();
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel()
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);
            }

            spriteBatch.End();
        }


        #endregion
    }
}
