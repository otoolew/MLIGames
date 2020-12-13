#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace AiRuleEngine
{
	public class CallNode : NodeBase
	{
		public BaseAction m_Script;
	
		public CallNode(NodeContainer container) : base(container)
		{
			m_MaxChildren = 0;
			m_NodeType = NodeType.CALLSCRIPT;
            m_Script = null;
		}

		public override void OnDestroy()
		{
			if (m_Script != null)
				UnityEngine.Object.DestroyImmediate(m_Script);
		}

		public override void OnGui()
		{
			EditorGUILayout.LabelField("Scripts", EditorStyles.boldLabel);
			List<ScriptDescriptor> scriptList = ScriptManager.GetScriptsByType(typeof(BaseAction));

            if (scriptList.Count > 0)
			{
				int selection = -1;
               
                if (m_Script != null)
				{
                    selection = scriptList.FindIndex(scriptDescriptor => scriptDescriptor.m_Type == m_Script.GetType());
					selection = Math.Max(0, selection);
				}

                string[] scriptNames = new string[scriptList.Count];
                
				for (int i = 0; i < scriptList.Count; i++)
                    scriptNames[i] = scriptList[i].m_Name;
					
				EditorGUILayout.BeginHorizontal();

				int newSelection = EditorGUILayout.Popup(selection, scriptNames);
                if (newSelection != selection)
                {
					if (m_Script != null)
						UnityEngine.Object.DestroyImmediate(m_Script);

                    ScriptDescriptor scriptDescriptor = scriptList[newSelection];
                    m_Script = (BaseAction)m_Container.m_Editor.m_TargetObject.AddComponent(scriptDescriptor.m_Type);
                    selection = newSelection;
                }

				EditorGUILayout.EndHorizontal();

                if (m_Script)
                    m_Script.ShowScriptGUI();
			}
		}

		public override bool CheckChild(NodeBase child)
		{
			bool result = false;
			
			if (m_Children.Contains (child))
				result = false;
			
			return result;
		}
	}
}
#endif