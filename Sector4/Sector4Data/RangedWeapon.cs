

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Sector4Data
{
    public class RangedWeapon : ContentObject
#if WINDOWS
, ICloneable
#endif
    {
        #region Description Data


        /// <summary>
        /// The name of this rangedweapon.
        /// </summary>
        private string name;

        /// <summary>
        /// The name of this rangedweapon.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        /// <summary>
        /// The long description of this rangedweapon.
        /// </summary>
        private string description;

        /// <summary>
        /// The long description of this rangedweapon.
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }


        /// <summary>
        /// The cost, in ammo points, to shoot this rangedweapon.
        /// </summary>
        private int ammoPointCost;

        /// <summary>
        /// The cost, in ammo points, to shoot this rangedweapon.
        /// </summary>
        public int AmmoPointCost
        {
            get { return ammoPointCost; }
            set { ammoPointCost = value; }
        }


        /// <summary>
        /// Builds and returns a string describing the power of this rangedweapon.
        /// </summary>
        public virtual string GetPowerText()
        {
            return TargetEffectRange.GetModifierString();
        }


        #endregion


        #region Target Buff/Debuff Data


        /// <summary>
        /// If true, the statistics change are used as a debuff (subtracted).
        /// Otherwise, the statistics change is used as a buff (added).
        /// </summary>
        private bool isOffensive;

        /// <summary>
        /// If true, the statistics change are used as a debuff (subtracted).
        /// Otherwise, the statistics change is used as a buff (added).
        /// </summary>
        public bool IsOffensive
        {
            get { return isOffensive; }
            set { isOffensive = value; }
        }


        /// <summary>
        /// The duration of the effect of this rangedweapon on its target, in rounds.
        /// </summary>
        /// <remarks>
        /// If the duration is zero, then the effects last for the rest of the battle.
        /// </remarks>
        private int targetDuration;

        /// <summary>
        /// The duration of the effect of this rangedweapon on its target, in rounds.
        /// </summary>
        /// <remarks>
        /// If the duration is zero, then the effects last for the rest of the battle.
        /// </remarks>
        public int TargetDuration
        {
            get { return targetDuration; }
            set { targetDuration = value; }
        }


        /// <summary>
        /// The range of statistics effects of this rangedweapon on its target.
        /// </summary>
        /// <remarks>
        /// This is a debuff if IsOffensive is true, otherwise it's a buff.
        /// </remarks>
        private StatisticsRange targetEffectRange = new StatisticsRange();

        /// <summary>
        /// The range of statistics effects of this rangedweapon on its target.
        /// </summary>
        /// <remarks>
        /// This is a debuff if IsOffensive is true, otherwise it's a buff.
        /// </remarks>
        [ContentSerializerIgnore]
        public StatisticsRange TargetEffectRange
        {
            get { return targetEffectRange; }
        }


        /// <summary>
        /// The initial range of statistics effects of this rangedweapon on its target.
        /// </summary>
        /// <remarks>
        /// This is a debuff if IsOffensive is true, otherwise it's a buff.
        /// </remarks>
        private StatisticsRange initialTargetEffectRange = new StatisticsRange();

        /// <summary>
        /// The initial range of statistics effects of this rangedweapon on its target.
        /// </summary>
        /// <remarks>
        /// This is a debuff if IsOffensive is true, otherwise it's a buff.
        /// </remarks>
        public StatisticsRange InitialTargetEffectRange
        {
            get { return initialTargetEffectRange; }
            set { initialTargetEffectRange = value; }
        }


        /// <summary>
        /// The number of simultaneous, adjacent targets affected by this rangedweapon.
        /// </summary>
        private int adjacentTargets;

        /// <summary>
        /// The number of simultaneous, adjacent targets affected by this rangedweapon.
        /// </summary>
        public int AdjacentTargets
        {
            get { return adjacentTargets; }
            set { adjacentTargets = value; }
        }


        #endregion


        #region RangedWeapon Leveling


        /// <summary>
        /// The level of the rangedweapon.
        /// </summary>
        private int level = 1;

        /// <summary>
        /// The level of the rangedweapon.
        /// </summary>
        [ContentSerializerIgnore]
        public int Level
        {
            get { return level; }
            set 
            {
                level = value;
                targetEffectRange = initialTargetEffectRange;
                for (int i = 1; i < level; i++)
                {
                    targetEffectRange += LevelingProgression;
                }
            }
        }


        /// <summary>
        /// Defines how the rangedweapon improves as it levels up.
        /// </summary>
        private StatisticsValue levelingProgression = new StatisticsValue();

        /// <summary>
        /// Defines how the rangedweapon improves as it levels up.
        /// </summary>
        public StatisticsValue LevelingProgression
        {
            get { return levelingProgression; }
            set { levelingProgression = value; }
        }


        #endregion


        #region Sound Effects Data


        /// <summary>
        /// The name of the sound effect cue played when the rangedweapon is shoot.
        /// </summary>
        private string creatingCueName;

        /// <summary>
        /// The name of the sound effect cue played when the rangedweapon is shoot.
        /// </summary>
        public string CreatingCueName
        {
            get { return creatingCueName; }
            set { creatingCueName = value; }
        }


        /// <summary>
        /// The name of the sound effect cue played when the rangedweapon effect is traveling.
        /// </summary>
        private string travelingCueName;

        /// <summary>
        /// The name of the sound effect cue played when the rangedweapon effect is traveling.
        /// </summary>
        public string TravelingCueName
        {
            get { return travelingCueName; }
            set { travelingCueName = value; }
        }


        /// <summary>
        /// The name of the sound effect cue played when the rangedweapon affects its target.
        /// </summary>
        private string impactCueName;

        /// <summary>
        /// The name of the sound effect cue played when the rangedweapon affects its target.
        /// </summary>
        public string ImpactCueName
        {
            get { return impactCueName; }
            set { impactCueName = value; }
        }


        /// <summary>
        /// The name of the sound effect cue played when the rangedweapon effect is blocked.
        /// </summary>
        private string blockCueName;

        /// <summary>
        /// The name of the sound effect cue played when the rangedweapon effect is blocked.
        /// </summary>
        public string BlockCueName
        {
            get { return blockCueName; }
            set { blockCueName = value; }
        }


        #endregion


        #region Graphics Data


        /// <summary>
        /// The content path and name of the icon for this rangedweapon.
        /// </summary>
        private string iconTextureName;

        /// <summary>
        /// The content path and name of the icon for this rangedweapon.
        /// </summary>
        public string IconTextureName
        {
            get { return iconTextureName; }
            set { iconTextureName = value; }
        }


        /// <summary>
        /// The icon texture for this rangedweapon.
        /// </summary>
        private Texture2D iconTexture;

        /// <summary>
        /// The icon texture for this rangedweapon.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D IconTexture
        {
            get { return iconTexture; }
        }
        
        
        /// <summary>
        /// The animating sprite used when this rangedweapon is in action.
        /// </summary>
        private AnimatingSprite rangedweaponSprite;

        /// <summary>
        /// The animating sprite used when this rangedweapon is in action.
        /// </summary>
        public AnimatingSprite RangedWeaponSprite
        {
            get { return rangedweaponSprite; }
            set { rangedweaponSprite = value; }
        }


        /// <summary>
        /// The overlay sprite for this rangedweapon.
        /// </summary>
        private AnimatingSprite overlay;

        /// <summary>
        /// The overlay sprite for this rangedweapon.
        /// </summary>
        public AnimatingSprite Overlay
        {
            get { return overlay; }
            set { overlay = value; }
        }


        #endregion


        #region Content Type Reader


        /// <summary>
        /// Read an RangedWeapon object from the content pipeline.
        /// </summary>
        public class RangedWeaponReader : ContentTypeReader<RangedWeapon>
        {
            /// <summary>
            /// Read an RangedWeapon object from the content pipeline.
            /// </summary>
            protected override RangedWeapon Read(ContentReader input, RangedWeapon existingInstance)
            {
                RangedWeapon rangedweapon = existingInstance;
                if (rangedweapon == null)
                {
                    rangedweapon = new RangedWeapon();
                }

                rangedweapon.AssetName = input.AssetName;

                rangedweapon.Name = input.ReadString();
                rangedweapon.Description = input.ReadString();
                rangedweapon.AmmoPointCost = input.ReadInt32();
                rangedweapon.IconTextureName = input.ReadString();
                rangedweapon.iconTexture = input.ContentManager.Load<Texture2D>(
                    System.IO.Path.Combine(@"Textures\RangedWeapons", rangedweapon.IconTextureName));
                rangedweapon.IsOffensive = input.ReadBoolean();
                rangedweapon.TargetDuration = input.ReadInt32();
                rangedweapon.targetEffectRange = rangedweapon.InitialTargetEffectRange = 
                    input.ReadObject<StatisticsRange>();
                rangedweapon.AdjacentTargets = input.ReadInt32();
                rangedweapon.LevelingProgression = input.ReadObject<StatisticsValue>();
                rangedweapon.CreatingCueName = input.ReadString();
                rangedweapon.TravelingCueName = input.ReadString();
                rangedweapon.ImpactCueName = input.ReadString();
                rangedweapon.BlockCueName = input.ReadString();
                rangedweapon.RangedWeaponSprite = input.ReadObject<AnimatingSprite>();
                rangedweapon.RangedWeaponSprite.SourceOffset = new Vector2(
                    rangedweapon.RangedWeaponSprite.FrameDimensions.X / 2,
                    rangedweapon.RangedWeaponSprite.FrameDimensions.Y);
                rangedweapon.Overlay = input.ReadObject<AnimatingSprite>();
                rangedweapon.Overlay.SourceOffset = new Vector2(
                    rangedweapon.Overlay.FrameDimensions.X / 2, 
                    rangedweapon.Overlay.FrameDimensions.Y);

                rangedweapon.Level = 1;

                return rangedweapon;
            }
        }


        #endregion


        #region ICloneable Members


        public object Clone()
        {
            RangedWeapon rangedweapon = new RangedWeapon();

            rangedweapon.adjacentTargets = adjacentTargets;
            rangedweapon.AssetName = AssetName;
            rangedweapon.blockCueName = blockCueName;
            rangedweapon.creatingCueName = creatingCueName;
            rangedweapon.description = description;
            rangedweapon.iconTexture = iconTexture;
            rangedweapon.iconTextureName = iconTextureName;
            rangedweapon.impactCueName = impactCueName;
            rangedweapon.initialTargetEffectRange = initialTargetEffectRange;
            rangedweapon.isOffensive = isOffensive;
            rangedweapon.levelingProgression = levelingProgression;
            rangedweapon.ammoPointCost = ammoPointCost;
            rangedweapon.name = name;
            rangedweapon.overlay = overlay.Clone() as AnimatingSprite;
            rangedweapon.rangedweaponSprite = rangedweaponSprite.Clone() as AnimatingSprite;
            rangedweapon.targetDuration = targetDuration;
            rangedweapon.travelingCueName = travelingCueName;

            rangedweapon.Level = Level;

            return rangedweapon;
        }


        #endregion
    }
}
