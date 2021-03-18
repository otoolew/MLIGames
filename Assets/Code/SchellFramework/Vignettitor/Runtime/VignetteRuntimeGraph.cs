// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 3/1/2016 10:39:19 AM
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SG.Core;
using SG.Entities;
using SG.Vignettitor.Runtime;
using SG.Vignettitor.VignetteData;
using UnityEngine;

namespace SG.Vignettitor
{
    /// <summary>
    /// Encapsulates a method that is executed when the graph
    /// has finished executing.
    /// </summary>
    /// <param name="lastNodeId">Id of the last node executed.</param>
    public delegate void GraphExecutionFinished(int lastNodeId, object transferredData);

    /// <summary>
    /// Method executed when graph enters a new node.
    /// </summary>
    /// <param name="enteredNode"></param>
    public delegate void NodeEntered(VignetteRuntimeNode enteredNode);

    [Serializable]
    public class VignetteRuntimeGraph : IGraphResolver
    {
        [SerializeField]
        private VignetteGraph _source;
        public VignetteGraph Source { get { return _source; } }

        /// <summary>
        /// Used during playback, this stores the id of the node currently in control.
        /// </summary>
        [SerializeField]
        private int _currentNodeId = -1;
        public int CurrentNodeId { get { return _currentNodeId; } }

        /// <summary>
        /// Used during playback, this stores an ordered list of all visited 
        /// nodes.
        /// </summary>
        [SerializeField]
        private List<int> _visitedNodes = new List<int>();
        public List<int> VisitedNodes { get { return _visitedNodes; } }

        private Dictionary<int, VignetteRuntimeNode> _runtimeNodes;

        /// <summary>
        /// Triggered when the Vignette finishes forward execution.
        /// The first parameter is the last NodeId of execution.
        /// The second is the transferredData on exit of that node.
        /// </summary>
        public event GraphExecutionFinished Finished;

        /// <summary>
        /// Triggered when the Vignette enters any node.
        /// </summary>
        public event NodeEntered NodeEntered;
		
		private readonly IVignetteTracker _tracker;

        public VignetteRuntimeGraph(VignetteGraph graph)
        {
            _source = graph;
            _runtimeNodes = GenerateRuntimeNodes(graph);
            _tracker = Services.Locate<IVignetteTracker>();
        }

        public bool Valid { get { return _runtimeNodes != null && _source; } }

        public bool IsPlaying { get { return Valid && _currentNodeId >= 0; } }

        private Dictionary<int, VignetteRuntimeNode> GenerateRuntimeNodes(VignetteGraph graph)
        {
            // construct the VignetteRuntimeNodes for this graph
            Dictionary<int, VignetteRuntimeNode> runtimeNodes = new Dictionary<int, VignetteRuntimeNode>(graph.allNodes.Count);
            for (int i = 0; i < graph.allNodes.Count; i++)
            {
                VignetteNode node = graph.allNodes[i];
                VignetteGeneratorDelegate generator = VignetteGeneratorAttribute.GetGeneratorDelegate<VignetteGeneratorDelegate>(node);
                if (generator == null)
                    throw new VignetteRuntimeException(
                        string.Concat("Unable to create generator for VignetteRuntimeGraph for NodeType ",
                            node.GetType().Name));
                runtimeNodes[node.NodeID] = generator.Invoke(node, this);
            }
            return runtimeNodes;
        }

        [CanBeNull]
        public VignetteRuntimeNode this[int id]
        {
            get
            {
                VignetteRuntimeNode runtime;
                return _runtimeNodes.TryGetValue(id, out runtime) ? runtime : null;
            }
        }

