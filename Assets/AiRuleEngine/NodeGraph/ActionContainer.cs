#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace AiRuleEngine
{
	public class ActionContainer : NodeContainer
	{
		public List<NodeBase> m_ActionList { get; private set; }

		Rect canvas = new Rect(0, 0, 2000, 2000);
		Vector2 scrollPosition = Vector2.zero;
		float maxXPosition = 0;
		float maxYPosition = 0;

		public ActionContainer ()
		{
			m_ActionList = new List<NodeBase>();
			minSize = new Vector2(800.0f, 500.0f);
		}

		public void Load()
		{
			RuleToGraph converter = new RuleToGraph(this);
			m_ActionList = converter.Convert(m_Editor.m_Rule);
		}

		void OnDestroy()
		{
			foreach (NodeBase action in m_ActionList)
			{
				action.OnDestroy();
			}
		}

		public override void OnGUI()
		{
			base.OnGUI();

			GUILayout.Label("Action Inspector", EditorStyles.boldLabel);

			if (m_NodesToAttach.Count == 2) 
			{
				if(m_NodesToAttach[0] != m_NodesToAttach[1])
				{
					m_ActionList[m_NodesToAttach[0]].AddChild(m_ActionList[m_NodesToAttach[1]]);
					m_NodeEdges.Add(new NodeEdge(m_ActionList[m_NodesToAttach[0]], m_ActionList[m_NodesToAttach[1]]));
										
					m_NodesToAttach.Clear();
				}
			}

			Event e = Event.current;
			//Canvas Scroll pan
			if (e.button == 0 && e.isMouse && e.type == EventType.MouseDrag && e.alt)
				scrollPosition += e.delta * 2;
			
			//find the max windows position
			for (int i = 0; i < m_ActionList.Count; i++)
			{
				maxXPosition = Mathf.Max (m_ActionList[i].m_Window.xMax, maxXPosition);
				maxYPosition = Mathf.Max (m_ActionList[i].m_Window.yMax, maxYPosition);
			}
			
			//Get and set canvas limits for the nodes
			Vector2 canvasLimits = new Vector2 (Mathf.Max(500, maxXPosition), Mathf.Max(100, maxYPosition));
			canvas.width = canvasLimits.x;
			canvas.height = canvasLimits.y;
			
			Rect actualCanvas = new Rect(5, 0, position.width - 10, position.height);
			
			scrollPosition = GUI.BeginScrollView(actualCanvas, scrollPosition, canvas);

			BeginWindows ();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Add Node: ",GUILayout.ExpandWidth(false));

			if (GUILayout.Button ("Call Script", GUILayout.ExpandWidth(false))) 
			{
				m_ActionList.Add(new CallNode(this));
			}
			
			if (GUILayout.Button ("Set Variable", GUILayout.ExpandWidth(false))) 
			{
				m_ActionList.Add(new SetNode(this));
			}
			
			if (GUILayout.Button ("Arithmetic", GUILayout.ExpandWidth(false))) 
			{
				m_ActionList.Add(new ArithmeticNode(this));
			}

			if (GUILayout.Button ("Constant", GUILayout.ExpandWidth(false))) 
			{
				m_ActionList.Add(new ConstantNode(this));
			}

			if (GUILayout.Button ("Variable", GUILayout.ExpandWidth(false))) 
			{
				m_ActionList.Add (new VariableNode (this));
			}

			if (m_NodesToAttach.Count == 1) 
			{
				if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(false)))
				{
					m_NodesToAttach.Clear();
				}
			}
			
			GUILayout.EndHorizontal();

			foreach (NodeBase action in m_ActionList)
			{
				GUILayoutOption[] options = { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.MinHeight(100f), GUILayout.MinWidth(100f) };
				int index = m_ActionList.IndexOf(action);
				action.m_Window = GUILayout.Window(index, action.m_Window, DrawAction, action.m_NodeType.ToString(), options);
			}

			foreach (NodeEdge edge in m_NodeEdges)
			{
				edge.OnDraw();
			}

			foreach (NodeEdge deletedEdge in m_RemoveEdgeList)
			{
				m_NodeEdges.Remove(deletedEdge);
			}

			foreach (NodeBase deletedNode in m_RemoveNodeList)
			{
				m_ActionList.Remove(deletedNode);
				deletedNode.OnDestroy();
			}
			
			m_RemoveEdgeList.Clear();
			m_RemoveNodeList.Clear();
			
			EndWindows();

			GUI.EndScrollView();
		}
		
		void DrawAction(int index)
		{
			m_ActionList[index].OnGui();

			if (m_ActionList[index].m_Children.Count < m_ActionList[index].m_MaxChildren && (m_NodesToAttach.Count != 1) && 
			    m_ActionList[index].m_NodeType != NodeType.ARITHMETIC && m_ActionList[index].m_NodeType != NodeType.CONSTANT &&
			    m_ActionList[index].m_NodeType != NodeType.VARIABLE)
				if (GUILayout.Button("Make Parent")) 
				{
					m_NodesToAttach.Add(index);
				}

			if ((m_NodesToAttach.Count == 1) && (index != m_NodesToAttach[0]) && (m_ActionList[m_NodesToAttach[0]].CheckChild(m_ActionList[index])))
				if (GUILayout.Button("Make Child")) 
				{
					m_NodesToAttach.Add(index);
				}

			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			Color currentColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.red;
			GUILayoutOption[] options = { GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(15f), GUILayout.Width(15f) };

			if (GUILayout.Button(" ", options))
			{
				foreach (NodeBase child in m_ActionList[index].m_Children)
					m_ActionList[index].RemoveChild(child);
				
				RemoveNode(m_ActionList[index]);
			}
			
			GUI.backgroundColor = currentColor;
			GUILayout.EndHorizontal();

			GUI.DragWindow();
		}
	}
}
#endif

