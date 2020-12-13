#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace AiRuleEngine
{
	public abstract class NodeContainer : EditorWindow
	{
		public GameObject m_TargetObject { get; private set; }

		public RuleInspector m_Editor { get; set; }
		public List<NodeEdge> m_RemoveEdgeList = new List<NodeEdge>();
		public List<NodeBase> m_RemoveNodeList = new List<NodeBase>();
		public List<int> m_NodesToAttach = new List<int>();
		public List<NodeEdge> m_NodeEdges = new List<NodeEdge>();
		public NodeType m_SelectedNodeType;
		
		public NodeContainer ()
		{
		}

		public virtual void OnGUI()
		{
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button ("Save", GUILayout.ExpandWidth(false))) 
			{
				m_Editor.SaveRule();
			}

			if (GUILayout.Button ("Cancel", GUILayout.ExpandWidth(false))) 
			{
				m_Editor.CloseEditors();
			}

			EditorGUILayout.EndHorizontal();
		}
		
		public void RemoveEdge(NodeEdge nodeEdge)
		{
			m_RemoveEdgeList.Add(nodeEdge);
		}

		public void RemoveNode(NodeBase node)
		{
			foreach(NodeEdge edgeToRemove in m_NodeEdges)
			{
				if (edgeToRemove.CheckForEdge(node))
					break;
			}

			m_RemoveNodeList.Add(node);
		}

		public void DrawEdge(int index)
		{
			m_NodeEdges[index].OnDraw();
		}
	}
}
#endif
