//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using SG.Core.OnGUI;
using SG.Vignettitor.Graph.Drawing;
using SG.Vignettitor.Graph.States;
using UnityEngine;

namespace SG.Vignettitor.VignettitorCore.States
{
    public class NewNodeState : GraphEditorState
    {
        protected readonly Vector2 NEW_RECT = new Vector2(450, 600);

        protected int parent;
        protected Vector2 position;
        protected int outputIndex;

        protected GUIStyle buttonStyle;

        protected VignetteNodeHelpWindow nodeHelpWindow;

        protected Vector2 scroll;
        protected NodeMenu nodeMenu;
        protected string filterText = string.Empty;

        protected List<int> currentMenuSelections = new List<int>() { 0 };

        protected virtual string NewNodeMenuSelectionsKey
        { get { return "NewNodeMenuSelections"; } }

        public NewNodeState(Vignettitor renderer, int parent, int outputIndex, Vector2 position)
            : base(renderer)
        {
            string savedSelections = PlayerPrefs.GetString(NewNodeMenuSelectionsKey, string.Empty);
            if (!string.IsNullOrEmpty(savedSelections))
                currentMenuSelections = savedSelections.Split('*').Select(x => int.Parse(x)).ToList();
            CreateNodeMenu();

            this.parent = parent;
            this.outputIndex = outputIndex;
            this.position = position;
        }

        protected virtual void CreateNodeMenu()
        {
            Vignettitor v = editor as Vignettitor;
            nodeMenu = new NodeMenu(v.head.GetType(), v.InheritNodes);
        }

        public override void Draw()
        {
            base.Draw();
            Rect r = editor.GetFullyOnScreenRect(
                new Rect(position.x, position.y, NEW_RECT.x, NEW_RECT.y));
            GUI.Window(2929, r, NewNodeWindow, "Select new node");
            GUI.BringWindowToFront(2929);

            if (nodeHelpWindow != null)
                nodeHelpWindow = nodeHelpWindow.Draw() ? null : nodeHelpWindow;
        }

        public override void OverlayDraw()
        {
            GUI.BringWindowToFront(2929);
            base.OverlayDraw();
            OnGUIUtils.DrawLine(editor.GetNodeExit(parent, outputIndex), position, editor.visuals.NewConnectionColor);
        }

        private void DrawNodeEntry(NodeMenu.NodeMenuEntry entry, bool addToRecentsOnSelect, Vector2 helpBoxOffset)
        {
            GUILayout.BeginHorizontal();

            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.alignment = TextAnchor.MiddleLeft;
                buttonStyle.contentOffset = new Vector2(20, 0);
            }
            
            if (GUILayout.Button(entry.nodeName, buttonStyle))
            {
                editor.CreateNewNode(parent, outputIndex, position, entry.nodeType);
                editor.state = new IdleState(editor);
                if (addToRecentsOnSelect)
                    nodeMenu.Recents.SetRecentlyUsed(entry);
            }

            if (entry.nodeIcon != null)
            {
                Rect lastR = GUILayoutUtility.GetLastRect();
                lastR.x += 5;
                lastR.y += 3;
                lastR.height -= 4;
                float ratio = entry.nodeIcon.width / (float)entry.nodeIcon.height;
                lastR.width = lastR.height*ratio;
                GUI.DrawTexture(lastR, entry.nodeIcon);
            }

            if (GUILayout.Button("?", GUILayout.ExpandWidth(false)))
            {
                Vector2 pos = Event.current.mousePosition;

                Vector2 helpPosition = editor.GetFullyOnScreenRect(new Rect(position.x, position.y, NEW_RECT.x, NEW_RECT.y)).position;
                helpPosition.x += pos.x - VignetteNodeHelpWindow.DEFAULT_WIDTH;
                helpPosition.y += pos.y + helpBoxOffset.y - scroll.y;

                nodeHelpWindow = new VignetteNodeHelpWindow(entry.nodeName, entry.nodeType, helpPosition);

            }
            GUILayout.EndHorizontal();
        }

        private void NewNodeWindow(int windowID)
        {
            if (nodeHelpWindow == null)
                GUI.FocusControl("SearchBox");
            if (GUILayout.Button("Cancel"))
            {
                editor.state = new IdleState(editor);
            }
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUI.SetNextControlName("SearchBox");
            GUILayout.Label("Search:", GUILayout.ExpandWidth(false));
            filterText = GUILayout.TextField(filterText, GUILayout.Height(25.0f));
            if (GUILayout.Button("x", GUILayout.Width(20)))
                filterText = string.Empty;
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            if (nodeMenu.Recents.Count > 0)
            {
                for (int i = 0; i < nodeMenu.Recents.Count; i++)
                {
                    NodeMenu.NodeMenuEntry entry = nodeMenu.Recents[i];
                    if (string.IsNullOrEmpty(filterText) || entry.nodeName.ToLower().Contains(filterText.ToLower()))
                    {
                        DrawNodeEntry(entry, false, Vector2.zero);
                    }
                }
                GUILayout.Space(10);
            }

            NodeMenu.SubMenu currentDepthPoint = nodeMenu.Root;
            int currentDepth = 0;
            while (currentDepth < 5)
            {
                if (currentDepthPoint.subMenus.Count > 0)
                {
                    if (currentDepth >= currentMenuSelections.Count)
                        currentMenuSelections.Add(0);

                    int newMenuSelection = GUILayout.Toolbar(currentMenuSelections[currentDepth], currentDepthPoint.GetMenusForToolbar());
                    if (newMenuSelection != currentMenuSelections[currentDepth])
                    {
                        // invalidate selections at further depths
                        currentMenuSelections = currentMenuSelections.GetRange(0, currentDepth + 1);
                        currentMenuSelections[currentDepth] = newMenuSelection;

                        PlayerPrefs.SetString(NewNodeMenuSelectionsKey, currentMenuSelections.Select(x => x.ToString()).Aggregate((a, b) => a + '*' + b));
                    }

                    if (newMenuSelection > 0)
                    {
                        currentDepthPoint = currentDepthPoint.subMenus[newMenuSelection - 1];
                        currentDepth++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            Rect scrollRect = GUILayoutUtility.GetLastRect();
            scroll = GUILayout.BeginScrollView(scroll);

            bool showedNode = false;
            foreach(NodeMenu.NodeMenuEntry entry in currentDepthPoint.GetDistinctEntries())
            {
                if (string.IsNullOrEmpty(filterText) || entry.nodeName.ToLower().Contains(filterText.ToLower()))
                {
                    showedNode = true;
                    DrawNodeEntry(entry, true, scrollRect.max); 
                }
            }

            // there are no currently shown nodes with this substring therefore assume the user
            //   actually wants to do a broad search and clear the toolbar selections
            if (filterText.Length > 0 && !showedNode)
                currentMenuSelections.Clear();

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }
    }
}
