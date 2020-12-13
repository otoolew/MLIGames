#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AiRuleEngine
{
    enum OperationType
    {
        ADD,
        SUBTRACT,
        DIVIDE,
        MULTIPLY,
        POWER,
        LOG
    }
	
    class ArithmeticNode : NodeBase
    {
		public OperationType m_OperationType { get; set; }

		public ArithmeticNode()
		{
			m_NodeType = NodeType.ARITHMETIC;
			m_MaxChildren = 2;
		}

		public ArithmeticNode(NodeContainer container) : base(container)
		{
			m_NodeType = NodeType.ARITHMETIC;
			m_MaxChildren = 2;
		}

		public override void OnGui()
		{
			EditorGUILayout.BeginHorizontal();
			m_OperationType = (OperationType)EditorGUILayout.EnumPopup(m_OperationType);
			EditorGUILayout.EndHorizontal();
		}

		public override bool CheckChild(NodeBase child)
		{
			bool result = false;

			if (CheckForCycles(this, child))
				return false;

			if (    (child.m_NodeType == NodeType.ARITHMETIC) || 
			        (child.m_NodeType == NodeType.VARIABLE) || 
                    (child.m_NodeType == NodeType.CONSTANT) )
				result = true;

			return result;
		}
		
		public string GetOperator()
		{
			string op = "none";

			switch (m_OperationType) 
			{
				case OperationType.ADD:
					op = "+";
					break;
				case OperationType.SUBTRACT:
					op = "-";
					break;
				case OperationType.DIVIDE:
					op = "/";
					break;
				case OperationType.MULTIPLY:
					op = "*";
					break;
				case OperationType.POWER:
					op = "^";
					break;
				case OperationType.LOG:
					op = "Log";
					break;
			}

			return op;
		}
    }
}
#endif
