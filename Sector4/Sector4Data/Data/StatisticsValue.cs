

#region Using Statements
using System;
using System.Text;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Sector4Data
{
    /// <summary>
    /// The set of relevant statistics for characters.
    /// </summary>
#if WINDOWS
    [Serializable]
#endif
    public struct StatisticsValue
    {
        [ContentSerializer(Optional = true)]
        public Int32 HealthPoints;

        [ContentSerializer(Optional = true)]
        public Int32 AmmoPoints;

        [ContentSerializer(Optional = true)]
        public Int32 PhysicalOffense;

        [ContentSerializer(Optional = true)]
        public Int32 PhysicalDefense;

        [ContentSerializer(Optional = true)]
        public Int32 AmmoalOffense;

        [ContentSerializer(Optional = true)]
        public Int32 AmmoalDefense;


        /// <summary>
        /// Returns true if this object is trivial - all values at zero.
        /// </summary>
        public bool IsZero
        {
            get
            {
                return ((HealthPoints == 0) && (AmmoPoints == 0) &&
                    (PhysicalOffense == 0) && (PhysicalDefense == 0) &&
                    (AmmoalOffense == 0) && (AmmoalDefense == 0));
            }
        }


        #region Initialization


        /// <summary>
        /// Create a new StatisticsValue object, fully specified by the parameters.
        /// </summary>
        public StatisticsValue(int healthPoints, int ammoPoints, int physicalOffense,
            int physicalDefense, int ammoalOffense, int ammoalDefense)
        {
            HealthPoints = healthPoints;
            AmmoPoints = ammoPoints;
            PhysicalOffense = physicalOffense;
            PhysicalDefense = physicalDefense;
            AmmoalOffense = ammoalOffense;
            AmmoalDefense = ammoalDefense;
        }


        #endregion


        #region Operator: StatisticsValue + StatisticsValue


        /// <summary>
        /// Add one value to another, piecewise, and return the result.
        /// </summary>
        public static StatisticsValue Add(StatisticsValue value1, 
            StatisticsValue value2)
        {
            StatisticsValue outputValue = new StatisticsValue();
            outputValue.HealthPoints = 
                value1.HealthPoints + value2.HealthPoints;
            outputValue.AmmoPoints = 
                value1.AmmoPoints + value2.AmmoPoints;
            outputValue.PhysicalOffense = 
                value1.PhysicalOffense + value2.PhysicalOffense;
            outputValue.PhysicalDefense = 
                value1.PhysicalDefense + value2.PhysicalDefense;
            outputValue.AmmoalOffense = 
                value1.AmmoalOffense + value2.AmmoalOffense;
            outputValue.AmmoalDefense = 
                value1.AmmoalDefense + value2.AmmoalDefense;
            return outputValue;
        }

        /// <summary>
        /// Add one value to another, piecewise, and return the result.
        /// </summary>
        public static StatisticsValue operator +(StatisticsValue value1, 
            StatisticsValue value2)
        {
            return Add(value1, value2);
        }


        #endregion


        #region Operator: StatisticsValue - StatisticsValue


        /// <summary>
        /// Subtract one value from another, piecewise, and return the result.
        /// </summary>
        public static StatisticsValue Subtract(StatisticsValue value1, 
            StatisticsValue value2)
        {
            StatisticsValue outputValue = new StatisticsValue();
            outputValue.HealthPoints =
                value1.HealthPoints - value2.HealthPoints;
            outputValue.AmmoPoints =
                value1.AmmoPoints - value2.AmmoPoints;
            outputValue.PhysicalOffense =
                value1.PhysicalOffense - value2.PhysicalOffense;
            outputValue.PhysicalDefense =
                value1.PhysicalDefense - value2.PhysicalDefense;
            outputValue.AmmoalOffense =
                value1.AmmoalOffense - value2.AmmoalOffense;
            outputValue.AmmoalDefense =
                value1.AmmoalDefense - value2.AmmoalDefense;
            return outputValue;
        }

        /// <summary>
        /// Subtract one value from another, piecewise, and return the result.
        /// </summary>
        public static StatisticsValue operator -(StatisticsValue value1, 
            StatisticsValue value2)
        {
            return Subtract(value1, value2);
        }


        #endregion


        // Compound assignment (+=, etc.) operators use the overloaded binary operators,
        // so there is no need in this case to override them explicitly


        #region Limiting


        /// <summary>
        /// Clamp all values piecewise with the provided minimum values.
        /// </summary>
        public void ApplyMinimum(StatisticsValue minimumValue)
        {
            HealthPoints = Math.Max(HealthPoints, minimumValue.HealthPoints);
            AmmoPoints = Math.Max(AmmoPoints, minimumValue.AmmoPoints);
            PhysicalOffense = Math.Max(PhysicalOffense, minimumValue.PhysicalOffense);
            PhysicalDefense = Math.Max(PhysicalDefense, minimumValue.PhysicalDefense);
            AmmoalOffense = Math.Max(AmmoalOffense, minimumValue.AmmoalOffense);
            AmmoalDefense = Math.Max(AmmoalDefense, minimumValue.AmmoalDefense);
        }


        /// <summary>
        /// Clamp all values piecewise with the provided maximum values.
        /// </summary>
        public void ApplyMaximum(StatisticsValue maximumValue)
        {
            HealthPoints = Math.Min(HealthPoints, maximumValue.HealthPoints);
            AmmoPoints = Math.Min(AmmoPoints, maximumValue.AmmoPoints);
            PhysicalOffense = Math.Min(PhysicalOffense, maximumValue.PhysicalOffense);
            PhysicalDefense = Math.Min(PhysicalDefense, maximumValue.PhysicalDefense);
            AmmoalOffense = Math.Min(AmmoalOffense, maximumValue.AmmoalOffense);
            AmmoalDefense = Math.Min(AmmoalDefense, maximumValue.AmmoalDefense);
        }


        #endregion


        #region String Output


        /// <summary>
        /// Builds a string that describes this object.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("HP:");
            sb.Append(HealthPoints.ToString());

            sb.Append("; Ammo:");
            sb.Append(AmmoPoints.ToString());

            sb.Append("; PO:");
            sb.Append(PhysicalOffense.ToString());

            sb.Append("; PD:");
            sb.Append(PhysicalDefense.ToString());

            sb.Append("; MO:");
            sb.Append(AmmoalOffense.ToString());

            sb.Append("; MD:");
            sb.Append(AmmoalDefense.ToString());

            return sb.ToString();
        }


        /// <summary>
        /// Builds a string that describes a modifier, where non-zero stats are skipped.
        /// </summary>
        public string GetModifierString()
        {
            StringBuilder sb = new StringBuilder();
            bool firstStatistic = true;

            // add the health points value, if any
            if (HealthPoints != 0)
            {
                if (firstStatistic)
                {
                    firstStatistic = false;
                }
                else
                {
                    sb.Append("; ");
                }
                sb.Append("HP:");
                sb.Append(HealthPoints.ToString());
            }

            // add the ammo points value, if any
            if (AmmoPoints != 0)
            {
                if (firstStatistic)
                {
                    firstStatistic = false;
                }
                else
                {
                    sb.Append("; ");
                }
                sb.Append("Ammo:");
                sb.Append(AmmoPoints.ToString());
            }

            // add the physical offense value, if any
            if (PhysicalOffense != 0)
            {
                if (firstStatistic)
                {
                    firstStatistic = false;
                }
                else
                {
                    sb.Append("; ");
                }
                sb.Append("PO:");
                sb.Append(PhysicalOffense.ToString());
            }

            // add the physical defense value, if any
            if (PhysicalDefense != 0)
            {
                if (firstStatistic)
                {
                    firstStatistic = false;
                }
                else
                {
                    sb.Append("; ");
                }
                sb.Append("PD:");
                sb.Append(PhysicalDefense.ToString());
            }

            // add the ammoal offense value, if any
            if (AmmoalOffense != 0)
            {
                if (firstStatistic)
                {
                    firstStatistic = false;
                }
                else
                {
                    sb.Append("; ");
                }
                sb.Append("MO:");
                sb.Append(AmmoalOffense.ToString());
            }

            // add the ammoal defense value, if any
            if (AmmoalDefense != 0)
            {
                if (firstStatistic)
                {
                    firstStatistic = false;
                }
                else
                {
                    sb.Append("; ");
                }
                sb.Append("MD:");
                sb.Append(AmmoalDefense.ToString());
            } 
            
            return sb.ToString();
        }


        #endregion


        #region Content Type Reader


        public class StatisticsValueReader : ContentTypeReader<StatisticsValue>
        {
            protected override StatisticsValue Read(ContentReader input, 
                StatisticsValue existingInstance)
            {
                StatisticsValue output = new StatisticsValue();

                output.HealthPoints = input.ReadInt32();
                output.AmmoPoints = input.ReadInt32();
                output.PhysicalOffense = input.ReadInt32();
                output.PhysicalDefense = input.ReadInt32();
                output.AmmoalOffense = input.ReadInt32();
                output.AmmoalDefense = input.ReadInt32();

                return output;
            }
        }


        #endregion
    }
}
