// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Eric Policaro
// 
//  Date: 02/15/2016
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using SG.Vignettitor.Graph;
using SG.Vignettitor.Runtime;
using SG.Vignettitor.VignetteData;
using UnityEngine;

namespace SG.Vignettitor.Nodes
{
    [NodeMenu("State/Subgraph")]
    [NodeHelp("Encapsulates a graph as a node. Output pins are added for each Outlet Node contained in the graph")]
    public class SubgraphNode : VignetteNode, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The attached graph.
        /// </summary>
        public VignetteGraph Graph;

        [SerializeField]
        private GraphInvocation _invocation;
        public GraphInvocation Invocation { get { return _invocation; } }

        [SerializeField]
        private GraphInvocation.GraphInvocationRemap[] _invocationRemaps;
        public GraphInvocation.GraphInvocationRemap[] InvocationRemaps { get { return _invocationRemaps; } }

        [SerializeField] // why serialize this?
        private int _outputCount;

        /// <summary>
        /// Gets the named outputs for the subgraph. Named outputs
        /// are defined in the attached graph by <see cref="OutletNode"/>.
        /// </summary>
        public List<OutletNode> Outlets { get; private set; }

        public Dictionary<int, int> OutletMap { get; private set; }

        /// <summary>
        /// Specifies how many outputs a node is expected to have.
        /// </summary>
        public override OutputRule OutputRule
        {
            get { return OutputRule.Static(_outputCount); }
        }

        public override ContentValidation Validate()
        {
            ContentValidation result = base.Validate();
            if (Graph == null)
                result.Error(this, "(ID:{0}) No sub graph attached.", NodeID);
            else
            {
                foreach (VignetteNode node in Graph.allNodes)
                {
                    var subgraph = node as SubgraphNode;
                    if (subgraph != null)
                    {
                        if (subgraph.Graph == Graph)
                        {
                            result.Error(this, "(ID:{0}) Recursive subgraph detected, not validating.", NodeID);
                        }
                        else
                            result.Add(node.Validate());
                    }
                    else
                        result.Add(node.Validate());
                }
                    

                result.Add(Graph.Signature.ValidateInvocation(_invocation,
                    new HashSet<string>(_invocationRemaps.Select(x => x.SubParameterName))));
            }

            return result;
        }

        public void OnAfterDeserialize()
        {
            RefreshOutlets();
        }

        public void OnEnable()
        {
            RefreshOutlets();
        }

        public void OnBeforeSerialize()
        {
            RefreshOutlets();
        }

        private void RefreshOutlets()
        {
            if (!Graph || !Graph.Entry)
            {
                Outlets = new List<OutletNode>();
                _outputCount = 0;
                return;
            }

            Outlets = Graph.Entry.GetAllChildrenOfTypeRecursively<OutletNode>();
            Outlets.Sort((a, b) => a.Order.CompareTo(b.Order));
            _outputCount = Outlets.Count;

            OutletMap = new Dictionary<int, int>(_outputCount);
            for (int i = 0; i < Outlets.Count; i++)
                OutletMap[Outlets[i].NodeID] = i;
        }

        public class SubgraphRuntimeNode : VignetteRuntimeNode
        {
            private SubgraphNode _source;
            private VignetteRuntimeGraph _subruntime;
            private object _subgraphTransferredData;

            public SubgraphRuntimeNode(SubgraphNode node, VignetteRuntimeGraph runtimeGraph)
                : base(node, runtimeGraph)
            {
                _source = node;
            }

            /// <summary>
            /// Begins control of this node.
            /// </summary>
            /// <param name="input">An object that may be used by any node.</param>
            public override void Enter(object input)
            {
                base.Enter(input);

                if (!_source.Graph || !_source.Graph.Entry)
                    throw new VignetteDataException("Subgraph Graph or Graph Entry missing");

                _subruntime = new VignetteRuntimeGraph(_source.Graph);

                GraphInvocation dynamicInvocation = _source.Invocation.CloneForSubGraph(
                    _runtime.Invocation, _source.InvocationRemaps);
                _subruntime.Finished += OnSubgraphFinished;
                _subruntime.Start(dynamicInvocation, _runtime.BinderSource, input);
            }

            private void OnSubgraphFinished(int lastNodeId, object transferredData)
            {
                _subruntime.Finished -= OnSubgraphFinished;
                
                _subgraphTransferredData = transferredData;
                int child;
                _source.OutletMap.TryGetValue(lastNodeId, out child);
                SelectChildOrExit(child);
            }

            public override void Update()
            {
                base.Update();
                _subruntime.Update();
            }

            public override object Exit()
            {
                if (_subruntime == null || !_subruntime.IsPlaying)
                    return _subgraphTransferredData;

                _subruntime.Finished -= OnSubgraphFinished;
                _subruntime.SetNode(-1);

                return _subgraphTransferredData;
            }

            [VignetteGenerator(typeof(VignetteGeneratorDelegate), typeof(SubgraphNode))]
            private static VignetteRuntimeNode Generate(VignetteNode node, VignetteRuntimeGraph runtimeGraph)
            {
                return new SubgraphRuntimeNode((SubgraphNode)node, runtimeGraph);
            }
        }
    }
}