// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   12/02/2016
// ----------------------------------------------------------------------------

using System;

namespace SG.Core.Inspector
{
    /// <summary> Options for comparing values to one another. </summary>
    public enum ComparisonOperator
    {
        LessThan,
        LessThanOrEqual,
        Equal,
        NotEqual,
        GreaterThanOrEqual,
        GreaterThan
    }

    /// <summary>
    /// Base class for ValueComparison that allows for it to be referenced 
    /// without a generic type, exposes an object based Evaluate function, and
    /// makes it a candidate for a property drawer since it is a class that is 
    /// Serializable.
    /// </summary>
    [Serializable]
    public abstract class BaseValueComparison
    {
        /// <summary>
        /// How to display each enum of the ComparisonOperator enumeration.
        /// </summary>
        public static readonly string[] OPERATOR_DISPLAY = { "<", "<=", "==", "!=", ">=", ">" };
    }

    /// <summary>
    /// The base class for serializable ValueComparisons that allow content 
    /// creators to specify value comparison logic in the inspector to drive 
    /// a game.
    /// </summary>
    /// <typeparam name="TValue">
    /// The IComparable type stored internally to use for comparison.
    /// </typeparam>
    /// <typeparam name="TInput">
    /// The type that may be compared to this object.
    /// </typeparam>
    public abstract class ValueComparison<TValue, TInput> : BaseValueComparison where TValue : IComparable where TInput : IComparable
    {
        #region -- Inspector Variables ----------------------------------------
        /// <summary>
        /// The operator to use to compare this value to other data.
        /// </summary>
        public ComparisonOperator Operator = ComparisonOperator.LessThan;

        /// <summary>  The value to compare to other data.  </summary>
        public TValue Value = default(TValue);
        #endregion -- Inspector Variables -------------------------------------

        /// <summary>
        /// Compares two IComparables according to the given operator.
        /// </summary>
        /// <param name="input">Left-hand side of the comparison.</param>
        /// <param name="op">Comparison operator.</param>
        /// <param name="value">Right-hand side of the comparison.</param>
        /// <returns>
        /// Result of comparing input to value using the given operator.
        /// </returns>
        public static bool Evaluate(TInput input, ComparisonOperator op, TValue value)
        {
            // We compare them inversly so we then have to flip the operators. 
            // This is done since the stored value on a ValueComparison may be 
            // a more complex type than the input. For example T value may be 
            // of type FloatRange and K input would be a float.
            int result = value.CompareTo(input);
            switch (op)
            {
                case ComparisonOperator.LessThan:
                    return (result > 0);
                case ComparisonOperator.LessThanOrEqual:
                    return (result >= 0);
                case ComparisonOperator.Equal:
                    return (result == 0);
                case ComparisonOperator.NotEqual:
                    return (result != 0);
                case ComparisonOperator.GreaterThanOrEqual:
                    return (result <= 0);
                case ComparisonOperator.GreaterThan:
                    return (result < 0);

                default:
                    return false;
            }
        }

        /// <summary>
        /// Compares the input to the 'Value' field of this ValueComparison.
        /// </summary>
        /// <param name="input">What to compare to 'Value'.</param>
        /// <returns>Comparison result.</returns>
        public virtual bool Evaluate(TInput input)
        {
            return Evaluate(input, Operator, Value);
        }

        public override string ToString()
        {
            return OPERATOR_DISPLAY[(int) Operator] + " " + Value;
        }
    }

    #region -- Default ValueComparison Implementations ------------------------
    /// <summary>  Handles comparing two floats. </summary>
    [Serializable]
    public class FloatComparison : ValueComparison<float, float> {}

    /// <summary>  Handles comparing two ints. </summary>
    [Serializable]
    public class IntComparison : ValueComparison<int, int> {}

    /// <summary> 
    /// Handles comparing two strings. For example:
    /// a != b
    /// a is less than b
    /// </summary>
    [Serializable]
    public class StringComparison : ValueComparison<string, string> {}

    /// <summary>  Handles comparing two bools. </summary>
    [Serializable]
    public class BoolComparison : ValueComparison<bool, bool> {}

    /// <summary>  
    /// Handles comparing an FloatRange to a float. This is useful for testing 
    /// if a float is in a range, below a range or above a range. For Example:
    /// -1 is less than [0 : 10)
    /// 5 == [0 : 10)
    /// 10 != [0 : 10)
    /// 10 == [0 : 10]
    /// </summary>
    [Serializable]
    public class FloatRangeComparison : ValueComparison<FloatRange, float>
    {
        public override bool Evaluate(float input)
        {
            if (Value.IsEmptySet())
                return false;
            return base.Evaluate(input);
        }
    }

    /// <summary>  
    /// Handles comparing an IntRange to an int. This is useful for testing if 
    /// a float is in a range, below a range or above a range. For Example:
    /// -1 is less than [0 : 10)
    /// 5 == [0 : 10)
    /// 10 != [0 : 10)
    /// 10 == [0 : 10]
    /// </summary>
    [Serializable]
    public class IntRangeComparison : ValueComparison<IntRange, int>
    {
        public override bool Evaluate(int input)
        {
            if (Value.IsEmptySet())
                return false;
            return base.Evaluate(input);
        }
    }

    /// <summary>
    /// Handles comparing a stringRange to a string. This is useful for testing
    /// if a string falls alphabetically within a range. For Example:
    /// a is less than b
    /// b == [a : c)  - meaning b is in the range a through c
    /// c != [a : c)  - meaning c is not in the range a through c
    /// c == [a : c]  - meaning c is not in the range a to(including) c
    /// </summary>
    [Serializable]
    public class StringRangeComparison : ValueComparison<StringRange, string>
    {
        public override bool Evaluate(string input)
        {
            if (Value.IsEmptySet())
                return false;
            return base.Evaluate(input);
        }
    }
    #endregion -- Default ValueComparison Implementations ---------------------
}