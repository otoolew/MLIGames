using UnityEditor;
using AiRuleEngine;

namespace AiRuleEngineEditor
{
	[CustomEditor(typeof(State))]
	public class StateEditor : Editor
	{
		override public void OnInspectorGUI()
		{
			
			(target as State).ShowGUI();
			//EditorUtils.EndOfInspector();
			Repaint();
		}
		
		public void OnEnable()
		{
		
		}
	}
}