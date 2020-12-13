using UnityEngine;
using UnityEditor;
using AiRuleEngine;

namespace AiRuleEngineEditor
{
	[CustomEditor(typeof(InferenceEngine))]
	public class InferenceEngineEditor : Editor
	{
		override public void OnInspectorGUI()
		{
			(target as InferenceEngine).ShowGUI();

			Repaint();
		}
		
		public void OnEnable()
		{
		
		}
	}
}