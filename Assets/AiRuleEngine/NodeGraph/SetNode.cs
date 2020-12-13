#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AiRuleEngine
{
	class SetNode : NodeBase
	{
		public string m_VariableName { get; set; }
		
		public SetNode(NodeContainer container) : base(container)
		{
			m_MaxChildren = 1;
			m_NodeType = NodeType.SETVARIABLE;
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
			
			if (CheckForCycles(this, child))
				return false;
			
			if ((child.m_NodeType == NodeType.ARITHMETIC) || 
			    (child.m_NodeType == NodeType.VARIABLE) || (child.m_NodeType == NodeType.CONSTANT))
				result = true;
			
			return result;
		}
	}
}
#endif