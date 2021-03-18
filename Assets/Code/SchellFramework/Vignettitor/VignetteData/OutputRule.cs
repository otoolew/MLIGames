//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   01/13/2014
//-----------------------------------------------------------------------------

namespace SG.Vignettitor.VignetteData
{
    /// <summary>
    /// There are three patterns that can be used for a nodes output: 
    /// Passthrough - may have 0 or 1 outputs
    /// Static - must have a specific number of outputs, specified with "Value"
    /// Variable - output count may be different based on external variables
    /// </summary>
    public class OutputRule
    {
        /// <summary> Describes behavior of node outputs. </summary>
        public enum RuleType
        {
            /// <summary> May have zero or one outputs. </summary>
            Passthrough,
            /// <summary> A fixed number of outputs. </summary>
            Static,
            /// <summary> Number of outputs may be changed by the user. </summary>
            Variable
        }

        #region -- Private ----------------------------------------------------
        private OutputRule()
        { }

        private static readonly OutputRule passthrough = new OutputRule 
        { Rule = RuleType.Passthrough };

        private static readonly OutputRule variable = new OutputRule 
        { Rule = RuleType.Variable };
        #endregion -- Private -------------------------------------------------

        #region -- Rule Creation ----------------------------------------------
        /// <summary> 
        /// Passthrough output nodes may have 0 or 1 children.
        /// </summary>
        public static OutputRule Passthrough()
        { return passthrough; }

        /// <summary>
        /// Variable output nodes may have any amount of outputs.
        /// </summary>
        public static OutputRule Variable()
        { return variable; }

        /// <summary>
        /// Static output nodes have a fixed number of outputs and all must be
        ///  filled in.
        /// </summary>
        /// <param name="value">Number of outputs.</param>
        public static OutputRule Static(int value)
        { return new OutputRule { Rule = RuleType.Static, Value = value }; }
        #endregion -- Rule Creation -------------------------------------------

        #region -- Properties -------------------------------------------------
        /// <summary>
        /// The output rule for a node type, describing exopectations for the 
        /// number of outputs.
        /// </summary>
        public RuleType Rule { get; private set; }

        /// <summary>
        /// Value to use for static output rule. This is how many outputs would 
        /// always appear on a node.
        /// </summary>
        public int Value { get; private set; }
        #endregion -- Properties ----------------------------------------------
    }
}
