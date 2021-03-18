// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 6/9/2016 2:56:31 PM
// ------------------------------------------------------------------------------

using SG.Core;
using SG.Entities;
using SG.Vignettitor.Runtime;
using SG.Vignettitor.VignetteData;
using UnityEngine;

namespace SG.Vignettitor
{
    /// <summary>
    /// Simple component that can own a Vignette and play it.
    /// </summary>
    public class VignettePlayerExecutor : MonoBehaviour
    {
        protected static readonly Notify Log = NotifyManager.GetInstance<VignettePlayerExecutor>();

        #region -- Protected Fields -------------------------------------------
        /// <summary>
        /// Graph we will play when no graph is specified in Play()
        /// </summary>
        [SerializeField]
        protected VignetteGraph _defaultGraph;

        [SerializeField]
        protected GraphInvocation _invocation;

        /// <summary>
        /// The vignette Graph that the player is currently executing. This 
        /// will be null when nothing is executing.
        /// </summary>
        [SerializeField]
        protected VignetteGraph _graph;

        /// <summary>
        /// Owned VignettePlayer (holds runtime state)
        /// </summary>
        [SerializeField]
        protected VignettePlayer _player;
        #endregion -- Protected Fields ----------------------------------------

        #region -- Public Properties ------------------------------------------
        /// <summary> Is any vignette playing at the moment? </summary>
        public bool IsPlaying { get { return _player.IsPlaying; } }
        #endregion -- Public Properties ---------------------------------------

        public void Awake()
        {
            if (_player == null)
                _player = new VignettePlayer();
        }

        /// <summary>
        /// Start executing the given vignette. Note that the execution may 
        /// complete (and therefore the current Graph may be set to null) 
        /// before the end of this function if all nodes complete in a single 
        /// frame.
        /// </summary>
        /// <param name="graph">The Graph to start.</param>
        /// <param name="invocation">GraphInvocation parameters to use for this invocation.</param>
        public void StartVignette(VignetteGraph graph, GraphInvocation invocation)
        {
            enabled = true;
            _graph = graph;
            _player.StartVignette(_graph, invocation, new BinderSource(gameObject));
        }

        protected virtual void Update()
        {
            if (!IsPlaying)
            {
                enabled = false;
                return;
            }
            _player.Update();
        }

        [ContextMenu("Play Default Graph")]
        public void PlayDefault()
        {
            if (!_defaultGraph)
            {
                Log.Error(this, "No DefaultGraph specified for Play");
                return;
            }

            Play(_defaultGraph);
        }

        public virtual void Play(VignetteGraph graph)
        {
            if (!graph)
            {
                Log.Error(this, "No Graph specified for Play");
                return;
            }

            if (IsPlaying)
            {
                Log.Error(this, "Graph {0} still Playing", _graph.SafeName());
                return;
            }

            StartVignette(graph, _invocation);
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            if (IsPlaying)
                _player.Stop();
        }
    }
}
