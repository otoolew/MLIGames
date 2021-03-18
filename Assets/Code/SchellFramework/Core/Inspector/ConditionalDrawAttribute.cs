// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   12/02/2016
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace SG.Core.Inspector
{
    /// <summary>
    /// Conditionally draws a field in the inspector if the value in another 
    /// field passes a comparison. Multiple ConditionalDrawAttribute attributes
    /// may be stacked on the same field for a logical 'AND' result.
    /// </summary>
    /// <example>
    /// Here is an extreme stacking example that demonstrates various 
    /// conditional draw types.
    /// 
    /// <code>
    /// public bool Condition;
    /// public bool OtherCondition;
    /// public float ShouldBePositive;
    /// public float Between10And20;
    /// public int NonZero;
    /// 
    /// [BoolConditionalDraw("Condition")]
    /// [BoolConditionalDraw("OtherCondition", Invert = true)]
    /// [FloatConditionalDraw("ShouldBePositive", ComparisonOperator.GreaterThan, 0.0f)]
    /// [FloatConditionalDraw("Between10And20", ComparisonOperator.GreaterThan, 10.0f)]
    /// [FloatConditionalDraw("Between10And20", ComparisonOperator.LessThan, 20.0f)]
    /// [IntConditionalDraw("NonZero", ComparisonOperator.NotEqual, 0)]
    /// [Tooltip("Will draw if all conditions are met.")]
    /// public string Dependent;
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public abstract class ConditionalDrawAttribute : PropertyAttribute
    {
        /// <summary>
        /// The name of the field that is used to determine if this field is 
        /// drawn.
        /// </summary>
        public string PropertyName;

        /// <summary>
        /// Should the condition be inverted? If this is true, then a property 
        /// condition evaluating to false will make this field draw.
        /// </summary>
        public bool Invert;

        /// <summary>
        /// If this is true, this field will always draw but will be disabled 
        /// when the condition evaluates to false.
        /// </summary>
        public bool AlwaysDraw;
    }

    #region -- Built-in Implementations ---------------------------------------
    /// <summary>
    /// Conditionally draws a target field in the inspector only if another 
    /// field's value is not null.
    /// </summary>
    public class NullConditionalDrawAttribute : ConditionalDrawAttribute
    {
        /// <summary>
        /// Conditionally draws a target field in the inspector only if another 
        /// field's value is not null
        /// </summary>
        /// <param name="conditionalPropertyName">
        /// Name of the float property whose value will be used to 
        /// conditionally draw this field.
        /// </param>
        public NullConditionalDrawAttribute(string conditionalPropertyName)
        { PropertyName = conditionalPropertyName; }
    }

    /// <summary>
    /// Conditionally draws a target field in the inspector only if another 
    /// field's bool value is true (or the invert flag is true).
    /// </summary>
    public class BoolConditionalDrawAttribute : ConditionalDrawAttribute
    {
        /// <summary>
        /// Conditionally draws a target field in the inspector only if another 
        /// field's bool value is true (or the invert flag is true).
        /// </summary>
        /// <param name="conditionalPropertyName">
        /// Name of the float property whose value will be used to 
        /// conditionally draw this field.
        /// </param>
        public BoolConditionalDrawAttribute(string conditionalPropertyName)
        { PropertyName = conditionalPropertyName; }
    }

    /// <summary>
    /// Conditionally draws a target field in the inspector only if another 
    /// field's float value passes a comparison.
    /// </summary>
    public class FloatConditionalDrawAttribute : ConditionalDrawAttribute
    {
        /// <summary> Operation to compare the input to Value. </summary>
        public ComparisonOperator Operator { get; private set; }

        /// <summary> Value to compare to the input. </summary>
        public float Value { get; private set; }

        /// <summary>
        /// Conditionally draws a target field in the inspector only if another
        /// field's float value passes a comparison.
        /// </summary>
        /// <param name="conditionalPropertyName">
        /// Name of the float property whose value will be used to 
        /// conditionally draw this field.
        /// </param>
        /// <param name="op">Operation to compare the input to Value.</param>
        /// <param name="value">Value to compare to the input.</param>
        public FloatConditionalDrawAttribute(string conditionalPropertyName,
            ComparisonOperator op, float value)
        {
            PropertyName = conditionalPropertyName;
            Operator = op;
            Value = value;
        }
    }

    /// <summary>
    /// Conditionally draws a target field in the inspector only if another 
    /// ints's float value passes a comparison.
    /// </summary>
    public class IntConditionalDrawAttribute : ConditionalDrawAttribute
    {
        /// <summary> Operation to compare the input to Value. </summary>
        public ComparisonOperator Operator { get; private set; }

        /// <summary> Value to compare to the input. </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Conditionally draws a target field in the inspector only if another
        /// int's float value passes a comparison.
        /// </summary>
        /// <param name="conditionalPropertyName">
        /// Name of the int property whose value will be used to 
        /// conditionally draw this field.
        /// </param>
        /// <param name="op">Operation to compare the input to Value.</param>
        /// <param name="value">Value to compare to the input.</param>
        public IntConditionalDrawAttribute(string conditionalPropertyName,
            ComparisonOperator op, int value)
        {
            PropertyName = conditionalPropertyName;
            Operator = op;
            Value = value;
        }
    }
    #endregion -- Built-in Implementations ------------------------------------
}