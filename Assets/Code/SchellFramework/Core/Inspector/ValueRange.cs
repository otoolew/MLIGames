// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   12/07/2016
// ----------------------------------------------------------------------------

using System;

namespace SG.Core.Inspector
{
    /// <summary>
    /// Base class for representing a range of values for purposes of editor 
    /// drawing. Nothing except ValueRange should inherit from this.
    /// </summary>
    public abstract class BaseRange {}

    /// <summary>
    /// Base class for representing a range of values and testing values 
    /// against that range.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value used in the range. Must be IComparable.
    /// </typeparam>
    [Serializable]
    public class ValueRange<T> : BaseRange, IComparable where T : IComparable
    {
        #region -- Inspector Variables ----------------------------------------
        /// <summary> Is the Min Value included in the range? </summary>
        public bool MinInclusive = true;

        /// <summary> Is the Max Value included in the range? </summary>
        public bool MaxInclusive;

        /// <summary> The minimum value in the range. </summary>
        public T Min;

        /// <summary> The maximum value in the range. </summary>
        public T Max;
        #endregion -- Inspector Variables -------------------------------------

        #region -- Constructors -----------------------------------------------
        /// <summary> Construct a new range of the form [min : max). </summary>
        /// <param name="min">Low end of the range, inclusive.</param>
        /// <param name="max">High end of the range, exclusive.</param>
        public ValueRange(T min, T max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Construct a range with explicit inclusiveness of min and max.
        /// </summary>
        /// <param name="min">Low end for the range.</param>
        /// <param name="minInclusive">Should the min be included?</param>
        /// <param name="max">High end for the range.</param>
        /// <param name="maxInclusive">should the max be included?</param>
        public ValueRange(T min, bool minInclusive, T max, bool maxInclusive)
        {
            Min = min;
            Max = max;
            MinInclusive = minInclusive;
            MaxInclusive = maxInclusive;
        }
        #endregion -- Constructors --------------------------------------------

        /// <summary> Check if a value is within the range. </summary>
        /// <param name="input">Value to check.</param>
        /// <returns>True if the value is in the range.</returns>
        public bool IsInRange(T input)
        {
            return IsInRange(input, MinInclusive, MaxInclusive);
        }

        /// <summary> 
        /// Checks if a value is in the range. This allows for alternative min 
        /// and max inclusiveness to be used for the comparison.
        /// </summary>
        /// <param name="input">Value to check.</param>
        /// <param name="minInclusive">
        /// Should the min value be included in the range?
        /// </param>
        /// <param name="maxInclusive">
        /// Should the max value be included in the range?
        /// </param>
        /// <returns>True if the value is in the range.</returns>
        public bool IsInRange(T input, bool minInclusive, bool maxInclusive)
        {
            if (IsEmptySet())
                return false;

            int minResult = input.CompareTo(Min);
            bool minTest = minInclusive
                ? minResult >= 0
                : minResult > 0;

            int maxResult = input.CompareTo(Max);
            bool maxTest = maxInclusive
                ? maxResult <= 0
                : maxResult < 0;

            return minTest && maxTest;
        }

        /// <summary>
        /// Compare a value to the range. Values within the range are ==,
        /// values outside the range are !=. Values lower than the min are less 
        /// than and values greater than the max are greater than.
        /// </summary>
        /// <param name="obj">Object to compare, must cast to T.</param>
        /// <returns>
        /// Result of comparison to the range and the min and max values. If 
        /// the object can not be cast to T, returns -1.
        /// </returns>
        public virtual int CompareTo(object obj)
        {
            if (obj is T)
            {
                int minResult = ((T) obj).CompareTo(Min);
                int maxResult = ((T) obj).CompareTo(Max);
                if (MinInclusive)
                {
                    if (minResult < 0)
                        return 1;
                }
                else
                {
                    if (minResult <= 0)
                        return 1;
                }

                if (MaxInclusive)
                {
                    if (maxResult > 0)
                        return -1;
                }
                else
                {
                    if (maxResult >= 0)
                        return -1;
                }
                return 0;
            }

            return -1;
        }

        /// <summary>
        /// Does this range represent the empty set? By definition, this is 
        /// true in two cases:
        ///  - Min greater than Max - [4 : 0)
        ///  - Min == Max and either Min or Max are not inclusive - [4 : 4)
        ///
        /// A range is not the empty set if 
        ///  - Min less than Max - [0 : 10)
        ///  - Min and Max are equal but both are inclusive - [5 : 5]
        /// </summary>
        /// <returns>True if there are no elements in this range.</returns>
        public bool IsEmptySet()
        {
            int minToMax = Min.CompareTo(Max);
            if (minToMax > 0)
                return true;
            if (minToMax == 0)
                return !(MinInclusive && MaxInclusive);
            return false;
        }

        public override string ToString()
        {
            return (MinInclusive ? "[" : "(") + 
                Min + " : " + Max + 
                (MaxInclusive ? "]" : ")");
        }
    }

    #region -- Default ValueRange Implementations -----------------------------
    /// <summary>
    /// Represents a range of integers and allows for other values to be tested 
    /// against the range.
    /// </summary>
    [Serializable]
    public class IntRange : ValueRange<int>
    {
        public IntRange(int min, int max) : base(min, max) {}

        public IntRange(int min, bool minInclusive, int max, bool maxInclusive)
            : base(min, minInclusive, max, maxInclusive) { }

        /// <summary>
        /// Gets a random int within the range.
        /// </summary>
        /// <param name="useInclusivness">
        /// Should this rangins min and max inclusivity be used for the random 
        /// range? The defaul is minInclusiv, maxExclusive.
        /// </param>
        /// <returns>A random value within the range.</returns>
        public int GetRandomValue(bool useInclusivness = true)
        {
            if (useInclusivness)
            {
                int min = MinInclusive ? Min : Min + 1;
                int max = MaxInclusive ? Max + 1 : Max;
                return UnityEngine.Random.Range(min, max);
            }
            return UnityEngine.Random.Range(Min, Max);
        }

        /// <summary>
        /// Get a random float within the int range. This will be min and max 
        /// inclusive.
        /// </summary>
        /// <returns>A random float within [min : max].</returns>
        public float GetRandomFloatValue()
        {
            return UnityEngine.Random.Range((float)Min, (float)Max);
        }
    }

    [Serializable]
    public class FloatRange : ValueRange<float>
    {
        public FloatRange(float min, float max) : base(min, max) { }
        public FloatRange(float min, bool minInclusive, float max, bool maxInclusive)
            : base(min, minInclusive, max, maxInclusive) { }

        public float GetRandomValue()
        {
            return UnityEngine.Random.Range(Min, Max);
        }
    }

    [Serializable]
    public class StringRange : ValueRange<string>
    {
        public StringRange(string min, string max) : base(min, max) { }
        public StringRange(string min, bool minInclusive, string max, bool maxInclusive)
            : base(min, minInclusive, max, maxInclusive) { }
    }
    #endregion -- Default ValueRange Implementations --------------------------
}