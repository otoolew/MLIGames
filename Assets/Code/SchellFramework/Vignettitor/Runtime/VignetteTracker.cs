// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 7/11/2016 1:22:28 PM
// ------------------------------------------------------------------------------

using System.Collections.Generic;
using SG.Core;
using SG.Vignettitor.VignetteData;

namespace SG.Vignettitor.Runtime
{
    /// <summary>
    /// IVignetteTracking service which tracks all currently executing vignettes.
    /// Used by the editor since multiple runtimes can exist for a single vignette now.
    /// </summary>
    [ServiceOnDemand(ServiceType = typeof(IVignetteTracker))]
    public class VignetteTracker : IVignetteTracker
    {
        private readonly Dictionary<VignetteGraph, List<VignetteRuntimeGraph>> _activeRuntimes =
            new Dictionary<VignetteGraph,List<VignetteRuntimeGraph>>();

        public void Track(VignetteRuntimeGraph runtime)
        {
            List<VignetteRuntimeGraph> runtimes;
            if (!_activeRuntimes.TryGetValue(runtime.Source, out runtimes))
                runtimes = _activeRuntimes[runtime.Source] = new List<VignetteRuntimeGraph>();
            if (!runtimes.Contains(runtime))
                runtimes.Add(runtime);
        }

        public void UnTrack(VignetteRuntimeGraph runtime)
        {
            List<VignetteRuntimeGraph> runtimes;
            if (!_activeRuntimes.TryGetValue(runtime.Source, out runtimes))
                return;
            runtimes.Remove(runtime);
        }

        public List<VignetteRuntimeGraph> GetRuntimes(VignetteGraph graph)
        {
            List<VignetteRuntimeGraph> runtimes;
            return !_activeRuntimes.TryGetValue(graph, out runtimes) ? null : runtimes;
        }
    }

    public interface IVignetteTracker : IService
    {
        void Track(VignetteRuntimeGraph runtime);
        void UnTrack(VignetteRuntimeGraph runtime);
        List<VignetteRuntimeGraph> GetRuntimes(VignetteGraph graph);
    }
}
