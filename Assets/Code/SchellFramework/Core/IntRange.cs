//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   06/17/2014
//-----------------------------------------------------------------------------

namespace SG.Core
{
    /// <summary>
    /// A range of integers that can be tested against to check if an integer is 
    /// within the range (inclusive). This provodes a custom inspector for
    /// visualizing the range or a set of ranges.
    /// </summary>
    [System.Serializable]
    public class IntRange
    {
        /// <summary>
        /// The lower value of the range.
        /// </summary>
        public int Minimum;

        /// <summary>
        /// The upper value of the range.
        /// </summary>
        public int Maximum;

        /// <summary>
        /// Checks if an integer is contained within this range.
        /// </summary>
        /// <param name="input">Value to test.</param>
        /// <returns>True if input is within this range.</returns>
        public bool Contains(int input)
        {
            return Minimum <= input && input <= Maximum;
        }
    }
}
