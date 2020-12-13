#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AiRuleEngine
{
    enum RelationType
    {
		EQUAL,
		NOTEQUAL,
        LESSTHAN,
		LESSTHANOREQUAL,
        GREATERTHAN,
        GREATERTHANOREQUAL
    }
	
    class RelationalNode : NodeBase
    {
		public RelationType m_RelationType { get; set; }

		public RelationalNode()
		{
			m_RelationType = RelationType.EQUAL;
			m_MaxChildren = 2;
		}

		public RelationalNode(NodeContainer container) : base(container)
		{
			m_RelationType = RelationType.EQUAL;
			m_MaxChildren = 2;
		}

		public override void OnGui()
		{		
			EditorGUILayout.BeginHorizontal();
			m_RelationType = (RelationType)EditorGUILayout.EnumPopup (m_RelationType);
			EditorGUILayout.EndHorizontal();
		}
	
		public override bool CheckChild(NodeBase child)
		{
			bool result = false;
			
			if (CheckForCycles(this, child))
				return false;
			
			if ((child.m_NodeType == NodeType.ARITHMETIC) || (child.m_NodeType == NodeType.RELATIONAL) || 
			    (child.m_NodeType == NodeType.VARIABLE) || (child.m_NodeType == NodeType.CONSTANT))
				result = true;
			
			return result;
			
		}

		public string GetOperator()
		{
			string op;

			switch (m_RelationType) 
			{
			case RelationType.LESSTHAN:
				op = "<";
				break;
			case RelationType.GREATERTHAN:
				op = ">";
				break;
			case RelationType.EQUAL:
				op = "==";
				break;
			case RelationType.NOTEQUAL:
				op = "!=";
				break;
			case RelationType.LESSTHANOREQUAL:
				op = "<=";
				break;
			case RelationType.GREATERTHANOREQUAL:
				op = ">=";
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