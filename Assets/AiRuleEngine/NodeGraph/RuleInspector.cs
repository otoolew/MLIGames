#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XMLRules;
using AiRuleEngine;
using UnityEngine;
using UnityEditor;

namespace AiRuleEngine
{
	public class RuleInspector
	{
		public ActionContainer m_ActionContainer  { get; private set; }
		public ConditionContainer m_ConditionContainer  { get; private set; }
		public Rule m_Rule { get; private set; }
		public GameObject m_TargetObject { get; private set; }

		public RuleInspector(Rule rule, GameObject target)
		{
			m_Rule = rule;
			m_TargetObject = target;
		}

		public void ShowEditor() 
		{
			m_ConditionContainer = EditorWindow.GetWindow<ConditionContainer>("Condition", true, typeof(ActionContainer));
			m_ConditionContainer.m_Editor = this;
			m_ConditionContainer.Load();

			m_ActionContainer = EditorWindow.GetWindow<ActionContainer>("Action", true, typeof(ConditionContainer));
			m_ActionContainer.m_Editor = this;
			m_ActionContainer.Load();
			
			m_ConditionContainer.Focus();
		}

		public void CloseEditors()
		{
			m_ActionContainer.Close();
			m_ConditionContainer.Close();
		}

		public bool Validate(out string validateMessage, out Rule rule)
		{
			bool result = true;
			rule = new Rule();
	
			validateMessage = "Rule Ok";

			if (m_ConditionContainer.m_ConditionList.Count > 0)
			{
				GraphToRule converter = new GraphToRule(m_TargetObject.GetComponent<State>() as State);

				rule.m_Rule = converter.Convert(m_Rule.m_Name, m_ConditionContainer.m_ConditionList[0], m_ActionContainer.m_ActionList);
	            rule.SetContext(m_TargetObject.GetComponent<InferenceEngine>() as InferenceEngine);

				try
				{
					result = rule.Validate();
				}
				catch (InvalidRuleException e)
				{
					validateMessage = e.Message;
				}
			}

			return result;
		}

		public void SaveRule()
		{
			string validateMessage = "";
			Rule rule;

			if (Validate(out validateMessage, out rule))
				m_Rule.m_Rule = rule.m_Rule;
			else
				EditorUtility.DisplayDialog("Validate Rule", "Error validating " + m_Rule.m_Name + "\n " + validateMessage, "Ok");
		}
	}
}
#endif

