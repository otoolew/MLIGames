#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace AiRuleEngine
{
    class ConstantNode : NodeBase
    {
		public VariableType m_ConstantType { get; set; }
		public object m_ConstantValue { get; set; }

		public ConstantNode()
		{
			m_NodeType = NodeType.CONSTANT;
			m_ConstantType = VariableType.FLOAT;
			m_ConstantValue = new float();
			m_MaxChildren = 0;
		}

		public ConstantNode(NodeContainer container) : base(container)
		{
			m_NodeType = NodeType.CONSTANT;
			m_ConstantType = VariableType.FLOAT;
			m_ConstantValue = new float();
			m_MaxChildren = 0;
		}

		public override void OnGui()
		{
			EditorGUILayout.BeginVertical();
			m_ConstantType = (VariableType)EditorGUILayout.EnumPopup(m_ConstantType);

			switch (m_ConstantType) 
			{
				case VariableType.BOOLEAN:
					if (m_ConstantValue.GetType() != typeof(bool))
						m_ConstantValue = new bool();
					m_ConstantValue = EditorGUILayout.Toggle((bool)m_ConstantValue);
					break;
	
				case VariableType.FLOAT:
					if (m_ConstantValue.GetType() != typeof(float))
						m_ConstantValue = new float();	
					m_ConstantValue = EditorGUILayout.FloatField((float)m_ConstantValue);
					break;

				case VariableType.INT:
					if (m_ConstantValue.GetType() != typeof(int))
				    	m_ConstantValue = new int();	
					m_ConstantValue = EditorGUILayout.IntField((int)m_ConstantValue);
					break;

				case VariableType.STRING:
					if (m_ConstantValue.GetType() != typeof(string))
						m_ConstantValue = "";
					m_ConstantValue = EditorGUILayout.TextField((string)m_ConstantValue);
					break;
			}

			EditorGUILayout.EndVertical();
		}
	
		public override bool CheckChild(NodeBase child)
		{
			bool result = false;
			
			return result;	
		}
    }
}
#endif