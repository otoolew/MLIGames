//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   10/03/2014
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SG.Vignettitor.Graph
{
    /// <summary>
    /// Manages node selection for a graph editor. This tracks marquee dragging 
    /// status and lists node that are selected or may be selected.
    /// </summary>
    public class NodeSelectionManager
    {
        #region -- Private Fields ---------------------------------------------
        /// <summary> Origin of the marquee when it is being drawn. </summary>
        private Vector2 marqueeStart;

        /// <summary>
        /// Action to call when the manager selects the first node.
        /// </summary>
        private readonly Action selectionActivated;

        /// <summary> Action to call whenever the selection changes. </summary>
        private readonly Action selectionChanged;

        /// <summary> Graph editor that this manages selections for. </summary>
        private readonly GraphEditor editor;
        #endregion -- Private Fields ------------------------------------------

        #region -- Properties -------------------------------------------------
        /// <summary> All of the node indices that are currently selected. </summary>
        public List<int> AllSelected { get; protected set; }

        /// <summary> Nodes under the marquee but not yet selected. </summary>
        public List<int> PotentialSelections { get; protected set; }
        
        /// <summary>
        /// Is the marquee active (darrging a selection box)?
        /// </summary>
        public bool MarqueeActive { get; protected set; }

        /// <summary> Bounds of the marquee if it is being dragged. </summary>
        public Rect MarqueeRect { get; protected set; }
        #endregion -- Properties ----------------------------------------------

        /// <summary> Create a new node selection manager. </summary>
        /// <param name="editor">
        /// Graph editor that this manages selections for.
        /// </param>
        /// <param name="selectionActivated">
        /// Action to call when the manager selects the first node.
        /// </param>
        /// <param name="selectionChanged">
        /// Action to call whenever the selection changes.
        /// </param>
        public NodeSelectionManager(GraphEditor editor, 
            Action selectionActivated, Action selectionChanged)
        {
            this.editor = editor;
            this.selectionActivated = selectionActivated;
            this.selectionChanged = selectionChanged;
            AllSelected = new List<int>();
            PotentialSelections = new List<int>();
        }

        #region -- Selection Changes ------------------------------------------
        /// <summary>
        /// If the node is not selected, select it, otherwise unselect it. 
        /// This will trigger a selection changed.
        /// </summary>
        /// <param name="id">ID of the node to toggle.</param>
        public void ToggleSelected(int id)
        {
            if (AllSelected.Contains(id))
                AllSelected.Remove(id);
            else
                AllSelected.Add(id);

            selectionChanged();
        }

        /// <summary>
        /// Gets a list of all of the selected node IDs.
        /// </summary>
        /// <returns>IDs of al selected nodes.</returns>
        public List<int> GetAllSelectedIDs()
        {
            List<int> result = new List<int>();
            for(int i = 0 ; i < AllSelected.Count; i++)
                result.Add(editor.GetIDByIndex(AllSelected[i]));
            return result;
        }

        /// <summary>
        /// Adds a node to the selection, triggering a selection changed and 
        /// potentially a selection activated.
        /// </summary>
        /// <param name="index">index of the node to add.</param>
        public void AddToSelection(int index)
        {
            if (AllSelected.Count == 0)
                if (selectionActivated != null)
                    selectionActivated();

            if (!AllSelected.Contains(index))
                AllSelected.Add(index);

            selectionChanged();
        }

        public void CleanSelection(int max)
        {
            for (int i = 0; i < AllSelected.Count; i++)
            {
                int index = AllSelected[i];
                if (index < max)
                    InternalRemove(AllSelected[i]);
            }
        }

        private void InternalRemove(int id)
        {
            if (AllSelected.Contains(id))
                AllSelected.Remove(id);
        }

        /// <summary>
        /// Remove a node from the selection, triggering a selection change.
        /// </summary>
        /// <param name="id">ID of the node to remove.</param>
        public void RemoveFromSelection(int id)
        {
            InternalRemove(id);
            selectionChanged();
        }
        
        /// <summary>
        /// Select all of the nodes in the associated editor.
        /// </summary>
        public void SelectAll()
        {
            for (int i = 0; i < editor.ViewStates.Count; i++)
                AddToSelection(i);
        }

        /// <summary>
        /// Remove everything from the selection, triggering selection changed.
        /// </summary>
        public void Clear()
        {
            AllSelected.Clear();
            selectionChanged();
        }

        /// <summary> Checks if a node is selected. </summary>
        /// <param name="index">index of the node to check.</param>
        /// <returns>True if the node is selected.</returns>
        public bool IsSelected(int index)
        { return AllSelected.Contains(index); }
        #endregion -- Selection Changes ---------------------------------------

        #region -- Marquee ----------------------------------------------------
        /// <summary> Begins selecting nodes with a marquee box. </summary>
        /// <param name="position">Position to begin selection.</param>
        /// <param name="additive">
        /// Should this add to the selection or start over?
        /// </param>
        public void BeginMarqueeSelect(Vector2 position, bool additive)
        {
            if (!additive)
                Clear();
            marqueeStart = position;
            MarqueeActive = true;
            Update(position);
        }

        /// <summary>
        /// Stop the marquee selection, transitioning all potentially selected 
        /// nodes to selected nodes.
        /// </summary>
        /// <param name="position">Position to end the slection at.</param>
        public void EndMarqueeSelect(Vector2 position)
        {
            MarqueeActive = false;
            for (int index = 0; index < PotentialSelections.Count; index++)
                AddToSelection(PotentialSelections[index]);

            PotentialSelections.Clear();
        }

        /// <summary>
        /// Updates the selection marquee, if active, to include all nodes that
        ///  fall within the marquee as potential nodes.
        /// </summary>
        /// <param name="position">Current mouse position.</param>
        public void Update(Vector2 position)
        {
            if (MarqueeActive)
            {
                float x = Mathf.Min(marqueeStart.x, position.x);
                float y = Mathf.Min(marqueeStart.y, position.y);
                float x2 = Mathf.Max(marqueeStart.x, position.x);
                float y2 = Mathf.Max(marqueeStart.y, position.y);

                MarqueeRect = Rect.MinMaxRect(x, y, x2, y2);

                for (int i = 0; i < editor.ViewStates.Count; i++)
                {
                    if (MarqueeRect.Overlaps(editor.ViewStates[i].renderRect))
                    {
                        if (!PotentialSelections.Contains(i) && !AllSelected.Contains(i))
                            PotentialSelections.Add(i);
                    }
                    else
                    {
                        if (PotentialSelections.Contains(i))
                            PotentialSelections.Remove(i);
                    }
                }
            }
        }
        #endregion -- Marquee -------------------------------------------------
    }
}
