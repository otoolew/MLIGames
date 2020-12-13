#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AiRuleEngine;
using UnityEngine;
using UnityEditor;

namespace AiRuleEngine
{
    class VariableNode : NodeBase
    {
		public VariableType m_VariableType { get; set; }
        public string m_VariableName { get; set; }

		public VariableNode()
		{
			m_NodeType = NodeType.VARIABLE;
			m_VariableName = "";
			m_VariableType = VariableType.FLOAT;
			m_MaxChildren = 0;
		}

		public VariableNode(NodeContainer container) : base(container)
		{
			m_NodeType = NodeType.VARIABLE;
			m_VariableName = "";
			m_VariableType = VariableType.FLOAT;
			m_MaxChildren = 0;
		}

		public override void OnGui()
		{
			State state = m_Container.m_Editor.m_TargetObject.GetComponent(typeof(State)) as State;

			if (state != null) 
			{
				List<string> variableNames;
				state.GetVariables(out variableNames);

				if (variableNames.Count > 0)
				{
					int selection = variableNames.FindIndex(x => x == m_VariableName);
					selection = Math.Max(0, selection);

					EditorGUILayout.BeginHorizontal ();
					selection = EditorGUILayout.Popup(selection, variableNames.ToArray());
					m_VariableName = variableNames[selection];
					Variable variable;
					state.GetVariable(m_VariableName, out variable);
					m_VariableType  = variable.GetVariableType();
					EditorGUILayout.EndHorizontal();
				}
				else
				{
					EditorGUILayout.HelpBox("Please Add Variables", MessageType.Info);
				}
			}
		}

		public override bool CheckChild(NodeBase child)
		{
			bool result = false;
			
			return result;
		}
    }
}
#endif