        /// <summary>
        /// Called before a graph begins execution, this will initialize all 
        /// nodes on the graph and enter the entry node.
        /// </summary>
        public virtual void Start(GraphInvocation invocation = null, IBinderSource binderSource = null, object transferredData = null)
        {
            _currentNodeId = -1;
            _visitedNodes.Clear();
            Invocation = invocation ?? GraphInvocation.Empty;
            BinderSource = binderSource ?? Entities.BinderSource.Empty;
            Dictionary<int, VignetteRuntimeNode>.Enumerator enumerator = _runtimeNodes.GetEnumerator();
            while (enumerator.MoveNext())
                enumerator.Current.Value.OnBeforeGraphStart();

            // Start tracking execution for the editor
            _tracker.Track(this);

            SetNode(_source.Entry.NodeID, transferredData);
        }

        public virtual void Update()
        {
            VignetteRuntimeNode currentNode;
            if (_runtimeNodes.TryGetValue(_currentNodeId, out currentNode))
                currentNode.Update();
        }

        /// <summary>
        /// Called when a graph has ended, giving all nodes a chance to clean 
        /// up changes they have made.
        /// </summary>
        protected virtual void End(object transferredData)
        {
            int lastNodeId = _currentNodeId;
            _currentNodeId = -1;
            // we do not clear visited nodes until the vignette is restarted
            Dictionary<int, VignetteRuntimeNode>.Enumerator enumerator = _runtimeNodes.GetEnumerator();
            while (enumerator.MoveNext())
                enumerator.Current.Value.OnBeforeGraphEnd();

            if (Finished != null)
                Finished.Invoke(lastNodeId, transferredData);

            // Stop tracking execution for the editor
            _tracker.UnTrack(this);
        }

        /// <summary>
        /// The function to call in order to specify the next VignetteNode
        /// that should be active.
        /// </summary>
        public virtual void SetNode(int nodeId, object transferredData = null)
        {
            VignetteRuntimeNode currentNode;
            _runtimeNodes.TryGetValue(_currentNodeId, out currentNode);

            // If there is a current node, transfer the output and exit.
            if (currentNode != null)
                transferredData = currentNode.Exit();

            VignetteRuntimeNode nextNode;
            _runtimeNodes.TryGetValue(nodeId, out nextNode);

            // if there is no next node, end the vignette
            if (nextNode == null)
            {
                End(transferredData);
                return;
            }

            _currentNodeId = nodeId;
            _visitedNodes.Add(_currentNodeId);
            nextNode.Enter(transferredData);
            if (NodeEntered != null)
                NodeEntered.Invoke(nextNode);
        }

        public virtual void SetHardwire(int nodeId, int overrideOutput)
        {
            _runtimeNodes[nodeId].OverrideOutput = overrideOutput;
        }

        public virtual void ToggleHardwire(int nodeId, int overrideOutput)
        {
            if (_runtimeNodes[nodeId].OverrideOutput == overrideOutput)
                _runtimeNodes[nodeId].OverrideOutput = -1;
            else
                _runtimeNodes[nodeId].OverrideOutput = overrideOutput;
        }

        public virtual void ClearHardwire()
        {
            Dictionary<int, VignetteRuntimeNode>.Enumerator enumerator = _runtimeNodes.GetEnumerator();
            while (enumerator.MoveNext())
                enumerator.Current.Value.OverrideOutput = -1;
        }

        #region IGraphResolver

        public string DebugPath { get { return _source.VignettePath; } }

        public IBinderSource BinderSource { get; private set; }
        
        public GraphInvocation Invocation { get; private set; }

        public T Resolve<T>(int nodeId, string propertyName, T fallback)
        {
            return _source.Signature.Resolve(nodeId, propertyName, Invocation, fallback);
        }

        public T ResolveGraphScope<T>(string propertyName, T fallback)
        {
            return Invocation.Resolve<T>(propertyName, fallback);
        }

        #endregion  
    }

    public interface IGraphResolver
    {
        string DebugPath { get; }
        IBinderSource BinderSource { get; }
        GraphInvocation Invocation { get; }
        T Resolve<T>(int nodeId, string propertyName, T fallback);
        T ResolveGraphScope<T>(string propertyName, T fallback);
    }
}
