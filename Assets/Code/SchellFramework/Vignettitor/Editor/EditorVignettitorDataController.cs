//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   05/08/2014
//-----------------------------------------------------------------------------

using SG.Vignettitor.Graph;
using SG.Vignettitor.VignetteData;
using SG.Vignettitor.VignettitorCore;
using System.Collections.Generic;
using System.Linq;
using SG.Core;
using SG.Core.Contracts;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor
{
    /// <summary>
    /// Data controller used for vignettes when accessed in the editor. This 
    /// allows for full access and modification of vignette data.
    /// </summary>
    public class EditorVignettitorDataController : VignettitorDataController
    {
        /// <summary>
        /// Should view state data be returned as a clone? Enabling this
        /// prevents view state data from being changed unless "SaveData"
        /// is called.
        /// </summary>
        public bool UseViewStateCopy;

        public bool HideSubAssets;

        /// <summary>
        /// Wrking copy of the view state data if UseViewStateCopy is true.
        /// </summary>
        private GraphViewState viewStateCopy;

        private int nextID;

        public static void ChangeGraphVisibility(VignetteGraph graph, HideFlags visibility)
        {
            Object[] objects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(graph));
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    if (!typeof(VignetteGraph).IsAssignableFrom(objects[i].GetType()))
                    {
                        objects[i].hideFlags = visibility;
                        EditorUtility.SetDirty(objects[i]);
                    }
                }
            }
            AssetDatabase.SaveAssets();
        }

        private int GetNextNodeID()
        {
            if (nextID == 0)
                GetAllNodeAssets();
            int result = nextID;
            nextID++;
            return result;
        }

        public override VignetteNode[] GetAllNodeAssets()
        {
            List<VignetteNode> result = new List<VignetteNode>();
            string path = AssetDatabase.GetAssetPath(head);
            Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);

            List<int> usedIDs = new List<int>();
            usedIDs.Add(0);
            List<VignetteNode> invalidIDNodes = new List<VignetteNode>();
            nextID = 0;
            int max = 0;
            for (int i = 0; i < objs.Length; i++)
            {
                VignetteNode n = objs[i] as VignetteNode;
                if (n != null)
                {
                    result.Add(n);
                    if (usedIDs.Contains(n.NodeID))
                        invalidIDNodes.Add(n);
                    else
                        usedIDs.Add(n.NodeID);
                    max = Mathf.Max(n.NodeID, max);
                }
            }
            nextID = Mathf.Max(nextID, max) + 1;

            for (int i = 0; i < invalidIDNodes.Count; i++)
                invalidIDNodes[i].NodeID = GetNextNodeID();

            return result.OrderBy(v => v.NodeID).ToArray();
        }

        public override void RegisterUndo(Object[] objs, string desc)
        {
            Undo.RecordObjects(objs, desc);
        }

        public static GraphViewState GetGraphViewState(string path)
        {
            Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
            for (int i = 0; i < objects.Length; i++)
                if (objects[i] != null && typeof(GraphViewState).IsAssignableFrom(objects[i].GetType()))
                    return objects[i] as GraphViewState;
            return null;
        }

        /// <summary>
        /// If the associated vignetet graph has a save format that is older 
        /// than the required version, the data will be upgraded to the latest
        /// format and the version number will be updated.
        /// </summary>
        public override void UpgradeDataIfNecessary()
        {
            // TODO: make a more generic upgrade process
            // This is just the process for upgrading from 0 to 1.
            if (head.SaveFormatVersion < VignettitorVersion.SAVE_FORMAT_VERSION)
            {
                Debug.Log("Upgrading vignette save format version of " + 
                    AssetDatabase.GetAssetPath(head) + " from " +
                    head.SaveFormatVersion + " to " + 
                    VignettitorVersion.SAVE_FORMAT_VERSION);

                VignetteNode[] nodes = GetAllNodeAssets();
                GraphViewState vs = GetOrCreateViewState();

                // In version 0, the arrays were expected to be parallel. This
                // follows that assumption and correctly tags each view with 
                // the node it represents.
                for (int i = 0; i < nodes.Length; i++)
                    if (i < vs.NodeViewStates.Count)
                        vs.NodeViewStates[i].ID = nodes[i].NodeID;

                // If the node views got out of sync and there are too many,
                // trim the end off.
                int dif = vs.NodeViewStates.Count - nodes.Length;
                if (dif > 0)
                    vs.NodeViewStates.RemoveRange(vs.NodeViewStates.Count - dif, dif);

                head.SaveFormatVersion = VignettitorVersion.SAVE_FORMAT_VERSION;
            }
        }

        public override ScriptableObject CreateNewNode(System.Type t)
        {
            string path = AssetDatabase.GetAssetPath(head);
            GraphViewState view = GetGraphViewState(path);
            RegisterUndo(new Object[] { head, view }, "Create Node");
            ScriptableObject result = ScriptableObject.CreateInstance(t);
            VignetteNode n = result as VignetteNode;
            if (n != null)
                n.NodeID = GetNextNodeID();
            result.name = t.Name;
            if (HideSubAssets)
                result.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(result, head);
            Undo.RegisterCreatedObjectUndo(result, "Create Node");
            EditorUtility.SetDirty(head);
            return result;
        }

        public override GraphViewState GetOrCreateViewState()
        {
            string path = AssetDatabase.GetAssetPath(head);

            GraphViewState result = GetGraphViewState(path);
            if (result == null)
            {
                result = ScriptableObject.CreateInstance<GraphViewState>();
                result.name = "_ViewState";
                if (HideSubAssets)
                    result.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(result, head);
                EditorUtility.SetDirty(head);
                AssetDatabase.SaveAssets();
            }
            EditorUtility.SetDirty(head);

            // Make a copy if necessary to prevent editing the actual data.
            if (UseViewStateCopy)
            {
                viewStateCopy = Object.Instantiate(result);
                viewStateCopy.name = result.name;
                if (HideSubAssets)
                    viewStateCopy.hideFlags = HideFlags.HideInHierarchy;
                result = viewStateCopy;
            }

            return result;
        }

        public override void DeleteNode(Object obj)
        {
            // Find all ISubordinateAsset objects that this node used
            SerializedObject so = new SerializedObject(obj);
            SerializedPropertyMap<ISubordinateAsset> map = 
                SubordinateAssetUtility.GetSubordinateAssets(so.GetIterator());

            // Destroy the node
            Undo.DestroyObjectImmediate(obj);

            // Build a list of referenced objects, avoiding duplicates.
            List<Object> objectsToDelete = new List<Object>();            
            for (int i = 0; i < map.GetCount(); i++)
            {
                Object o = map.GetEntry(i).Object as Object;
                if (!objectsToDelete.Contains(o))
                    objectsToDelete.Add(o);
            }

            // Destroy each unique ISubordinateAsset object.
            for (int i = 0; i < objectsToDelete.Count; i++)
                Undo.DestroyObjectImmediate(objectsToDelete[i]);

            EditorUtility.SetDirty(head);
        }

        public override void RenameNode(Object obj, string name)
        {
            Undo.RecordObject(obj, "Rename " + name);
            obj.name = name;
            EditorUtility.SetDirty(obj);
        }

        public override void SelectNodes(Object[] objects)
        {
            Selection.objects = objects;
        }

        public override void SaveData()
        {
            // If working with a copy but a hard save is requested, 
            // get the actual data and write the copy into it.
            if (UseViewStateCopy)
            {
                UseViewStateCopy = false;
                GraphViewState actualViewState = GetOrCreateViewState();
                SerializedObject destSO = new SerializedObject(actualViewState);
                SerializedObject sourceSO = new SerializedObject(viewStateCopy);
                Copy(sourceSO, destSO);
                UseViewStateCopy = true;
            }
            string path = AssetDatabase.GetAssetPath(head);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            head.name = name;
            head.SaveFormatVersion = VignettitorVersion.SAVE_FORMAT_VERSION;

            EditorUtility.SetDirty(head);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Copy all data from the source object to the dest object.
        /// </summary>
        /// <param name="source">The SerializedObject to copy.</param>
        /// <param name="dest">The SerializedObject to recieve data.</param>
        private static void Copy(SerializedObject source, SerializedObject dest)
        {
            SerializedProperty sp = source.GetIterator();
            do
            {
                if (!string.IsNullOrEmpty(sp.propertyPath))
                    dest.CopyFromSerializedProperty(sp);
            }
            while (sp.Next(true));
            dest.ApplyModifiedProperties();
        }

        public override ScriptableObject AddNode(ScriptableObject node)
        {
            VignetteNode n = node as VignetteNode;
            if (n != null)
                n.NodeID = GetNextNodeID();
            if (HideSubAssets)
                node.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(node, head);
            EditorUtility.SetDirty(head);
            return node;
        }
    }
}
