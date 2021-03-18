//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   03/05/2015
//-----------------------------------------------------------------------------

using System.Linq;
using SG.Vignettitor.Graph;
using SG.Vignettitor.VignetteData;
using SG.Vignettitor.VignettitorCore;
using UnityEngine;

namespace SG.Vignettitor.Runtime
{
    /// <summary>
    /// Data controller to use in a vignettitor view that operates within a 
    /// game's runtime. This can access but can not modify any data on disk.
    /// </summary>
    public class RuntimeVignettitorDataController : VignettitorDataController
    {
        /// <summary>
        /// Returns a copy of the view state so that it can not affect the 
        /// stored asset.
        /// </summary>
        /// <returns>A View State copy.</returns>
        public override GraphViewState GetOrCreateViewState()
        {
            GraphViewState result = Resources.Load(head.VignettePath, typeof(GraphViewState)) as GraphViewState;
            if (result == null)
            {
                result = ScriptableObject.CreateInstance<GraphViewState>();
                result.name = "_ViewState";
            }
            return result;
        }

        /// <summary>
        /// Creates a new node but does not write it to disk.
        /// </summary>
        /// <param name="t">Type of node to create.</param>
        /// <returns>A node of the given type.</returns>
        public override ScriptableObject CreateNewNode(System.Type t)
        {
            ScriptableObject result = ScriptableObject.CreateInstance(t);
            result.name = t.Name;
            return result;
        }

        /// <summary> Deletes the supplied node. </summary>
        /// <param name="obj">Node to delete.</param>
        public override void DeleteNode(Object obj)
        {
            Object.DestroyImmediate(obj, true);
        }

        public override void UpgradeDataIfNecessary()
        { }

        public override void RenameNode(Object obj, string name)
        { obj.name = name; }

        public override void SelectNodes(Object[] objects)
        {}

        public override void SaveData()
        {}

        public override void RegisterUndo(Object[] objs, string desc)
        {}

        public override VignetteNode[] GetAllNodeAssets()
        {
            VignetteNode[] result = head.GetAllNodes();
            return result.OrderBy(v => v.NodeID).ToArray();
        }

        /// <summary>
        /// Runtime Editor does not allow adding a node to an existing head 
        /// asset.
        /// </summary>
        /// <param name="node">Ignored.</param>
        /// <returns>Always returns null.</returns>
        public override ScriptableObject AddNode(ScriptableObject node)
        {
            return null;
        }
    }
}
