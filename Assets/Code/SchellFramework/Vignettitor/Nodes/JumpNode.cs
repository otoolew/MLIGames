//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   08/21/2015
//-----------------------------------------------------------------------------

using SG.Core;
using SG.Vignettitor.Runtime;
using SG.Vignettitor.VignetteData;
using SG.Vignettitor.VignettitorCore;
using UnityEngine;

namespace SG.Vignettitor.Nodes
{
    /// <summary>
    /// Jump nodes transition immediately to their only child node and are 
    /// meant to serve an organizational purpose but not a functional purpose.
    /// </summary>
    [NodeHelp("Jump nodes transition immediately to their only child node and are meant to serve an organizational purpose but not a functional purpose.")]
    [NodeMenu("Jump", typeof(VignetteGraph))]
    public class JumpNode : VignetteNode, INodeCopyPasteHandler
    {
        /// <summary>
        /// Used by the INodeCopyPasteHandler implementation for a custom copy 
        /// and paste handler that maintains jump destinations when copying a 
        /// jump node but not the destination node.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private VignetteNode destinationNode;

        public override OutputRule OutputRule
        { get { return OutputRule.Static(1); } }

        public override bool SupportsLookahead<T>()
        { return true; }

        public class JumpRuntimeNode : VignetteRuntimeNode
        {
            public JumpRuntimeNode(JumpNode node, VignetteRuntimeGraph runtimeGraph)
                : base(node, runtimeGraph) { }

            public override void Enter(object input)
            {
                base.Enter(input);
                Skip();
            }

            public override T Lookahead<T>()
            {
                int length = Source.Children.SafeLength();
                if (length != 1)
                    throw new VignetteDataException(CHILD_MISMATCH_ERROR, length, 1);
                return LookaheadChild<T>(0);
            }

            private T LookaheadChild<T>(int choice) where T : VignetteNode
            {
                VignetteNode child = Source.Children.SafeGet(choice);
                if (!child)
                    return null;
                return child.SupportsLookahead<T>() ? _runtime[child.NodeID].Lookahead<T>() : null;
            }

            [VignetteGenerator(typeof(VignetteGeneratorDelegate), typeof(JumpNode))]
            private static VignetteRuntimeNode Generate(VignetteNode node, VignetteRuntimeGraph runtimeGraph)
            {
                return new JumpRuntimeNode((JumpNode)node, runtimeGraph);
            }
        }

        #region -- INodeCopyPasteHandler Implementation -----------------------
        void INodeCopyPasteHandler.OnWillCopy() {}

        void INodeCopyPasteHandler.OnDidCopy()
        {
            if (Children.Length > 0)
                destinationNode = Children[0];
        }

        void INodeCopyPasteHandler.OnDidPaste(VignettitorDataController source, VignettitorDataController destination)
        {
            if (destinationNode != null && source == destination)
            {
                // TODO: do not copy reference when moving between graphs
                if (Children.Length == 0) 
                    Children = new VignetteNode[1];
                if (Children[0] == null)
                    Children[0] = destinationNode;
                destinationNode = null;
            }
        }
        #endregion -- INodeCopyPasteHandler Implementation --------------------
    }
}
