// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Eric Policaro
// 
//  Date: 02/15/2016
// ----------------------------------------------------------------------------

using System;
using SG.Dynamics;
using SG.Vignettitor.Graph;
using SG.Vignettitor.VignetteData;
using UnityEngine;

namespace SG.Vignettitor.Nodes
{
    [NodeMenu("State/Outlet")]
    [NodeHelp("Defines a named output for a graph that will be used as a subgraph.")]
    public class OutletNode : VignetteNode
    {
        public const string DefaultOutletName = "Exit";

        /// <summary>
        /// Label for the output pin.
        /// </summary>
        [Header("Named Output Parameters")]
        [Tooltip("Name for the connection output label.")]
        public string OutletName = DefaultOutletName;

        /// <summary>
        /// Value that determines the order outlets will be
        /// arranged in a parent graph (lower numbers at the top).
        /// </summary>
        [Tooltip("Lower numbers will be listed higher in the parent graph")]
        public int Order;

        /// <summary>
        /// Return value of the graph.
        /// </summary>
        [SerializeField]
        private DynamicValue _output;
        public DynamicValue Output { get { return _output; } }

        /// <summary>
        /// Should the return value of the graph be a passthrough from
        /// the node previous to this one?
        /// </summary>
        [SerializeField]
        private bool _outputIsPassthrough;
        public bool OutputIsPassthrough { get { return _outputIsPassthrough; } }

        public override OutputRule OutputRule { get { return OutputRule.Static(0); } }

        public override ContentValidation Validate()
        {
            ContentValidation result = base.Validate();
            if (string.IsNullOrEmpty(OutletName))
            {
                result.Warning(this, "(ID:{0}) Outlet has no name", NodeID);
            }

            return result;
        }

        private void OnValidate()
        {
            if (_output != null)
                _output.ClearDeserializedValue();
        }

        private void Reset()
        {
            OutletName = DefaultOutletName;
        }

        public class Runtime : VignetteRuntimeNode
        {
            private OutletNode _source;
            private object _passthroughData;

            public Runtime(OutletNode node, VignetteRuntimeGraph runtimeGraph)
                : base(node, runtimeGraph)
            {
                _source = node;
            }

            public override void Enter(object input)
            {
                base.Enter(input);
                if (_source._outputIsPassthrough)
                    _passthroughData = input;
                Skip();
            }

            public override object Exit()
            {
                return _source.OutputIsPassthrough ? new DynamicValue(_passthroughData) : _source.Output;
            }

            [VignetteGenerator(typeof(VignetteGeneratorDelegate), typeof(OutletNode))]
            private static VignetteRuntimeNode Generate(VignetteNode node, VignetteRuntimeGraph runtimeGraph)
            {
                return new Runtime((OutletNode)node, runtimeGraph);
            }
        }
    }
}