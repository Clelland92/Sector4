
#region Using Statements
using System;
using System.Collections.Generic;
using Sector4Data;
#endregion

namespace Sector4
{
    /// <summary>
    /// Determines AI for monsters in combat.
    /// </summary>
    
    class ArtificialIntelligence
    {
        /// <summary>
        /// The monster that this object is choosing AI for.
        /// </summary>
        private CombatantMonster monster;


        #region Action Lists


        /// <summary>
        /// The offensive actions available to the monster.
        /// </summary>
        private List<CombatAction> offensiveActions = new List<CombatAction>();


        /// <summary>
        /// The defensive actions available to the monster.
        /// </summary>
        private List<CombatAction> defensiveActions = new List<CombatAction>();


        #endregion


        #region Initialization


        /// <summary>
        /// Construct a new ArtificialIntelligence object to control a given combatant.
        /// </summary>
        public ArtificialIntelligence(CombatantMonster monster)
        {
            // check the parameter
            if (monster == null)
            {
                throw new ArgumentNullException("monster");
            }

            // assign the parameter
            this.monster = monster;

            // generate all actions available
            GenerateAllActions();
        }


        #endregion


        #region Action Generation


        /// <summary>
        /// Generate the actions available to this monster.
        /// </summary>
        private void GenerateAllActions()
        {
            // clear out any pre-existing actions
            offensiveActions.Clear();
            defensiveActions.Clear();

            // generate the melee attack option
            GenerateMeleeAction();

            // generate the rangedweapon attack options
            GenerateRangedWeaponAttackActions();

            // generate the defend action
            GenerateDefendAction();

            // sort the lists by potential, descending
            offensiveActions.Sort(CombatAction.CompareCombatActionsByHeuristic);
            defensiveActions.Sort(CombatAction.CompareCombatActionsByHeuristic);
        }


        /// <summary>
        /// Generate the melee attack option for this monster.
        /// </summary>
        private void GenerateMeleeAction()
        {
            // add a new melee action to the list
            offensiveActions.Add(new MeleeCombatAction(monster));
        }


        /// <summary>
        /// Generate the melee attack option for this monster.
        /// </summary>
        private void GenerateDefendAction()
        {
            // add a new melee action to the list
            defensiveActions.Add(new DefendCombatAction(monster));
        }


        /// <summary>
        /// Generate the rangedweapon attack options for this monster.
        /// </summary>
        private void GenerateRangedWeaponAttackActions()
        {
            // retrieve the rangedweapons for this monster
            List<RangedWeapon> rangedweapons = monster.Monster.RangedWeapons;

            // if there are no rangedweapons, then there's nothing to do
            if ((rangedweapons == null) || (rangedweapons.Count <= 0))
            {
                return;
            }

            // check each rangedweapon for attack actions
            foreach (RangedWeapon rangedweapon in rangedweapons)
            {
                // skip non-offensive rangedweapons
                if (!rangedweapon.IsOffensive)
                {
                    continue;
                }

                // add the new action to the list
                offensiveActions.Add(new WeaponryCombatAction(monster, rangedweapon));
            }
        }


        #endregion


        #region Action Selection


        
        /// <returns>The chosen action, 
        public CombatAction ChooseAction()
        {
            CombatAction combatAction = null;

            // determine if the monster will use a defensive action
            if ((monster.Monster.DefendPercentage > 0) && 
                (defensiveActions.Count > 0) &&
                (Session.Random.Next(0, 100) < monster.Monster.DefendPercentage))
            {
                combatAction = ChooseDefensiveAction();
            }

            // if we do not have an action yet
            combatAction = (combatAction ?? ChooseOffensiveAction());

           
            combatAction.Reset();

            return combatAction;
        }


        /// <summary>
        /// Choose which offensive action to perform.
        /// </summary>
        
        private CombatAction ChooseOffensiveAction()
        {
            List<CombatantPlayer> players = CombatEngine.Players;

            // is there is a valid combat in progress
            if ((players == null) || (players.Count <= 0))
            {
                return null;
            }

            // randomly choose a living target
            int targetIndex;
            do
            {
                targetIndex = Session.Random.Next(players.Count);
            }
            while (players[targetIndex].IsDeadOrDying);
            CombatantPlayer target = players[targetIndex];

            // the action lists are sorted by descending potential, 
            // so find the first eligible action
            foreach (CombatAction action in offensiveActions)
            {
                // check the restrictions on the action
                if (action.IsCharacterValidUser)
                {
                    action.Target = target;
                    return action;
                }
            }

            // no eligible actions found
            return null;
        }


        /// <summary>
        /// Choose which defensive action to perform.
        /// </summary>
        
        private CombatAction ChooseDefensiveAction()
        {
            List<CombatantMonster> monsters = CombatEngine.Monsters;

            // be sure that there is a valid combat in progress
            if ((monsters == null) || (monsters.Count <= 0))
            {
                return null;
            }

            // find the monster with the least health
            CombatantMonster target = null;
            int leastHealthAmount = Int32.MaxValue;
            foreach (CombatantMonster targetMonster in monsters)
            {
                // skip dead or dying targets
                if (targetMonster.IsDeadOrDying)
                {
                    continue;
                }
                // if the monster is damaged and it has the least health points,
                // then it becomes the new target
                StatisticsValue maxStatistics =
                    targetMonster.Monster.CharacterClass.GetStatisticsForLevel(
                        targetMonster.Monster.CharacterLevel);
                int targetMonsterHealthPoints = targetMonster.Statistics.HealthPoints;
                if ((targetMonsterHealthPoints < maxStatistics.HealthPoints) &&
                    (targetMonsterHealthPoints < leastHealthAmount))
                {
                    target = targetMonster;
                    leastHealthAmount = targetMonsterHealthPoints;
                }
            }

            // if there is no target, then don't do anything
            if (target == null)
            {
                return null;
            }

            // the action lists are sorted by descending potential, 
            // so find the first eligible action
            foreach (CombatAction action in defensiveActions)
            {
                // check the restrictions on the action
                if (action.IsCharacterValidUser)
                {
                    action.Target = target;
                    return action;
                }
            }

            // no eligible actions found
            return null;
        }


        #endregion
    }
}
