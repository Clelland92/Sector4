

#region Using Statements
using System;
using System.Text;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Sector4Data
{
    /// <summary>
    /// A range of character statistics values.
    /// </summary>
    /// <remarks>Typically used for constrained random modifiers.</remarks>
#if WINDOWS
    [Serializable]
#endif
    public class StatisticsRange
    {
        [ContentSerializer(Optional = true)]
        public Int32Range HealthPointsRange;

        [ContentSerializer(Optional = true)]
        public Int32Range AmmoPointsRange;

        [ContentSerializer(Optional = true)]
        public Int32Range PhysicalOffenseRange;

        [ContentSerializer(Optional = true)]
        public Int32Range PhysicalDefenseRange;

        [ContentSerializer(Optional = true)]
        public Int32Range AmmoalOffenseRange;

        [ContentSerializer(Optional = true)]
        public Int32Range AmmoalDefenseRange;


        #region Value Generation


        /// <summary>
        /// Generate a random value between the minimum and maximum, inclusively.
        /// </summary>
        /// <param name="random">The Random object used to generate the value.</param>
        public StatisticsValue GenerateValue(Random random)
        {
            // check the parameters
            Random usedRandom = random;
            if (usedRandom == null)
            {
                usedRandom = new Random();
            }

            // generate the new value
            StatisticsValue outputValue = new StatisticsValue();
            outputValue.HealthPoints = HealthPointsRange.GenerateValue(usedRandom);
            outputValue.AmmoPoints = AmmoPointsRange.GenerateValue(usedRandom);
            outputValue.PhysicalOffense = PhysicalOffenseRange.GenerateValue(usedRandom);
            outputValue.PhysicalDefense = PhysicalDefenseRange.GenerateValue(usedRandom);
            outputValue.AmmoalOffense = AmmoalOffenseRange.GenerateValue(usedRandom);
            outputValue.AmmoalDefense = AmmoalDefenseRange.GenerateValue(usedRandom);

            return outputValue;
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
            sb.Append(HealthPointsRange.ToString());

            sb.Append("; Ammo:");
            sb.Append(AmmoPointsRange.ToString());

            sb.Append("; PO:");
            sb.Append(PhysicalOffenseRange.ToString());

            sb.Append("; PD:");
            sb.Append(PhysicalDefenseRange.ToString());

            sb.Append("; MO:");
            sb.Append(AmmoalOffenseRange.ToString());

            sb.Append("; MD:");
            sb.Append(AmmoalDefenseRange.ToString());

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
            if ((HealthPointsRange.Minimum != 0) || (HealthPointsRange.Maximum != 0))
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
                sb.Append(HealthPointsRange.ToString());
            }

            // add the ammo points value, if any
            if ((AmmoPointsRange.Minimum != 0) || (AmmoPointsRange.Maximum != 0))
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
                sb.Append(AmmoPointsRange.ToString());
            }

            // add the physical offense value, if any
            if ((PhysicalOffenseRange.Minimum != 0) || 
                (PhysicalOffenseRange.Maximum != 0))
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
                sb.Append(PhysicalOffenseRange.ToString());
            }

            // add the physical defense value, if any
            if ((PhysicalDefenseRange.Minimum != 0) || 
                (PhysicalDefenseRange.Maximum != 0))
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
                sb.Append(PhysicalDefenseRange.ToString());
            }

            // add the ammoal offense value, if any
            if ((AmmoalOffenseRange.Minimum != 0) || 
                (AmmoalOffenseRange.Maximum != 0))
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
                sb.Append(AmmoalOffenseRange.ToString());
            }

            // add the ammoal defense value, if any
            if ((AmmoalDefenseRange.Minimum != 0) || 
                (AmmoalDefenseRange.Maximum != 0))
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
                sb.Append(AmmoalDefenseRange.ToString());
            }

            return sb.ToString();
        }


        #endregion


        #region Operator: StatisticsRange + StatisticsValue


        /// <summary>
        /// Add one value to another, piecewise, and return the result.
        /// </summary>
        public static StatisticsRange Add(StatisticsRange value1,
            StatisticsValue value2)
        {
            StatisticsRange outputRange = new StatisticsRange();
            outputRange.HealthPointsRange = 
                value1.HealthPointsRange + value2.HealthPoints;
            outputRange.AmmoPointsRange = 
                value1.AmmoPointsRange + value2.AmmoPoints;
            outputRange.PhysicalOffenseRange = 
                value1.PhysicalOffenseRange + value2.PhysicalOffense;
            outputRange.PhysicalDefenseRange = 
                value1.PhysicalDefenseRange + value2.PhysicalDefense;
            outputRange.AmmoalOffenseRange = 
                value1.AmmoalOffenseRange + value2.AmmoalOffense;
            outputRange.AmmoalDefenseRange =
                value1.AmmoalDefenseRange + value2.AmmoalDefense;
            return outputRange;
        }

        /// <summary>
        /// Add one value to another, piecewise, and return the result.
        /// </summary>
        public static StatisticsRange operator +(StatisticsRange value1,
            StatisticsValue value2)
        {
            return Add(value1, value2);
        }


        #endregion


        #region Content Type Reader


        /// <summary>
        /// Reads a StatisticsRange object from the content pipeline.
        /// </summary>
        public class StatisticsRangeReader : ContentTypeReader<StatisticsRange>
        {
            protected override StatisticsRange Read(ContentReader input, 
                StatisticsRange existingInstance)
            {
                StatisticsRange output = new StatisticsRange();

                output.HealthPointsRange = input.ReadObject<Int32Range>();
                output.AmmoPointsRange = input.ReadObject<Int32Range>();
                output.PhysicalOffenseRange = input.ReadObject<Int32Range>();
                output.PhysicalDefenseRange = input.ReadObject<Int32Range>();
                output.AmmoalOffenseRange = input.ReadObject<Int32Range>();
                output.AmmoalDefenseRange = input.ReadObject<Int32Range>();

                return output;
            }
        }


        #endregion
    }
}
