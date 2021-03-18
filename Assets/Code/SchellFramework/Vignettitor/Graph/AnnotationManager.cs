//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   10/03/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using SG.Core.OnGUI;
using SG.Vignettitor.Graph.States;
using SG.Vignettitor.VignetteData;
using UnityEngine;

namespace SG.Vignettitor.Graph
{
    /// <summary>
    /// Manages creation, deletion and drawing of annotations on a graph 
    /// editor.
    /// </summary>
    public class AnnotationManager
    {
        #region -- Constants --------------------------------------------------
        /// <summary>
        /// Space to draw above the note text for action buttons.
        /// </summary>
        private const float HEADER_HEIGHT = 20.0f;

        /// <summary> Space left at the bottom of the annotation. </summary>
        private const float FOOTER_HEIGHT = 16.0f;

        /// <summary> Notes can not be smaller than this. </summary>
        public const float MIN_WIDTH = 130.0f;

        /// <summary> Notes can not be smaller than this. </summary>
        public const float MIN_HEIGHT = 100.0f;

        /// <summary> Amount to trim off the header and footer. </summary>
        private readonly RectOffset HEADER_FOOTER_OFFSET = new RectOffset(1, 1, 1, 1);

        /// <summary>
        /// Color for annotations if no configured colors are found.
        /// </summary>
        private readonly Color32 DEFAULT_COLOR = new Color32(57, 168, 223, 60);

        /// <summary>
        /// Icon on the bottom left used to scale an annotation.
        /// </summary>
        private static readonly BuiltInTexture2D scaleIcon = new BuiltInTexture2D("iVBORw0KGgoAAAANSUhEUgAAAAgAAAAICAYAAADED76LAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNvyMY98AAAAzSURBVChTY/j//z9ejFUQhoEAuwQIgyUZGPzwS2JTgCKJrgBDEtkNYA66JIjGK8nA4AcAxDh7BySHKUQAAAAASUVORK5CYII=");
        #endregion -- Constants -----------------------------------------------

        #region -- Private Fields ---------------------------------------------
        /// <summary>Text scroll for the node being edited.</summary>
        private Vector2 scroll;

        /// <summary>Style used to draw the text of the note.</summary>
        private GUIStyle textStyle = null;

        /// <summary>Style for the edit, delete, etc buttons.</summary>
        private GUIStyle buttonStyle;

        /// <summary>Generated texture for the corner scale icon.</summary>
        private Texture2D scaleTex { get { return scaleIcon; } }

        /// <summary>
        /// ID to use for the GUI window when editing an annotation.
        /// </summary>
        private readonly int windowID;

        /// <summary>The state of the editing view.</summary>
        private enum EditState
        {
            Idle,
            PickColor,
            ConfirmDelete
        }        
        private EditState editState = EditState.Idle;

        /// <summary>
        /// Is the annotation in edit currently being scaled?
        /// </summary>
        private bool scaling;
        
        /// <summary> Popup used for binding options. </summary>
        private AnnotationBindPopup bindPopup;
        #endregion -- Private Fields ------------------------------------------

        #region -- Public Properties ------------------------------------------
        /// <summary>Annotation currently being edited.</summary>
        public Annotation AnnotationInEdit { get; private set; }

        /// <summary>Graph editor that this manages annotations for.</summary>
        public GraphEditor Editor { get; private set; }
        #endregion -- Public Properties ---------------------------------------

        #region -- Initialization ---------------------------------------------
        /// <summary> Create a new annotation manager. </summary>
        /// <param name="editor">Graph editor this manages notes for.</param>
        public AnnotationManager(GraphEditor editor)
        {
            this.Editor = editor;
	        windowID = GetType().GetHashCode();
        }
        
        /// <summary>
        /// Initialize GUI styles and textures that must be done during a 
        /// gui call.
        /// </summary>
        private void InitializeGUI()
        {
            textStyle = new GUIStyle(GUI.skin.label);
            textStyle.normal.background = null;
            textStyle.active.background = null;
            textStyle.hover.background = null;
            textStyle.wordWrap = true;

            buttonStyle = new GUIStyle();
            buttonStyle.margin = new RectOffset(2, 2, 0, 0);
            buttonStyle.padding = new RectOffset(4, 4, 0, 0);
        }
        #endregion -- Initialization ------------------------------------------

