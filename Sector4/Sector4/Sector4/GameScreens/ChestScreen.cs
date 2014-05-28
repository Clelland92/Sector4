

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
    class ChestScreen : InventoryScreen
    {
        #region Graphics Data


        
        private Texture2D leftQuantityArrow;

        
        private Texture2D rightQuantityArrow;


        #endregion


        #region Columns


        private const int nameColumnInterval = 80;
        private const int powerColumnInterval = 270;
        private const int quantityColumnInterval = 450;


        #endregion


        #region Data Access


        
        private MapEntry<Chest> chestEntry;

        // get the gear in the chest 
        public override ReadOnlyCollection<ContentEntry<Gear>> GetDataList()
        {
            if ((chestEntry == null) || (chestEntry.Content == null))
            {
                return null;
            }
            return chestEntry.Content.Entries.AsReadOnly();
        }


        /// <summary>
        /// The selected quantity of the current entry.
        /// </summary>
        private int selectedQuantity = 0;


        
        private void ResetSelectedQuantity()
        {
            
            if ((chestEntry == null) || (chestEntry.Content == null))
            {
                selectedQuantity = 0;
                return;
            }

            // set the quantity to the maximum
            if (IsMoneySelected)
            {
                selectedQuantity = chestEntry.Content.Money;
            }
            else if ((SelectedGearIndex >= 0) && 
                (SelectedGearIndex < chestEntry.Content.Entries.Count))
            {
                selectedQuantity = chestEntry.Content[SelectedGearIndex].Count;
            }
        }

        #endregion


        #region List Navigation


        
        public bool IsMoneySelected
        {
            get 
            { 
                return ((chestEntry != null) && (chestEntry.Content != null) && 
                    (chestEntry.Content.Money > 0) && (SelectedIndex == 0));
            }
        }


        
        public int SelectedGearIndex
        {
            get
            {
                return ((chestEntry != null) && (chestEntry.Content != null) && 
                    (chestEntry.Content.Money > 0)) ? SelectedIndex - 1 : SelectedIndex;
            }
        }


        /// <summary>
        /// Move the current selection up one entry.
        /// </summary>
        protected override void MoveCursorUp()
        {
            base.MoveCursorUp();
            ResetSelectedQuantity();
        }


        /// <summary>
        /// Move the current selection down one entry.
        /// </summary>
        protected override void MoveCursorDown()
        {
            base.MoveCursorDown();
            ResetSelectedQuantity();
        }


        /// <summary>
        /// Decrease the selected quantity by one.
        /// </summary>
        protected override void MoveCursorLeft()
        {
            // decrement the quantity, looping around if necessary
            if (selectedQuantity > 0)
            {
                selectedQuantity--;
            }
            else if (IsMoneySelected)
            {
                selectedQuantity = chestEntry.Content.Money;
            }
            else if (SelectedGearIndex < chestEntry.Content.Entries.Count)
            {
                selectedQuantity = chestEntry.Content[SelectedGearIndex].Count;
            }
        }


        /// <summary>
        /// Increase the selected quantity by one.
        /// </summary>
        protected override void MoveCursorRight()
        {
            int maximumQuantity = 0;
            // get the maximum quantity for the selected entry
            if (IsMoneySelected)
            {
                maximumQuantity = chestEntry.Content.Money;
            }
            else if (SelectedGearIndex < chestEntry.Content.Entries.Count)
            {
                maximumQuantity = chestEntry.Content[SelectedGearIndex].Count;
            }
            else
            {
                return;
            }
            // loop to zero if the selected quantity is already at maximum.
            selectedQuantity = selectedQuantity < maximumQuantity ? 
                selectedQuantity + 1 : 0;
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Creates a new ChestScreen object.
        /// </summary>
        /// <param name="chestEntry">The chest entry shown in the screen</param>
        public ChestScreen(MapEntry<Chest> chestEntry)
            : base(true)
        {
            // check the parameter
            if ((chestEntry == null) || (chestEntry.Content == null))
            {
                throw new ArgumentNullException("chestEntry.Content");
            }
            this.chestEntry = chestEntry;

            // clean up any empty entries
            this.chestEntry.Content.Entries.RemoveAll(
                delegate(ContentEntry<Gear> contentEntry)
                {
                    return (contentEntry.Count <= 0);
                });

            // sort the chest entries by name
            this.chestEntry.Content.Entries.Sort(
                delegate(ContentEntry<Gear> gearEntry1, ContentEntry<Gear> gearEntry2)
                {
                    // handle null values
                    if ((gearEntry1 == null) || (gearEntry1.Content == null))
                    {
                        return ((gearEntry2 == null) || (gearEntry2.Content == null) ?
                            0 : 1);
                    }
                    else if ((gearEntry2 == null) || (gearEntry2.Content == null))
                    {
                        return -1;
                    }

                    // sort by name
                    return gearEntry1.Content.Name.CompareTo(gearEntry2.Content.Name);
                });

            // set up the initial selected-quantity values
            ResetSelectedQuantity();

            // configure the menu strings
            titleText = chestEntry.Content.Name;
            selectButtonText = "Take";
            backButtonText = "Close";
            xButtonText = String.Empty;
            yButtonText = "Take All";
            leftTriggerText = String.Empty;
            rightTriggerText = String.Empty;
        }
        

        /// <summary>
        /// Load the graphics content from the content manager.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            ContentManager content = ScreenManager.Game.Content;

            leftQuantityArrow = 
                content.Load<Texture2D>(@"Textures\Buttons\QuantityArrowLeft");
            rightQuantityArrow = 
                content.Load<Texture2D>(@"Textures\Buttons\QuantityArrowRight");
        }


        #endregion


        #region Input Handling


        /// <summary>
        /// Allows the screen to handle user input.
        /// </summary>
        public override void HandleInput()
        {
            // if the chestEntry.Content is empty, exit immediately
            if (chestEntry.Content.IsEmpty)
            {
                BackTriggered();
                return;
            }

            if (InputManager.IsActionTriggered(InputManager.Action.CursorUp))
            {
                MoveCursorUp();
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.CursorDown))
            {
                MoveCursorDown();
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.IncreaseAmount))
            {
                MoveCursorRight();
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.DecreaseAmount))
            {
                MoveCursorLeft();
            }
            // Close is pressed
            else if (InputManager.IsActionTriggered(InputManager.Action.Back))
            {
                BackTriggered();
            }
            // Take is pressed
            else if (InputManager.IsActionTriggered(InputManager.Action.Ok))
            {
                if (IsMoneySelected)
                {
                    SelectTriggered(null);
                }
                else
                {
                    ReadOnlyCollection<ContentEntry<Gear>> dataList = GetDataList();
                    SelectTriggered(dataList[SelectedGearIndex]);
                }
            }
            // Take All is pressed
            else if (InputManager.IsActionTriggered(InputManager.Action.TakeView))
            {
                ButtonYPressed(null); // take-all doesn't need an entry
            }
        }


        /// <summary>
        /// Respond to the triggering of the Back action.
        /// </summary>
        protected override void BackTriggered()
        {
            // clean up any empty entries
            chestEntry.Content.Entries.RemoveAll(
                delegate(ContentEntry<Gear> contentEntry)
                {
                    return (contentEntry.Count <= 0);
                });

            // if the chestEntry.Content is empty, remove it from the game and exit
            if (chestEntry.Content.IsEmpty)
            {
                Session.RemoveChest(chestEntry);
            }
            else
            {
                // otherwise, store the modified chestEntry.Content
                Session.StoreModifiedChest(chestEntry);
            }

            // exit the screen
            base.BackTriggered();
        }


        
        protected override void SelectTriggered(ContentEntry<Gear> entry) 
        {
            // if the quantity is zero, don't bother
            if (selectedQuantity <= 0)
            {
                return;
            }

            // check to see if money is selected
            if (IsMoneySelected)
            {
                // play the "pick up money" cue
                AudioManager.PlayCue("Pistol");
                // add the money to the party
                Session.Party.PartyMoney += selectedQuantity;
                chestEntry.Content.Money -= selectedQuantity;
                if (chestEntry.Content.Money > 0)
                {
                    selectedQuantity =
                        Math.Min(selectedQuantity, chestEntry.Content.Money);
                }
                else
                {
                    ResetSelectedQuantity();
                }
            }
            else
            {
                // remove the selected quantity of gear from the chest
                int quantity = selectedQuantity;
                if ((entry.Content != null) && (quantity > 0))
                {
                    Session.Party.AddToInventory(entry.Content, quantity);
                    entry.Count -= quantity;
                }
                if (entry.Count > 0)
                {
                    selectedQuantity = Math.Min(entry.Count, selectedQuantity);
                }
                else
                {
                    // if the entry is now empty, remove it from the chest
                    chestEntry.Content.Entries.RemoveAt(SelectedGearIndex);
                    ResetSelectedQuantity();
                }
            }
        }


        /// <summary>
        /// Respond to the triggering of the Y button (and related key).
        /// </summary>
        protected override void ButtonYPressed(ContentEntry<Gear> entry) 
        {
            // add the entire amount of money
            if (chestEntry.Content.Money > 0)
            {
                AudioManager.PlayCue("Pistol");
                Session.Party.PartyMoney += chestEntry.Content.Money;
                chestEntry.Content.Money = 0;
            }
            // add all items at full quantity
            // -- there is no limit to the party's inventory
            ReadOnlyCollection<ContentEntry<Gear>> entries = GetDataList();
            foreach (ContentEntry<Gear> gearEntry in entries)
            {
                Session.Party.AddToInventory(gearEntry.Content, gearEntry.Count);
            }
            // clear the entries, as they're all gone now
            chestEntry.Content.Entries.Clear();
            selectedQuantity = 0;
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draws the screen.
        /// </summary>
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // get the content list
            ReadOnlyCollection<ContentEntry<Gear>> dataList = GetDataList();

            // fix the indices for the current list size
            int maximumCount = chestEntry.Content.Money > 0 ? dataList.Count + 1 : 
                dataList.Count;
            SelectedIndex = (int)MathHelper.Clamp(SelectedIndex, 0, maximumCount - 1);
            StartIndex = (int)MathHelper.Clamp(StartIndex, 0,
                maximumCount - MaximumListEntries);
            EndIndex = Math.Min(StartIndex + MaximumListEntries, maximumCount);

            spriteBatch.Begin();

            DrawBackground();
            if (dataList.Count > 0)
            {
                DrawListPosition(SelectedIndex + 1, maximumCount);
            }
            DrawButtons();
            DrawPartyMoney();
            DrawColumnHeaders();
            DrawTitle();

            // draw each item currently shown
            Vector2 position = listEntryStartPosition +
                new Vector2(0f, listLineSpacing / 2);
            for (int index = StartIndex; index < EndIndex; index++)
            {
                if ((index == 0) && (chestEntry.Content.Money > 0))
                {
                    if (index == SelectedIndex)
                    {
                        DrawSelection(position);
                        DrawMoneyEntry(position, true);
                    }
                    else
                    {
                        DrawMoneyEntry(position, false);
                    }
                }
                else
                {
                    int currentIndex = chestEntry.Content.Money > 0 ? index - 1 : index;
                    ContentEntry<Gear> entry = dataList[currentIndex];
                    if (index == SelectedIndex)
                    {
                        DrawSelection(position);
                        DrawEntry(entry, position, true);
                        DrawSelectedDescription(entry);
                    }
                    else
                    {
                        DrawEntry(entry, position, false);
                    }
                }
                position.Y += listLineSpacing;
            }

            spriteBatch.End();
        }


       
        protected virtual void DrawMoneyEntry(Vector2 position, bool isSelected)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 drawPosition = position;

            // draw the icon
            spriteBatch.Draw(moneyTexture, 
                drawPosition + iconOffset + new Vector2(0f, 7f), Color.White);

            // draw the name
            Color color = isSelected ? Fonts.HighlightColor : Fonts.DisplayColor;
            drawPosition.Y += listLineSpacing / 4;
            drawPosition.X += nameColumnInterval;
            spriteBatch.DrawString(Fonts.GearInfoFont, "Money", drawPosition, color);

            // skip the power text
            drawPosition.X += powerColumnInterval;

            // draw the quantity
            drawPosition.X += quantityColumnInterval;
            if (isSelected)
            {
                // draw the left selection arrow
                drawPosition.X -= leftQuantityArrow.Width;
                spriteBatch.Draw(leftQuantityArrow,
                    new Vector2(drawPosition.X, drawPosition.Y - 4), Color.White);
                drawPosition.X += leftQuantityArrow.Width;
                // draw the selected quantity ratio
                string quantityText = selectedQuantity.ToString() + "/" +
                    chestEntry.Content.Money.ToString();
                spriteBatch.DrawString(Fonts.GearInfoFont, quantityText, 
                    drawPosition, color);
                drawPosition.X += Fonts.GearInfoFont.MeasureString(quantityText).X;
                // draw the right selection arrow
                spriteBatch.Draw(rightQuantityArrow,
                    new Vector2(drawPosition.X, drawPosition.Y - 4), Color.White);
                drawPosition.X += rightQuantityArrow.Width;
            }
            else
            {
                // draw the remaining quantity
                spriteBatch.DrawString(Fonts.GearInfoFont, 
                    chestEntry.Content.Money.ToString(), drawPosition, color);
            }
        }


        
        protected override void DrawEntry(ContentEntry<Gear> entry, Vector2 position,
            bool isSelected)
        {
            // check the parameter
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }
            Gear gear = entry.Content as Gear;
            if (gear == null)
            {
                return;
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 drawPosition = position;

            // draw the icon
            spriteBatch.Draw(gear.IconTexture, drawPosition + iconOffset, Color.White);

            // draw the name
            Color color = isSelected ? Fonts.HighlightColor : Fonts.DisplayColor;
            drawPosition.Y += listLineSpacing / 4;
            drawPosition.X += nameColumnInterval;
            spriteBatch.DrawString(Fonts.GearInfoFont, gear.Name, drawPosition, color);

            // draw the power
            drawPosition.X += powerColumnInterval;
            string powerText = gear.GetPowerText();
            Vector2 powerTextSize = Fonts.GearInfoFont.MeasureString(powerText);
            Vector2 powerPosition = drawPosition;
            powerPosition.Y -= (float)Math.Ceiling((powerTextSize.Y - 30f) / 2);
            spriteBatch.DrawString(Fonts.GearInfoFont, powerText,
                powerPosition, color);

            // draw the quantity
            drawPosition.X += quantityColumnInterval;
            if (isSelected)
            {
                // draw the left selection arrow
                drawPosition.X -= leftQuantityArrow.Width;
                spriteBatch.Draw(leftQuantityArrow,
                    new Vector2(drawPosition.X, drawPosition.Y - 4), Color.White);
                drawPosition.X += leftQuantityArrow.Width;
                // draw the selected quantity ratio
                string quantityText = selectedQuantity.ToString() + "/" + 
                    entry.Count.ToString();
                spriteBatch.DrawString(Fonts.GearInfoFont, quantityText,
                    drawPosition, color);
                drawPosition.X += Fonts.GearInfoFont.MeasureString(quantityText).X;
                // draw the right selection arrow
                spriteBatch.Draw(rightQuantityArrow,
                    new Vector2(drawPosition.X, drawPosition.Y - 4), Color.White);
                drawPosition.X += rightQuantityArrow.Width;
            }
            else
            {
                // draw the remaining quantity
                spriteBatch.DrawString(Fonts.GearInfoFont, entry.Count.ToString(),
                    drawPosition, color);
            }
        }


        #endregion
    }
}