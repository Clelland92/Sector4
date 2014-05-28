

#region Using Statements
using System;
using Sector4Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Sector4
{
    /// <summary>
    /// Actions taken by a player in combat.
    /// </summary>
    
    abstract class CombatAction
    {
        #region State


        /// <summary>
        /// Returns true if the action is offensive.
        /// </summary>
        public abstract bool IsOffensive
        {
            get;
        }


        /// <summary>
        /// Returns true if this action needs a target.
        /// </summary>
        public abstract bool IsTargetNeeded
        {
            get;
        }


        #endregion


        #region Combat Stage


        /// <summary>
        /// Stages of the action as it is executed.
        /// </summary>
        public enum CombatActionStage
        {
            /// <summary>
            /// The initial state.
            /// </summary>
            NotStarted,

            /// <summary>
            /// The action is getting ready to start.
            
            Preparing,

            
            /// <example>
            /// The character walks to melee targets while in this stage.  
            /// </example>
            Advancing,


            /// <summary>
            /// The action is being done to the target(s).
            /// </summary>
            Executing,

            /// <summary>
            /// The action is returning from the target
            
            Returning,

            /// <summary>
            /// The action is ending
            /// </summary>
            Finishing,

            /// <summary>
            /// The action is complete.
            /// </summary>
            Complete,
        };


        /// <summary>
        /// The current state of the action.
        /// </summary>
        protected CombatActionStage stage = CombatActionStage.NotStarted;

        /// <summary>
        /// The current state of the action.
        /// </summary>
        public CombatActionStage Stage
        {
            get { return stage; }
        }


        /// <summary>
        /// Starts a new combat stage.  
        /// </summary>
        
        protected virtual void StartStage()
        {
            switch (stage)
            {
                case CombatActionStage.Preparing: 
                    break;

                case CombatActionStage.Advancing:
                    break;

                case CombatActionStage.Executing:
                    break;

                case CombatActionStage.Returning:
                    break;

                case CombatActionStage.Finishing:
                    break;

                case CombatActionStage.Complete:
                    break;
            }
        }


        /// <summary>
        /// Update the action for the current fight.
        /// </summary>
       
        protected virtual void UpdateCurrentStage(GameTime gameTime)
        {
            switch (stage)
            {
                case CombatActionStage.NotStarted:
                    break;

                case CombatActionStage.Preparing:
                    break;

                case CombatActionStage.Advancing:
                    break;

                case CombatActionStage.Executing:
                    break;

                case CombatActionStage.Returning:
                    break;

                case CombatActionStage.Finishing:
                    break;

                case CombatActionStage.Complete:
                    break;
            }
        }


        /// <summary>
        /// Returns true if the combat action is ready to proceed 
        /// </summary>
        protected virtual bool IsReadyForNextStage
        {
            get
            {
                switch (stage)
                {
                    case CombatActionStage.Preparing: 
                        break;

                    case CombatActionStage.Advancing: 
                        break;

                    case CombatActionStage.Executing: 
                        break;

                    case CombatActionStage.Returning: 
                        break;

                    case CombatActionStage.Finishing: 
                        break;
                }

                // fall through - the action doesn't care about the state, so move on
                return true;
            }
        }


        #endregion


        #region Combatant


        /// <summary>
        /// The player performing this action.
        /// </summary>
        protected Combatant combatant;

        /// <summary>
        /// The player performing this action.
        /// </summary>
        public Combatant Combatant
        {
            get { return combatant; }
        }


        /// <summary>
        /// Returns true if the player can use this action.
        /// </summary>
        public virtual bool IsCharacterValidUser
        {
            get { return true; }
        }


        #endregion


        #region Target


        /// <summary>
        /// The target of the action.
        /// </summary>
        public Combatant Target = null;


        /// <summary>
        /// The number of adjacent targets in each direction that are affected.
        /// </summary>
        protected int adjacentTargets = 0;

        /// <summary>
        /// The number of adjacent targets in each direction that are affected.
        /// </summary>
        public int AdjacentTargets
        {
            get { return adjacentTargets; }
        }


        #endregion


        #region Heuristic


        /// <summary>
        /// The heuristic used to compare actions of this type to similar ones.
        /// </summary>
        public abstract int Heuristic
        {
            get;
        }


        /// <summary>
        /// Compares the combat actions by their heuristic, in descending order.
        /// </summary>
        public static int CompareCombatActionsByHeuristic(
            CombatAction a, CombatAction b)
        {
            return b.Heuristic.CompareTo(a.Heuristic);
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructs a new CombatAction object.
        /// </summary>
        /// <param name="combatant">The combatant performing the action.</param>
        public CombatAction(Combatant combatant)
        {
            // check the parameter
            if (combatant == null)
            {
                throw new ArgumentNullException("combatant");
            }

            // assign the parameter
            this.combatant = combatant;

            Reset();
        }


        /// <summary>
        /// Reset the action so that it may be started again.
        /// </summary>
        public virtual void Reset()
        {
            // set the state to not-started
            stage = CombatActionStage.NotStarted;
        }


        /// <summary>
        /// Start executing the combat action.
        /// </summary>
        public virtual void Start()
        {
            // set the state to the first step
            stage = CombatActionStage.Preparing;
            StartStage();
        }


        #endregion


        #region Updating


        /// <summary>
        /// Updates the action over time.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            // update the current stage
            UpdateCurrentStage(gameTime);

            // if the action is ready for the next stage, then advance
            if ((stage != CombatActionStage.NotStarted) &&
                (stage != CombatActionStage.Complete) && IsReadyForNextStage)
            {
                switch (stage)
                {
                    case CombatActionStage.Preparing:
                        stage = CombatActionStage.Advancing;
                        break;

                    case CombatActionStage.Advancing:
                        stage = CombatActionStage.Executing;
                        break;

                    case CombatActionStage.Executing:
                        stage = CombatActionStage.Returning;
                        break;

                    case CombatActionStage.Returning:
                        stage = CombatActionStage.Finishing;
                        break;

                    case CombatActionStage.Finishing:
                        stage = CombatActionStage.Complete;
                        break;
                }
                StartStage();
            }
        }


        #endregion


        #region Drawing


        
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }


        #endregion
    }
}
