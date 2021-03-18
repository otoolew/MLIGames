//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   08/21/2015
//-----------------------------------------------------------------------------

using SG.Core;
using SG.Vignettitor.VignetteData;
using Random = UnityEngine.Random;

namespace SG.Vignettitor.Nodes
{
    /// <summary>
    /// Random nodes will immediately exit and pass control to a random child 
    /// node.
    /// </summary>
    [NodeHelp("Random nodes will immediately exit and pass control to a random child node.")]
    [NodeMenu("Random", typeof(VignetteGraph))]
    public class RandomChoiceNode : VignetteNode
    {
        public override OutputRule OutputRule
        {
            get { return OutputRule.Variable(); }
        }

        public override bool SupportsLookahead<T>()
        {
            return true;
        }

        public class RandomChoiceRuntimeNode : VignetteRuntimeNode
        {
            public RandomChoiceRuntimeNode(RandomChoiceNode node, VignetteRuntimeGraph runtimeGraph)
                : base(node, runtimeGraph) { }

            public override void Enter(object input)
            {
                base.Enter(input);
                int choice = GetNextChildIndex();
                if (choice >= 0)
                    SelectChild(choice);
                else
                    EndVignette();
            }

            private int GetNextChildIndex()
            {
                if (!Source.Children.HasAny())
                    return -1;

                // Handle overrides.
                return IsHardwired ? OverrideOutput : Random.Range(0, Source.Children.Length);
            }

            public override T Lookahead<T>()
            {
                int choice = GetNextChildIndex();
                VignetteNode child = Source.Children.SafeGet(choice);
                if (!child)
                    return null;
                return child.SupportsLookahead<T>() ? _runtime[child.NodeID].Lookahead<T>() : null;
            }

            [VignetteGenerator(typeof(VignetteGeneratorDelegate), typeof(RandomChoiceNode))]
            private static VignetteRuntimeNode Generate(VignetteNode node, VignetteRuntimeGraph runtimeGraph)
            {
                return new RandomChoiceRuntimeNode((RandomChoiceNode)node, runtimeGraph);
            }
        }
    }
}
