//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   05/08/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace SG.Core.OnGUI
{
    /// <summary>
    /// Displays an OnGUI control that has a nested list of items where each 
    /// one may be collapsed.
    /// </summary>
    public class HierarchyView
    {
        /// <summary>
        /// A display node in the hierarchy. This will draw with it's name and 
        /// a button to expand and collapse it in the hierarchy.
        /// </summary>
        public class Node
        {
            public string DisplayName = "";

            /// <summary>
            /// A node can hold some user data.
            /// </summary>
            public object objData;

            // TODO: set this when changing parent.
            public string FullPath = "";

            public List<Node> Children = new List<Node>();

            public bool Expanded;
            
            public bool Selected;

            public System.Action<Node> OnSelect;

            public Node(string name, object obj = null)
            {
                DisplayName = name;
                objData = obj;
            }

            public Node(string name, System.Action<Node> onSelect, object obj = null)
            {
                DisplayName = name;
                OnSelect = onSelect;
                objData = obj;
            }

            public void AddChild(Node n)
            {
                if (!Children.Contains(n))
                    Children.Add(n);
            }

            public void Clear()
            {
                Children.Clear();
            }

            public int GetTotalCount()
            {
                int nTotal = 0;
                foreach (Node n in Children)
                {
                    nTotal += n.GetTotalCount();   
                }

                return Children.Count + nTotal;
            }
        }

        /// <summary>Height to draw each node in.</summary>
        private const float NODE_HEIGHT = 20.0f;

        /// <summary>Amount to indent each node from its parent.</summary>
        private const float INDENT = 30.0f;

        /// <summary>Empty space on left and right.</summary>
        private const float EDGE_SPACE = 30.0f;

        /// <summary>
        /// Create a new Hierachy View for drawing vertical nested trees.
        /// </summary>
        public HierarchyView() : this("root")
        {
        }

        public HierarchyView(string rootName)
        {
            _root = new Node(rootName);
            _root.Expanded = true;
        }

        /// <summary>Add a node to the root of the hierarchy.</summary>
        /// <param name="n">Node to add.</param>
        public void AddNode(Node n)
        {
            _root.AddChild(n); 
        }

        /// <summary>Clears the root.</summary>
        public void Clear()
        {
            _root.Clear(); 
        }

        /// <summary>Does the root have any nodes in it.</summary>
        public bool HasChildren()
        {
            return _root.Children.Count > 0;
        }

        public int ChildCount()
        {
            return _root.GetTotalCount();
        }

        public float GetDrawHeight()
        {
            return GetNodeHeight(_root);
        }

        private float GetNodeHeight(Node n)
        {
            if (!n.Expanded || n.Children.Count == 0)
                return NODE_HEIGHT;
            
            float result = NODE_HEIGHT;
            for (int i = 0; i < n.Children.Count; i++)
            {
                result += GetNodeHeight(n.Children[i]);
            }
            return result;         
        }

        /// <summary>Draw the tree view inside the given Rect.</summary>
        /// <param name="r">Container for the view.</param>
        /// <param name="bDrawLines">Should the view draw the lines for nodes? This should be set to false if the hierarchy view is part of a scroll view.</param>
        public void Draw(Rect r, bool bDrawLines)
        {
            _container = r;
            _nextDrawY = r.y;
            if (bDrawLines)
            {
                DrawNode(_root, EDGE_SPACE);
            }
            else
            {
                DrawNodeNoLines(_root, EDGE_SPACE);
            }
        }

        /// <summary>
        /// Draw an individual node in tha hierarchy and all of its descendents
        /// until an unexpanded node is encountered. As well as draw lines connecting the 
        /// hierarchy.
        /// </summary>
        /// <param name="n">The node do draw.</param>
        /// <param name="depth">
        /// The draw depth in the hierarchy. This is a pixel amount to tab in.
        /// </param>
        private void DrawNode(Node n, float depth)
        {
            //Rect l = new Rect(depth + 200.0f, nextDrawY, 100, 20);
            //GUI.Label(l, GetNodeHeight(n).ToString());
            Rect button = DrawNodeButton(n, depth);
            if (n.Expanded)
            {
                for (int i = 0; i < n.Children.Count; i++)
                {
                    float centerY = _nextDrawY + Mathf.Floor(NODE_HEIGHT / 2.0f);
                    Vector2 hStart = new Vector2(button.center.x, centerY);
                    Vector2 hEnd = new Vector2(button.center.x + INDENT, centerY);
                    OnGUIUtils.DrawLine(hStart, hEnd, Color.gray);
                    DrawNode(n.Children[i], depth + INDENT);
                }

                Vector2 vStart = button.center;
                vStart.y += NODE_HEIGHT / 2.0f;
                Vector2 vEnd = new Vector2(vStart.x, _nextDrawY);
                vEnd.y -= Mathf.Ceil(NODE_HEIGHT / 2.0f);
                OnGUIUtils.DrawLine(vStart, vEnd, Color.gray);
            }
        }

        public List<Node> selectedNodes = new List<Node>();

        /// <summary>
        /// This method will draw the node as a button when it is a leaf node and has a callback.
        /// Otherwise, it will draw it as a label.
        /// </summary>
        /// <param name="n">The node do draw.</param>
        /// <param name="depth">
        /// The draw depth in the hierarchy. This is a pixel amount to tab in.
        /// </param>
        private Rect DrawNodeButton(Node n, float depth)
        {
            Rect r = new Rect(depth, _nextDrawY, _container.width - depth - EDGE_SPACE, NODE_HEIGHT);
            Rect button = new Rect(r);
            button.x += 5;
            button.y += 1;
            button.height -= 2;
            button.width = button.height + 3;

            Rect label = new Rect(r);
            label.x = button.xMax + 5.0f;
            label.width -= label.x;

            GUIStyle style = GUI.skin.button;
            if (n.Children.Count > 0)
            {
                style = GUI.skin.label;
                string icon = n.Expanded ? "-" : "+";
                if (GUI.Button(button, icon))
                    n.Expanded = !n.Expanded;
            }

            if (n.OnSelect == null)
                style = GUI.skin.label;

            if (GUI.Button(label, n.DisplayName, style))
            {
                if (n.OnSelect != null)
                    n.OnSelect(n);
            }
            _nextDrawY += NODE_HEIGHT;

            return button;
        }

        /// <summary>
        /// Draw an individual node in tha hierarchy and all of its descendents
        /// until an unexpanded node is encountered. This will not draw any lines.
        /// </summary>
        /// <param name="n">The node do draw.</param>
        /// <param name="depth">
        /// The draw depth in the hierarchy. This is a pixel amount to tab in.
        /// </param>
        private void DrawNodeNoLines(Node n, float depth)
        {
            DrawNodeButton(n, depth);
            if (n.Expanded)
            {
                for (int i = 0; i < n.Children.Count; i++)
                {
                    DrawNodeNoLines(n.Children[i], depth + INDENT);
                }
            }
        }

        /// <summary>Outer bounds for the controls to draw in.</summary>
        private Rect _container;

        /// <summary>The vertical position to the next node.</summary>
        private float _nextDrawY = 10.0f;

        /// <summary>
        /// The root of the hierarcy. This is not explicitly drawn, but it's 
        /// children are. All nodes in the hierarchy must be under this.
        /// </summary>
        private readonly Node _root;
    }
}
