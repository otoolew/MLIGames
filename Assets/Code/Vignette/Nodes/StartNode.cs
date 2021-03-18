// ------------------------------------------------------------------------------
//  Copyright © 2021 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Billy Pete
//
//  Created: 3/16/2021 5:58:57 PM
// ------------------------------------------------------------------------------

using SG.Vignettitor;
using SG.Vignettitor.NodeViews;
using SG.Vignettitor.Graph;
using SG.Vignettitor.Graph.NodeViews;
using SG.Vignettitor.VignetteData;
using UnityEngine;

namespace SG.MLIGames
{
    /// <summary>
    /// The Start of A node
    /// </summary>
    [NodeHelp("The Start of A node")]
    [NodeMenu("Start Node", typeof(SG.Vignettitor.VignetteData.VignetteGraph))]
    public class StartNode : VignetteNode
    {
        public override OutputRule OutputRule { get { return OutputRule.Passthrough(); } }

        public override ContentValidation Validate()
        {
            ContentValidation cv = base.Validate();
            return cv;
        }
    }

    [NodeView(typeof(StartNode))]
    public class StartView : VignetteNodeView
    {
        protected override Color DefaultColor { get { return new Color(1.00f, 0.58f, 0.58f, 1.00f); } }

        public override void Draw(Rect rect)
        {
            StartNode n = Node as StartNode;
            base.Draw(rect);
        }
    }

    public class StartRuntime : VignetteRuntimeNode
    {
        private StartNode _source;

        protected StartRuntime(StartNode node, VignetteRuntimeGraph graph)
                : base(node, graph)
        {
            _source = node;
        }

        public override void Enter(object input)
        {
            base.Enter(input);

            Log.Debug("Start Node Rocking!");

            Skip();
        }

        [VignetteGenerator(typeof(VignetteGeneratorDelegate), typeof(StartNode))]
        private static VignetteRuntimeNode Generate(VignetteNode node, VignetteRuntimeGraph graph)
        {
            return new StartRuntime((StartNode)node, graph);
        }
    }
}