        #region -- Annotation Control -----------------------------------------
        /// <summary>Enter an edit mode for the given annotation.</summary>
        /// <param name="note">Annotation to edit.</param>
        public void BeginAnnotationEdit(Annotation note)
        {
            AnnotationInEdit = note;
            Editor.EnterEditAnnotationState();
        }

        /// <summary>Exit annotation edit mode.</summary>
        public void EndAnnotationEdit()
        {
            if(AnnotationInEdit.BoundNodes != null)
                 BindAnnotation(AnnotationInEdit, AnnotationInEdit.BoundNodes);
            AnnotationInEdit = null;
            bindPopup = null;
            editState = EditState.Idle;
            if (Editor.state is EditAnnotationState)
                Editor.EnterIdleOrSelectedState();
        }

        /// <summary>
        /// Returns all annotations that have all of their bound nodes present
        /// in the given nodes list.
        /// 
        /// In other words, the bound nodes set of each annotation returned is 
        /// a subset of nodes.
        /// </summary>
        /// <param name="nodes">Nodes to check against.</param>
        /// <returns>
        /// A list of Annotations that are bound to a subset of "nodes" and are
        /// not bound to anything outside of "nodes"
        /// </returns>
        public List<Annotation> GetAnnotationsForBoundNodes(List<VignetteNode> nodes)
        {
            List<Annotation> result = new List<Annotation>(Editor.graphViewState.Annotations.ToArray());
            for (int i = result.Count - 1; i >= 0; i--)
            {
                Annotation note = result[i];

                if (note.BoundNodes == null || note.BoundNodes.Count == 0)
                {
                    result.RemoveAt(i);
                    continue;
                }

                for (int n = 0; n < note.BoundNodes.Count; n++)
                {
                    bool found = false;
                    for (int vn = 0; vn < nodes.Count; vn++)
                    {
                        if (nodes[vn].NodeID == note.BoundNodes[n])
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        result.RemoveAt(i);
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Create a new annotation at the given position.
        /// </summary>
        /// <param name="position">Position to place the annotation.</param>
        /// <returns>The newly created annotation.</returns>
        public Annotation CreateAnnotation(Vector2 position)
        {
            Annotation a = new Annotation();
            a.Position.width = Editor.visuals.DefaultAnnotationSize.x;
            a.Position.height = Editor.visuals.DefaultAnnotationSize.y;
            a.Position.x = position.x - a.Position.width / 2;
            a.Position.y = position.y - a.Position.height / 2;
            Editor.graphViewState.Annotations.Add(a);
            return a;
        }
        
        /// <summary>Delete the given annotation.</summary>
        /// <param name="annotation">Annotation to delete.</param>
        public void DeleteAnnotation(Annotation annotation)
        {
            Editor.graphViewState.Annotations.Remove(annotation);
        }

        /// <summary>
        /// Force cancel any scaling operation of annotations.
        /// </summary>
        public void CancelAnnotationScale()
        { scaling = false; }

        /// <summary>
        /// Tell the annotation to check for rescaling events.
        /// </summary>
        public void OnAnnotationScale()
        {
            if (scaling)
            {
                AnnotationInEdit.Position.width += 
                    Event.current.delta.x / Editor.Zoom;
                AnnotationInEdit.Position.height += 
                    Event.current.delta.y / Editor.Zoom;
            }
        }
        #endregion -- Annotation Control --------------------------------------

        #region -- Node Binding -----------------------------------------------
        /// <summary>
        /// Update all bound annotations to track the nodes they are bound to.
        /// </summary>
        public virtual void Update()
        {
            for (int i = 0; i < Editor.graphViewState.Annotations.Count; i++)
            {
                Annotation note = Editor.graphViewState.Annotations[i];
                List<int> boundIndices = new List<int>();
                for (int b = note.BoundNodes.Count-1; b >=0; b--)
                {
                    int index = Editor.GetIndexByID(note.BoundNodes[b]);
                    if (index != -1)
                        boundIndices.Add(index);
                    else
                    {
                        // If an annotation references an invalid node, like a 
                        // deleted one, remove the reference.
                        note.BoundNodes.RemoveAt(b);
                    }
                }

                if (note != AnnotationInEdit && note.BoundNodes != null && note.BoundNodes.Count > 0)
                {
                    Rect bounds = Editor.GetBounds(boundIndices.ToArray());
                    Vector2 center = bounds.center - note.BoundPositionOffset;  
                    note.Position.center = center;

                    note.Position.width = Mathf.Max(Mathf.Abs(bounds.width - note.BoundSizeOffset.x), MIN_WIDTH);
                    note.Position.height = Mathf.Max(Mathf.Abs(bounds.height - note.BoundSizeOffset.y), MIN_HEIGHT);
                }
            }
        }

        /// <summary>
        /// Bind the annotation open for edit to any nodes that it intersects.
        /// </summary>
        public void BindOpenAnnotationToOverlap()
        {
            Editor.SelectionManager.Clear();
            Editor.SelectionManager.BeginMarqueeSelect(Editor.GraphToScreenPoint(AnnotationInEdit.Position.min), false);
            Editor.SelectionManager.Update(Editor.GraphToScreenPoint(AnnotationInEdit.Position.max));
            Editor.SelectionManager.EndMarqueeSelect(Editor.GraphToScreenPoint(AnnotationInEdit.Position.max));
            BindAnnotation(AnnotationInEdit, Editor.SelectionManager.GetAllSelectedIDs());
        }

        /// <summary>
        /// Bind all selected nodes to the annotation that is open for edit.
        /// </summary>
        public void BindOpenAnnotationToSelection()
        {
            BindAnnotation(AnnotationInEdit, Editor.SelectionManager.GetAllSelectedIDs());
        }

        /// <summary>
        /// Unbind annotation that is open for edit from any selected nodes.
        /// </summary>
        public void ClearOpenAnnotationBind()
        {
            BindAnnotation(AnnotationInEdit, new List<int>());
        }

        /// <summary>
        /// Select all nodes bound to the annotation open for edit.
        /// </summary>
        public void SelectOpenAnnotationBinds()
        {
            List<int> boundNodes = new List<int>(AnnotationInEdit.BoundNodes);
            EndAnnotationEdit();
            Editor.SelectionManager.Clear();
            for (int i = 0; i < boundNodes.Count; i++)
            {
                int index = Editor.GetIndexByID(boundNodes[i]);
                Editor.SelectionManager.AddToSelection(index);
            }
        }

        /// <summary>
        /// Bind the given annotation to the listed nodes.
        /// </summary>
        /// <param name="note">The annotaiton to be bound.</param>
        /// <param name="nodeIDs">The nodes to bind the annotation to.</param>
        private void BindAnnotation(Annotation note, List<int> nodeIDs)
        {
            note.BoundNodes = new List<int>(nodeIDs);

            List<int> boundIndices = new List<int>();
            for (int b = 0; b < note.BoundNodes.Count; b++)
                boundIndices.Add(Editor.GetIndexByID(note.BoundNodes[b]));

            Rect bounds = Editor.GetBounds(boundIndices.ToArray());
            note.BoundSizeOffset = new Vector2(
                bounds.width - note.Position.width,
                bounds.height - note.Position.height);
            note.BoundPositionOffset = bounds.center - note.Position.center;
        }
        #endregion -- Node Binding --------------------------------------------

        #region -- Drawing ----------------------------------------------------
        /// <summary>Draw all of the annotations.</summary>
        public virtual void DrawAnnotations()
        {
            // Initialize GUI styles and assets on first draw.
            if (textStyle == null)
                InitializeGUI();

            if (bindPopup != null)
                bindPopup = bindPopup.Draw() ? null : bindPopup;
   
            // Draw each annotation.
            for (int i = 0; i < Editor.graphViewState.Annotations.Count; i++)
            {
                Annotation note = Editor.graphViewState.Annotations[i];
                //if (editor.state is EditAnnotationState && (editor.state as EditAnnotationState).Note == note)
                if (note == AnnotationInEdit)
                    DrawEditAnnotation(note);
                else
                    DrawIdleAnnotation(note);
            }
        }

        /// <summary>
        /// Draw an annotation that is not being interacted with.
        /// </summary>
        /// <param name="note">Annotation to draw.</param>
        public void DrawIdleAnnotation(Annotation note)
        {
            Rect pos = Editor.GraphToScreenRect(note.Position);
            OnGUIUtils.DrawBox(pos, "", GetAnnotationColor(note.ColorIndex), Color.black);
            GUILayout.BeginArea(pos);
            if (!Editor.ReadOnly)
            {
                if (GUILayout.Button("edit", buttonStyle, GUILayout.ExpandWidth(false)))
                    BeginAnnotationEdit(note);
                Editor.DrawAdapter.AddCursorRect(GUILayoutUtility.GetLastRect(), DrawAdapter.MouseCursor.Link);
            }

            GUILayout.Label(note.Note, textStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.EndArea();
        }

        /// <summary>
        /// Draw an annotation that is currently being edited.
        /// </summary>
        /// <param name="note">Annotation to draw.</param>
        public void DrawEditAnnotation(Annotation note)
        {
            if (note.BoundNodes != null)
            {
                for (int i = 0; i < note.BoundNodes.Count; i++)
                {
                    int nodeIndex = Editor.GetIndexByID(note.BoundNodes[i]);
                    float multiplier = Editor.SelectionManager.IsSelected(nodeIndex) ? 2.0f : 1.0f;
                    int border = (int)(Editor.visuals.SelectionBorderWidth * multiplier * Editor.Zoom);
                    RectOffset offset = new RectOffset(border, border, border, border);

                    OnGUIUtils.DrawBox(offset.Add(Editor.GetNodeRect(nodeIndex)), "",
                        GetAnnotationColor(note.ColorIndex), Color.white);
                }
            }

            Rect pos = Editor.GraphToScreenRect(note.Position);
            pos = GUI.Window(windowID, pos, DrawNote, "");
            note.Position = Editor.ScreenToGraphRect(pos);
            note.Position.width = Mathf.Max(note.Position.width, MIN_WIDTH);
            note.Position.height = Mathf.Max(note.Position.height, MIN_HEIGHT);
        }

        /// <summary>
        /// Get the annotation color at the given index. If no colors are 
        /// found, or the index is not valid, a default is returned.
        /// </summary>
        /// <param name="index">Index of the color to look up.</param>
        /// <returns>Color at the index or a default.</returns>
        public Color32 GetAnnotationColor(int index)
        {
            if (Editor.visuals.AnnotationColors == null || Editor.visuals.AnnotationColors.Length == 0)
                return DEFAULT_COLOR;
            index = Mathf.Clamp(index, 0, Editor.visuals.AnnotationColors.Length);
            return Editor.visuals.AnnotationColors[index];
        }
        #endregion -- Drawing -------------------------------------------------

        #region -- Editing Drawing --------------------------------------------
        /// <summary>Draws the top section of an annotation.</summary>
        /// <param name="pos">Rect for the header.</param>
        private void DrawNoteHeader(Rect pos)
        {
            
            Rect area = pos;
            GUILayout.BeginArea(area);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("done", buttonStyle, GUILayout.ExpandWidth(false)))
                EndAnnotationEdit();
            Editor.DrawAdapter.AddCursorRect(GUILayoutUtility.GetLastRect(), DrawAdapter.MouseCursor.Link);            

            if (editState == EditState.PickColor)
                DrawColorPicker();
            else if (editState == EditState.ConfirmDelete)
                DrawConfirmDelete();
            else if (editState == EditState.Idle)
            {
                //OnGUIUtils.DrawBox(pos, "======", Color.clear, Color.clear);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("delete", buttonStyle))
                    editState = EditState.ConfirmDelete;
                Editor.DrawAdapter.AddCursorRect(GUILayoutUtility.GetLastRect(), DrawAdapter.MouseCursor.Link);
                if (GUILayout.Button("color", buttonStyle))
                    editState = EditState.PickColor;
                Editor.DrawAdapter.AddCursorRect(GUILayoutUtility.GetLastRect(), DrawAdapter.MouseCursor.Link);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.EndArea();

            Editor.DrawAdapter.AddCursorRect(pos, DrawAdapter.MouseCursor.MoveArrow);
        }

        /// <summary>
        /// Draws a color selection in the header.
        /// </summary>
        private void DrawColorPicker()
        {
            GUILayout.FlexibleSpace();
            int selected = -1;
            for (int i = 0; i < Editor.visuals.AnnotationColors.Length; i++)
            {
                if (GUILayout.Button("", buttonStyle, GUILayout.MaxWidth(12.0f)))
                    selected = i;
                OnGUIUtils.DrawBox(GUILayoutUtility.GetLastRect(), "", Editor.visuals.AnnotationColors[i], Color.black);
                Editor.DrawAdapter.AddCursorRect(GUILayoutUtility.GetLastRect(), DrawAdapter.MouseCursor.Link);
            }
            if (selected != -1)
            {
                editState = EditState.Idle;
                AnnotationInEdit.ColorIndex = selected;
            }
        }

        /// <summary>
        /// Draws buttons asking the user if they want to delete.
        /// </summary>
        private void DrawConfirmDelete()
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label("Delete?", buttonStyle);
            if (GUILayout.Button("yes", buttonStyle))
            {
                Editor.AnnotationManager.DeleteAnnotation(AnnotationInEdit);
                editState = EditState.Idle;
                EndAnnotationEdit();
            }
            Editor.DrawAdapter.AddCursorRect(GUILayoutUtility.GetLastRect(), DrawAdapter.MouseCursor.Link);
            if (GUILayout.Button("no", buttonStyle))
                editState = EditState.Idle;
            Editor.DrawAdapter.AddCursorRect(GUILayoutUtility.GetLastRect(), DrawAdapter.MouseCursor.Link);
        }

        /// <summary>
        /// Draw the bottom of an annotation, including the scaling corner.
        /// </summary>
        /// <param name="pos">Rect for teh footer.</param>
        private void DrawNoteFooter(Rect pos)
        {
            Rect bindRect = new Rect( 1, pos.yMax - pos.height, 94, pos.height);
            OnGUIUtils.DrawBox(bindRect, "", new Color(.2f, .2f, .2f, .6f), Color.black);
            if (GUI.Button(bindRect, "node binding...", buttonStyle))
            {
                bindPopup = new AnnotationBindPopup(this, Event.current.mousePosition + Editor.GraphToScreenPoint(AnnotationInEdit.Position.min));
            }
            Editor.DrawAdapter.AddCursorRect(bindRect, DrawAdapter.MouseCursor.Link);

            Rect scaleHit =
                new Rect(pos.width - pos.height + 1,
                    pos.yMax - pos.height,
                    pos.height, pos.height);

            Rect scaleRect =
                new Rect(pos.width - scaleTex.width + 1,
                    pos.yMax - scaleTex.height,
                    scaleTex.width, scaleTex.height);
            GUI.DrawTexture(scaleRect, scaleTex);
            Editor.DrawAdapter.AddCursorRect(scaleRect, DrawAdapter.MouseCursor.ResizeUpLeft);
            if (Event.current.type == EventType.MouseDown && scaleHit.Contains(Event.current.mousePosition))
            {
                scaling = true;
                Vector2 p = new Vector2(AnnotationInEdit.Position.x, AnnotationInEdit.Position.y);
                Editor.GraphToScreenPoint(p + (Event.current.mousePosition / Editor.Zoom));
                Event.current.Use();
            }
        }

        /// <summary>Draw the text body of an annotation.</summary>
        /// <param name="pos">Rect to draw in.</param>
        private void DrawNoteBody(Rect pos)
        {
            GUILayout.BeginArea(pos);
            scroll = GUILayout.BeginScrollView(scroll);
            AnnotationInEdit.Note = Editor.DrawAdapter.TextAreaLayout(AnnotationInEdit.Note, textStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Window call for drawing the annotation in edit.
        /// </summary>
        /// <param name="id">OnGUI Window ID</param>
        private void DrawNote(int id)
        {
            Rect pos = Editor.GraphToScreenRect(AnnotationInEdit.Position);
            pos.x = 0; pos.y = 0;

            Rect headerRect = new Rect(pos);
            headerRect.height = HEADER_HEIGHT;
            headerRect = HEADER_FOOTER_OFFSET.Remove(headerRect);

            Rect footerRect = new Rect(pos);
            footerRect.y = pos.height - FOOTER_HEIGHT;
            footerRect.height = FOOTER_HEIGHT;
            footerRect = HEADER_FOOTER_OFFSET.Remove(footerRect);

            Rect bodyRect = new Rect(pos);
            bodyRect.height -= HEADER_HEIGHT + FOOTER_HEIGHT;
            bodyRect.y += HEADER_HEIGHT;
            bodyRect = HEADER_FOOTER_OFFSET.Remove(bodyRect);

            OnGUIUtils.DrawBox(pos, "", GetAnnotationColor(AnnotationInEdit.ColorIndex), Color.black);

            DrawNoteHeader(headerRect);
            // The header may delete or deselect the annotation.
            if (AnnotationInEdit != null)
            {
                DrawNoteBody(bodyRect);
                DrawNoteFooter(footerRect);
            }

            if (Event.current.type == EventType.MouseUp)
                scaling = false;
            GUI.DragWindow(headerRect);
        }
        #endregion -- Editing Drawing -----------------------------------------
    }
}