#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace AiRuleEngine
{
	public class NodeEdge
	{
		public NodeBase m_FromNode { get; set; }
		public NodeBase m_ToNode { get; set; }
		public Rect m_StartConnector;
		public Rect m_EndConnector;
		public Vector3 m_EdgePosStart = Vector3.zero;
		public Vector3 m_EdgePosEnd = Vector3.zero;
		private int m_ConnectWidth;

		public NodeEdge (NodeBase from, NodeBase to)
		{
			m_FromNode = from;
			m_ToNode = to;
			m_ConnectWidth = m_FromNode.m_Children.IndexOf(m_ToNode);
		}
		
		public void OnDraw()
		{
			m_StartConnector = new Rect (0, 0, 20, 20);
			m_StartConnector.center = m_EdgePosStart;
			GUI.Box (m_StartConnector, "");

			Event e = Event.current;

			if((e.type == EventType.MouseDown & e.button == 0) && (m_StartConnector.Contains(e.mousePosition)))
			{
				RemoveEdgeFunction();
			}

			if (m_FromNode != null && m_ToNode != null)
				DrawNodeCurve(m_FromNode.m_Window, m_ToNode.m_Window);
		}

		private void DrawNodeCurve(Rect start, Rect end)
		{
			Vector3 startPos = new Vector3 (start.x + ((m_ConnectWidth + 1) * start.width / Math.Max(3,m_ConnectWidth + 2)), start.y + start.height, 0);
			Vector3 endPos = new Vector3 (end.x + end.width / 2, end.y, 0);
			Vector3 startTan = startPos + Vector3.up * 50;
			Vector3 endTan = endPos + Vector3.down * 50;
			Color shadowColor = new Color (0, 0, 0, 0.06f);
			m_EdgePosStart = startPos;
			m_EdgePosEnd = new Vector3 (end.x, end.y + end.height / 2, 0);
			
			for (int i = 0; i < 3; i++)
				Handles.DrawBezier (startPos, endPos, startTan, endTan, shadowColor, null, (i + 1) * 5);
			
			Handles.DrawBezier (startPos, endPos, startTan, endTan, Color.red, null, 3);
		}

		public bool CheckForEdge(NodeBase node)
		{
			bool result = false;
			if(m_FromNode == node || m_ToNode == node)
			{
				RemoveEdgeFunction();
				result = true;
			}
			return result;
		}

		void RemoveEdgeFunction()
		{
			m_FromNode.RemoveChild(m_ToNode);
			m_FromNode.m_Container.RemoveEdge(this);
			m_ConnectWidth = m_FromNode.m_Children.Count;
		}
	}
}
#endif