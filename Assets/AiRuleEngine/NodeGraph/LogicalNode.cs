#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace AiRuleEngine
{
    enum LogicalOperatorType
    {
        AND,
        OR,
        NOT
    }
	
    class LogicalNode : NodeBase
    {
		public LogicalOperatorType m_LogicalType;

		public LogicalNode()
		{
			m_NodeType = NodeType.LOGICAL;
			m_MaxChildren = 2;
		}

		public LogicalNode(NodeContainer container) : base(container)
		{
			m_NodeType = NodeType.LOGICAL;
			m_MaxChildren = 2;
		}

		public override void OnGui()
		{
			EditorGUILayout.BeginHorizontal();
			m_LogicalType = (LogicalOperatorType)EditorGUILayout.EnumPopup(m_LogicalType);
			EditorGUILayout.EndHorizontal();

			if (m_LogicalType == LogicalOperatorType.NOT)
				m_MaxChildren = 1;
			else
				m_MaxChildren = 2;
		}

		public override bool CheckChild(NodeBase child)
		{
			bool result = false;

			if (CheckForCycles(this, child))
				return false;

			if ((child.m_NodeType == NodeType.LOGICAL) || (child.m_NodeType == NodeType.RELATIONAL) || 
			   (child.m_NodeType == NodeType.VARIABLE) || (child.m_NodeType == NodeType.CONSTANT))
				result = true;

			return result;
			
		}

		public string GetOperator()
		{
			string op = "none";

			switch (m_LogicalType) 
			{
			case LogicalOperatorType.AND:
					op = "And";
					break;
			case LogicalOperatorType.OR:
					op = "Or";
					break;
			case LogicalOperatorType.NOT:
					op = "Not";
					break;
				default:
					op = "none";
					break;
			}

			return op;
		}
    }
}
#endif