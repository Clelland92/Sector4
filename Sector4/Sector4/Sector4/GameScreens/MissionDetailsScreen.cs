

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
    /// Display the details of a  mission.
    /// </summary>
    class MissionDetailsScreen : GameScreen
    {
        private Mission mission;


        #region Graphics Data


        private Texture2D backgroundTexture;
        private Texture2D backIconTexture;
        private Texture2D scrollTexture;
        private Texture2D fadeTexture;
        private Texture2D lineTexture;

        private Vector2 backgroundPosition;
        private Vector2 screenSize;
        private Vector2 titlePosition;
        private Vector2 textPosition;
        private Vector2 backTextPosition;
        private Vector2 backIconPosition;
        private Vector2 scrollPosition;
        private Vector2 topLinePosition;
        private Vector2 bottomLinePosition;
        private Rectangle fadeDest;

        private Color headerColor = new Color(128, 6, 6);
        private Color textColor = new Color(102, 40, 16);


        #endregion


        #region Dialog Text


        private string titleString = "Mission Details";
        private List<Line> currentDialogue;

        private int startIndex;
        private int endIndex;
        private int maxLines;


        #endregion


        #region Initialization


        /// <summary>
        /// Creates a new MissionDetailsScreen 
        /// </summary>
        public MissionDetailsScreen(Mission mission)
            : base()
        {
            // check the parameter
            if (mission == null)
            {
                throw new ArgumentNullException("Mission");
            }
            this.mission = mission;
            this.IsPopup = true;

            currentDialogue = new List<Line>();
            maxLines = 13;

            textPosition.X = 261f;

            AddStrings(this.mission.Name,
                Fonts.BreakTextIntoList(this.mission.Description,
                Fonts.DescriptionFont, 715), GetRequirements(this.mission));
        }


        
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            backgroundTexture =
                content.Load<Texture2D>(@"Textures\GameScreens\PopupScreen");
            backIconTexture =
                content.Load<Texture2D>(@"Textures\Buttons\facebutton_b");
            scrollTexture =
                content.Load<Texture2D>(@"Textures\GameScreens\ScrollButtons");
            
            fadeTexture = content.Load<Texture2D>(@"Textures\GameScreens\FadeScreen");

            // Get the screen positions
            screenSize = new Vector2(viewport.Width, viewport.Height);
            fadeDest = new Rectangle(viewport.X, viewport.Y,
                viewport.Width, viewport.Height);

            backgroundPosition = new Vector2(
                (viewport.Width - backgroundTexture.Width) / 2,
                (viewport.Height - backgroundTexture.Height) / 2);
            scrollPosition = backgroundPosition + new Vector2(820f, 200f);

            titlePosition = new Vector2(
                (screenSize.X - Fonts.HeaderFont.MeasureString(titleString).X) / 2,
                backgroundPosition.Y + 20f);

            backTextPosition = new Vector2(screenSize.X / 2 - 500f,
                backgroundPosition.Y + 530f);
            backIconPosition = new Vector2(
                backTextPosition.X - backIconTexture.Width - 10f, backTextPosition.Y);

            
        }


        #endregion


        #region Text Generation


        /// <summary>
        /// A line of text with its own color and font.
        /// </summary>
        private struct Line
        {
            public string text;
            public Color color;
            public SpriteFont font;
        }


        /// <summary>
        /// Add strings to list of lines
        /// </summary>
        /// <param name="name">Name of the mission</param>
        /// <param name="description">Description of the mission</param>
        /// <param name="requirements">Requirements of the mission</param>
        private void AddStrings(string name, List<string> description,
            List<Line> requirements)
        {
            Line line;

            line.color = headerColor;
            line.font = Fonts.DescriptionFont;

            // Title text
            titleString = name;
            titlePosition.X = (screenSize.X -
                Fonts.HeaderFont.MeasureString(titleString).X) / 2;
            titlePosition.Y = backgroundPosition.Y + 70f;

            currentDialogue.Clear();
            line.text = "Mission";
            currentDialogue.Add(line);
            foreach (string str in description)
            {
                line.text = str;
                line.color = textColor;
                currentDialogue.Add(line);
            }
            foreach (Line str in requirements)
            {
                currentDialogue.Add(str);
            }
            // Set the start index and end index
            startIndex = 0;
            endIndex = maxLines;
            if (endIndex > currentDialogue.Count)
            {
                textPosition.Y = 375f;
                foreach (Line str in currentDialogue)
                {
                    textPosition.Y -= str.font.LineSpacing / 2;
                }

                endIndex = currentDialogue.Count;
            }
            else
            {
                textPosition.Y = 225f;
            }
        }


        /// <summary>
        ///  mission requirements
        /// </summary>
        
        private List<Line> GetRequirements(Mission mission)
        {
            List<Line> reqdList;
            Line reqd;
            int currentCount = 0;
            int totalCount = 0;
            List<string> dialog;

            reqdList = new List<Line>();
            reqd.font = Fonts.DescriptionFont;

            // Add Monster Requirements
            if (mission.MonsterRequirements.Count > 0)
            {
                reqd.color = headerColor;
                reqd.text = String.Empty;
                reqdList.Add(reqd);
                reqd.text = "Monster Progress";
                reqdList.Add(reqd);

                for (int i = 0; i < mission.MonsterRequirements.Count; i++)
                {
                    reqd.color = textColor;
                    currentCount = mission.MonsterRequirements[i].CompletedCount;
                    totalCount = mission.MonsterRequirements[i].Count;
                    Monster monster = mission.MonsterRequirements[i].Content;
                    reqd.text = monster.Name + " = " + currentCount + " / " + 
                        totalCount;

                    if (currentCount == totalCount)
                    {
                        reqd.color = Color.Red;
                    }
                    reqdList.Add(reqd);
                }
            }

            // Add Item Requirements
            if (mission.GearRequirements.Count > 0)
            {
                reqd.color = headerColor;
                reqd.text = String.Empty;
                reqdList.Add(reqd);
                reqd.text = "Item Progress";
                reqdList.Add(reqd);

                for (int i = 0; i < mission.GearRequirements.Count; i++)
                {
                    reqd.color = textColor;
                    currentCount = mission.GearRequirements[i].CompletedCount;
                    totalCount = mission.GearRequirements[i].Count;
                    Gear gear = mission.GearRequirements[i].Content;
                    reqd.text = gear.Name + " = " + currentCount + " / " + totalCount;
                    if (currentCount == totalCount)
                    {
                        reqd.color = Color.Red;
                    }
                    reqdList.Add(reqd);
                }
            }

            // Add Current Objective
            reqd.color = headerColor;
            reqd.text = String.Empty;
            reqdList.Add(reqd);
            reqd.text = "Current Objective";
            reqdList.Add(reqd);
            reqd.color = textColor;

            switch (mission.Stage)
            {
                case Mission.MissionStage.InProgress:
                    dialog = Fonts.BreakTextIntoList(mission.ObjectiveMessage,
                        Fonts.DescriptionFont, 715);
                    for (int i = 0; i < dialog.Count; i++)
                    {
                        reqd.text = dialog[i];
                        reqdList.Add(reqd);
                    }
                    break;

                case Mission.MissionStage.RequirementsMet:
                    dialog = Fonts.BreakTextIntoList(mission.DestinationObjectiveMessage,
                        Fonts.DescriptionFont, 715);
                    for (int i = 0; i < dialog.Count; i++)
                    {
                        reqd.text = dialog[i];
                        reqdList.Add(reqd);
                    }
                    break;

                case Mission.MissionStage.Completed:
                    reqd.font = Fonts.ButtonNamesFont;
                    reqd.color = new Color(139, 21, 73);
                    reqd.text = "Mission Completed";
                    reqdList.Add(reqd);
                    break;
            }

            return reqdList;
        }


        #endregion


        #region Updating


        /// <summary>
        /// Handles user input.
        /// </summary>
        public override void HandleInput()
        {
            // exit the screen
            if (InputManager.IsActionTriggered(InputManager.Action.Back) ||
                InputManager.IsActionTriggered(InputManager.Action.Ok))
            {
                ExitScreen();
                return;
            }
            // scroll up
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorUp))
            {
                if (startIndex > 0)
                {
                    startIndex--;
                    endIndex--;
                }
            }
            // scroll Down
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorDown))
            {
                if (endIndex < currentDialogue.Count)
                {
                    startIndex++;
                    endIndex++;
                }
            }
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draw the screen
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Vector2 dialoguePosition = textPosition;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // Draw the fading screen
            spriteBatch.Draw(fadeTexture, fadeDest, Color.White);

            // Draw the popup background
            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);

            

            // Draw the scrollbar
            spriteBatch.Draw(scrollTexture, scrollPosition, Color.White);

            // Draw the Back button
            spriteBatch.Draw(backIconTexture, backIconPosition, Color.White);
            spriteBatch.DrawString(Fonts.ButtonNamesFont, "Back", backTextPosition,
                Color.White);

            // Draw the title
            spriteBatch.DrawString(Fonts.HeaderFont, titleString, titlePosition,
                Fonts.TitleColor);

            //Draw the information dialog
            for (int i = startIndex; i < endIndex; i++)
            {
                dialoguePosition.X = (int)((screenSize.X -
                    currentDialogue[i].font.MeasureString(
                    currentDialogue[i].text).X) / 2) - 20;

                spriteBatch.DrawString(currentDialogue[i].font,
                    currentDialogue[i].text, dialoguePosition, 
                    currentDialogue[i].color);
                dialoguePosition.Y += currentDialogue[i].font.LineSpacing;
            }

            spriteBatch.End();
        }


        #endregion
    }
}
