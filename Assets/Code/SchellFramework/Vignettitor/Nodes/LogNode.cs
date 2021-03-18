//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   08/21/2015
//-----------------------------------------------------------------------------

using SG.Vignettitor.Graph;
using SG.Vignettitor.NodeViews;
using SG.Vignettitor.VignetteData;
using UnityEngine;

namespace SG.Vignettitor.Nodes
{
    /// <summary>
    /// Log Nodes output a debug log message and immediately transition to the
    /// next node.
    /// </summary>
    [NodeHelp("Log Nodes output a debug log message and immediately transition to the next node.")]
    [NodeMenu("Log", typeof(VignetteGraph))]
    public class LogNode : VignetteNode
    {
        [NodeViewField]
        [NodeEditViewField(true, AutoFocus = true, DrawFieldName = false)]
        [TextArea]
        [Tooltip("Text to output to the console when the node is entered.")]
        public string Message;

        public override OutputRule OutputRule
        {
            get { return OutputRule.Passthrough(); }
        }

        public override ContentValidation Validate()
        {
            ContentValidation cv = base.Validate();
            if (string.IsNullOrEmpty(Message))
                cv.Warning(this, "LogNode {0} has an empty message.", this);
            return cv;
        }

        public class LogRuntimeNode : VignetteRuntimeNode
        {
            private LogNode _source;

            public LogRuntimeNode(LogNode node, VignetteRuntimeGraph runtimeGraph)
                : base(node, runtimeGraph)
            {
                _source = node;
            }

            public override void Enter(object input)
            {
                base.Enter(input);
                Debug.Log(_runtime.Resolve(_source.NodeID, "Message", _source.Message));
                Skip();
            }

            [VignetteGenerator(typeof(VignetteGeneratorDelegate), typeof(LogNode))]
            private static VignetteRuntimeNode Generate(VignetteNode node, VignetteRuntimeGraph runtimeGraph)
            {
                return new LogRuntimeNode((LogNode)node, runtimeGraph);
            }
        }
    }
}
