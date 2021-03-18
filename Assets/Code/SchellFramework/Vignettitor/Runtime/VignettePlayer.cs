//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   02/20/2015
//-----------------------------------------------------------------------------

using System;
using SG.Entities;
using SG.Vignettitor.VignetteData;

namespace SG.Vignettitor.Runtime
{
    /// <summary>
    /// A simple implementation of a system that can execute the instructions 
    /// defined in a vignette graph. This implementation allows for a single 
    /// vignette to be executing at a time per VignettePlayer instance.
    /// 
    /// Many usages of the vignette system may benefit from writing a different 
    /// implementation of a vignette player.
    /// </summary>
    [Serializable]
    public class VignettePlayer
    {
        #region -- Protected Fields -------------------------------------------
        /// <summary>
        /// The vignette graph that the player is currently executing. This 
        /// will be null when nothing is executing.
        /// </summary>
        protected VignetteRuntimeGraph _runtimeGraph;
        #endregion -- Protected Fields ----------------------------------------

        #region -- Public Properties ------------------------------------------
        /// <summary> Is any vignette playing at the moment? </summary>
        public bool IsPlaying { get { return _runtimeGraph != null && _runtimeGraph.IsPlaying; } }

        public event GraphExecutionFinished Finished;
        #endregion -- Public Properties ---------------------------------------

        /// <summary>
        /// Start executing the given vignette. Note that the execution may 
        /// complete (and therefore the current graph may be set to null) 
        /// before the end of this function if all nodes complete in a single 
        /// frame.
        /// </summary>
        /// <param name="graph">The graph to start.</param>
        /// <param name="invocation"></param>
        /// <param name="binderSource"></param>
        public void StartVignette(VignetteGraph graph, GraphInvocation invocation = null, IBinderSource binderSource = null)
        {
            graph.CollectConnectedNodes();
            _runtimeGraph = new VignetteRuntimeGraph(graph);
            _runtimeGraph.Finished += OnVignetteFinished;
            _runtimeGraph.Start(invocation, binderSource);
        }

        public virtual void Update()
        {
            if (IsPlaying)
                _runtimeGraph.Update();
        }

        public void Stop()
        {
            if (IsPlaying)
                _runtimeGraph.SetNode(-1);
        }

        protected virtual void OnVignetteFinished(int lastNodeId, object transferredData)
        {
            _runtimeGraph = null;
            if (Finished != null)
                Finished.Invoke(lastNodeId, transferredData);
        }
    }
}
