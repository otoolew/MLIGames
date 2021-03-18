// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   12/09/2016
// ----------------------------------------------------------------------------

using System;

namespace SG.Core.Inspector
{
    /// <summary>
    /// Base class for comparable range sets, used as a hook into a property 
    /// drawer. It is expected that only ValueRangeSet extend this class.
    /// </summary>
    public abstract class BaseValueRangeSet {}

    /// <summary>
    /// Base generic implementation for mathcing input to the index of a 
    /// pre-specified range.  
    /// </summary>
    /// <typeparam name="TComparison">
    /// The Comparison type used to determine ranges. There will be an array of 
    /// these representing the different tests.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The Range type used in the comparison type.
    /// </typeparam>
    /// <typeparam name="TInput">
    /// The type used in the range. This is the input into the test.
    /// </typeparam>
    public abstract class ValueRangeSet<TComparison, TValue, TInput> : BaseValueRangeSet
        where TComparison : ValueComparison<TValue, TInput>
        where TValue : IComparable
        where TInput : IComparable
    {
        /// <summary>
        /// The list of ranges used to test input. Input will be tested against 
        /// these ranges in sequence.
        /// </summary>
        public TComparison[] Ranges = new TComparison[0];

        /// <summary>
        /// Get the index of the range (defined by "Ranges") that the given 
        /// input falls in. The ranges are tested sequentially so it will 
        /// return the first result that matches.
        /// </summary>
        /// <param name="input">Argument to test agains Ranges.</param>
        /// <returns>
        /// The index of the matched range or -1 if the input does not fall 
        /// within any range.
        /// </returns>
        public int GetIndex(TInput input)
        {
            for (int i = 0; i < Ranges.Length; i++)
            {
                if (Ranges[i].Evaluate(input))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Checks if an input fits in to any of the ranges.
        /// </summary>
        /// <param name="input">Value to test.</param>
        /// <returns>
        /// False if GetIndex(input) would return -1, otherwise true.
        /// </returns>
        public bool MatchesAny(TInput input)
        {
            return GetIndex(input) != -1;
        }
    }

    #region -- Default RangeSet Implementations -------------------------------
    /// <summary>
    /// Allows for matching an input float to an index in a float range list.
    /// 
    /// For example, a content creator can set up ranges:
    /// less than 0.0, [0.0 : 10.0), [10.0 : 20.0), >= 20.0
    /// and then test input for a range.
    /// -1.0 returns 0, 6.0 returns 1, 16.0 returns 2, 50.0 returns 3.
    /// </summary>
    [Serializable]
    public class FloatRangeSet : ValueRangeSet<FloatRangeComparison, FloatRange, float> {}

    /// <summary>
    /// Allows for matching an input int to an index in a int range list.
    /// 
    /// For example, a content creator can set up ranges:
    /// less than 0, [0 : 10), [10 : 20), >= 20
    /// and then test input for a range.
    /// -1 returns 0, 6 returns 1, 16 returns 2, 50 returns 3.
    /// </summary>
    [Serializable]
    public class IntRangeSet : ValueRangeSet<IntRangeComparison, IntRange, int> {}

    /// <summary>
    /// Represents a set of alphabetical ranges and allows for testing input 
    /// for each range. 
    /// 
    /// For example, this can be used to set up ranges A - K and K - Z. Then 
    /// an input string can be tested to see what range it falls in. "Code" is 
    /// in group 0 and and "MonoBehaviour" is in group 1. 
    /// </summary>
    [Serializable]
    public class StringRangeSet : ValueRangeSet<StringRangeComparison, StringRange, string> {}
    #endregion -- Default RangeSet Implementations ----------------------------
}