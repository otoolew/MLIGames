using UnityEditor;
using AiRuleEngine;

namespace AiRuleEngineEditor
{
	[CustomEditor(typeof(RuleBase))]
	public class RuleBaseEditor : Editor
	{
		override public void OnInspectorGUI()
		{
			
			(target as RuleBase).ShowGUI();
			//EditorUtils.EndOfInspector();
			Repaint();
		}
		
		public void OnEnable()
		{
		
		}
	}
}