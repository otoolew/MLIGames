//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   05/05/2014
//-----------------------------------------------------------------------------

using SG.Vignettitor.Graph;
using SG.Vignettitor.VignetteData;
using UnityEngine;

namespace SG.Vignettitor.VignettitorCore
{
    /// <summary>
    /// An interface that defines the functions that the vignettitor can 
    /// perform to modify the data it represents.
    /// </summary>
    public abstract class VignettitorDataController
    {
        public VignetteGraph head;
        public virtual void SetVignetteHead(VignetteGraph head)
        { this.head = head; }

        public abstract void UpgradeDataIfNecessary();
        public abstract GraphViewState GetOrCreateViewState();
        public abstract VignetteNode[] GetAllNodeAssets();
        public abstract void RegisterUndo(Object[] objs, string desc);
        public abstract ScriptableObject CreateNewNode(System.Type t);
        public abstract ScriptableObject AddNode(ScriptableObject node);
        public abstract void DeleteNode(Object obj);
        public abstract void RenameNode(Object obj, string name);
        public abstract void SelectNodes(Object[] objects);
        public abstract void SaveData();
    }
}
