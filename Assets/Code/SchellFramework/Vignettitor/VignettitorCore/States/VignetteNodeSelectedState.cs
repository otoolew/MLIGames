//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Alex Pizzini
//  Date:   09/16/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using SG.Vignettitor.Graph;
using SG.Vignettitor.Graph.Drawing;
using SG.Vignettitor.Graph.States;
using SG.Vignettitor.VignetteData;
using UnityEngine;

namespace SG.Vignettitor.VignettitorCore.States
{
    public class VignetteNodeSelectedState : SelectedNodeState
    {
        private readonly Vignettitor vignettitor;

        /// <summary>
        /// The first node in the list of selected nodes for the connected 
        /// vignettitor.
        /// </summary>
        private VignetteNode primarySelection = null;

        /// <summary>
        /// Do all selected nodes have valid children counts?
        /// </summary>
        protected bool validChildren;

        protected VignetteNodeHelpWindow nodeHelpWindow;

        public VignetteNodeSelectedState(Vignettitor renderer)
            : base(renderer)
        {
            vignettitor = editor as Vignettitor;
            supportedCommands.Add("Duplicate");
            supportedCommands.Add("Copy");
            validChildren = ValidateChildCount(false);
        }

        #region -- GraphEditorState Overrides ---------------------------------
        public override void OnSelectionChanged()
        {
            base.OnSelectionChanged();
            validChildren = ValidateChildCount(false);

            if (vignettitor.SelectionManager.AllSelected.Count > 0)
            {
                primarySelection = vignettitor.allNodes[vignettitor.SelectionManager.AllSelected[0]];
            }
            else
            {
                primarySelection = null;
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (nodeHelpWindow != null)
                nodeHelpWindow = nodeHelpWindow.Draw() ? null : nodeHelpWindow;
        }

        public override void CommandDraw()
        {
            base.CommandDraw();

            // Only enable "Skip to Node" and Hardwire if the nodes are 
            // connected to the graph root.
            bool wasEnabled = GUI.enabled;
            //GUI.enabled = true; // vignettitor.IsConnected(primaryNode
            GUI.enabled = vignettitor.head.allNodes.Contains(primarySelection);
            if (GUILayout.Button("Skip To Node"))
            {
                editor.EndOfFrameActions += () =>
                {
                    vignettitor.SkipToNode(primarySelection);
                };
            }
            
            if (GUILayout.Button("Hardwire"))
            {
                vignettitor.HardwirePath(primarySelection);
            }
            GUI.enabled = wasEnabled;

            if (GUILayout.Button("Duplicate Node") || MatchExecuteCommand("Duplicate"))
            {
                vignettitor.DuplicateNodes();
            }

            if (GUILayout.Button("Copy") || MatchExecuteCommand("Copy"))
            {
                vignettitor.Copy();
            }

            if (GUILayout.Button("Node Help"))
            {
                Rect comRect = editor.GetCommandRect();
                Vector2 helpPosition = new Vector2(
                    comRect.x + Event.current.mousePosition.x - VignetteNodeHelpWindow.DEFAULT_WIDTH,
                    comRect.y + Event.current.mousePosition.y);

                nodeHelpWindow = new VignetteNodeHelpWindow(
                    editor.GetNameForType(primarySelection.GetType()), 
                    primarySelection.GetType(), helpPosition);
            }
            if (editor.SelectionManager.AllSelected.Count > 1)
            {
                if (GUILayout.Button("Align in Row"))
                {
                    float y = ((Vignettitor)editor).GetViewState(editor.SelectionManager.AllSelected[0]).Position.y;
                    for (int i = 0; i < editor.SelectionManager.AllSelected.Count; i++)
                        ((Vignettitor) editor).GetViewState(editor.SelectionManager.AllSelected[i]).Position.y = y;
                }
                if (GUILayout.Button("Distribute Horizontal"))
                {
                    Distribute(editor.SelectionManager.AllSelected,
                        editor.visuals.NodeWidth, editor.visuals.NodeXSpace, true);
                }

                if (GUILayout.Button("Align in Column"))
                {
                    float x = ((Vignettitor)editor).GetViewState(editor.SelectionManager.AllSelected[0]).Position.x;
                    for (int i = 0; i < editor.SelectionManager.AllSelected.Count; i++)
                        ((Vignettitor)editor).GetViewState(editor.SelectionManager.AllSelected[i]).Position.x = x;
                }

                if (GUILayout.Button("Distribute Vertical"))
                {
                    Distribute(editor.SelectionManager.AllSelected,
                        editor.visuals.NodeHeight, editor.visuals.NodeYSpace, false);
                }
                
            }

            if (!validChildren)
            {
                Color old = GUI.color;
                GUI.color = vignettitor.visuals.ErrorColor;
                if (GUILayout.Button("Fix Child Count"))
                {
                    ValidateChildCount(true);
                    validChildren = ValidateChildCount(false);
                    vignettitor.OnGraphStructureChange();
                }
                GUI.color = old;
            }                
        }

        /// <summary>
        /// Event spaces nodes to fill their bounds and may expand the bounds 
        /// if the do not fit nicely.
        /// </summary>
        /// <param name="nodes">Nodes to space.</param>
        /// <param name="size">
        /// Width or height of the node size (to ensure a minimum spacing).
        /// </param>
        /// <param name="space">Minimum separation between nodes.</param>
        /// <param name="horiz">
        /// If true, space on the horizontal axis, otherwise vertical.
        /// </param>
        private void Distribute(List<int> nodes, float size, float space, bool horiz)
        {
            List<NodeViewState> views = new List<NodeViewState>();
            Vignettitor v = editor as Vignettitor;
            List<Vector2> positions = new List<Vector2>();
            for (int i = 0; i < nodes.Count; i++)
            {
                views.Add(v.GetViewState(nodes[i]));
                positions.Add(views[views.Count - 1].Position);
            }

            views = horiz ? 
                views.OrderBy(x => x.Position.x).ToList() : 
                views.OrderBy(x => x.Position.y).ToList();

            Rect bounds = v.GetBounds(positions);
            float scale = horiz ? bounds.width : bounds.height;
            if (scale < (positions.Count*(size + space)) - space)
                scale = (positions.Count * (size + space))- space;
            
            float startRange = scale - size;

            float inc = 1.0f / (views.Count - 1);
            for (int i = 0; i < views.Count; i++)
            {
                if (horiz)
                    views[i].Position.x = bounds.x + (inc * i * (startRange));
                else
                    views[i].Position.y = bounds.y + (inc * i * (startRange));
            }

            for (int i = 0; i < nodes.Count; i++)
                actualPositions[i] = (editor.ViewStates[nodes[i]].Position);
        }
        #endregion -- GraphEditorState Overrides ------------------------------

        /// <summary>
        /// Checks if the selected nodes have a valid child count and 
        /// optionally fixes them by culling the child list down to the 
        /// expected max length.
        /// </summary>
        /// <param name="fix">Should invalid nodes be fixed?</param>
        /// <returns>True if all selected nodes have valid children.</returns>
        public bool ValidateChildCount(bool fix)
        {
            bool valid = true;
            for (int i = 0; i < editor.SelectionManager.AllSelected.Count; i++)
            {
                int maxChildren = int.MaxValue;
                VignetteNode n = vignettitor.allNodes[editor.SelectionManager.AllSelected[i]];
                if (n.OutputRule.Rule == OutputRule.RuleType.Passthrough)
                {
                    maxChildren = 1;
                    if (n.Children.Length == 1 && n.Children[0] == null)
                    {
                        valid = false;
                        if (fix)
                            n.Children = new VignetteNode[0];
                    }
                }
                if (n.OutputRule.Rule == OutputRule.RuleType.Static)
                    maxChildren = n.OutputRule.Value;
                if (n.Children.Length > maxChildren)
                {
                    if (fix)
                    {
                        VignetteNode[] newList = new VignetteNode[maxChildren];
                        for (int c = 0; c < newList.Length; c++)
                            newList[c] = n.Children[c];
                        n.Children = newList;
                    }
                    valid = false;
                }
            }
            return valid;
        }
    }
}
