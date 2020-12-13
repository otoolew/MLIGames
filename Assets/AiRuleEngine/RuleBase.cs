using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altova.Types;
using XMLRules;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AiRuleEngine
{
    [ExecuteInEditMode]
    public class RuleBase : MonoBehaviour
    {
        InferenceEngine m_Context;
        List<Rule> m_Rules = new List<Rule>();
		public bool m_DebugMode = false;

        public RuleBase()
        {
            m_Context = null;
        }

        public RuleBase(InferenceEngine context)
        {
            m_Context = context;
        }

        public RuleBase(RuleBaseType ruleBase)
        {
            int ruleCount = ruleBase.GetRulesCount();

            for (int i = 0; i < ruleCount; i++)
            {
                Rule thisRule = new Rule(ruleBase.GetRulesAt(i));

                AddRule(thisRule);
            }
        }

		public void Clear()
		{
			m_Rules.Clear ();
		}

		public bool Load(RuleBaseType ruleBase)
        {
			m_Rules.Clear();

			int ruleCount = ruleBase.GetRulesCount();

			for (int i = 0; i < ruleCount; i++)
			{
                Rule thisRule = new Rule(ruleBase.GetRulesAt(i));

			    AddRule(thisRule);
			}

			return Validate();
        }

		public bool Save(ref RuleBaseType ruleBase)
        {
            try
            {
                int ruleCount = m_Rules.Count();

                for (int i = 0; i < ruleCount; i++)
                {
					ruleBase.AddRules(m_Rules[i].m_Rule);
                }  
            }
            catch (Exception e)
            {
                Debug.Log("Error in Save RuleBase " + e.ToString());
                return false;
            }

            return true;
        }

        public void SetContext(InferenceEngine inferenceEngine)
        {
            m_Context = inferenceEngine;
        }

        public InferenceEngine GetContext()
        {
            return m_Context;
        }

        public void AddRule(Rule rule)
        {
            rule.SetContext(m_Context);

            m_Rules.Add(rule);
        }

        public void AddRule(RuleType rule)
        {
            Rule newRule = new Rule(rule);
            newRule.SetContext(m_Context);

            m_Rules.Add(newRule);
        }

        public bool RemoveRule(Rule rule)
        {
            return m_Rules.Remove(rule);
        }

        public int GetRuleCount()
        {
            return m_Rules.Count;
        }

        public Rule GetRuleAtIndex(int index)
        {
            return m_Rules[index];
        }

        public Rule GetRule(string name)
        {
            Rule ruleFound = new Rule();

            ruleFound = null; 

            for (int i = 0; i < m_Rules.Count(); i++)
            {
                if (m_Rules[i].m_Name == name)
                    ruleFound = m_Rules[i];
            }

            return ruleFound;
        }

        public bool RemoveRule(string name)
        {
            bool removed = false;

            for (int i = 0; i < m_Rules.Count(); i++)
            {
                if (m_Rules[i].m_Name == name)
                       removed = m_Rules.Remove(m_Rules[i]);
            }

            return removed;
        }

        public List<Rule> GetRules()
        {
            return m_Rules;
        }

        public void Reset()
        {
            foreach (Rule rule in m_Rules)
            {
               rule.Reset();
            }
        }

		public bool Validate()
		{
			bool result = true;

			foreach (Rule rule in m_Rules)
			{
				result = rule.Validate() && result;
			}

			return result;
		}

		public void Update()
		{
			if (!gameObject.GetComponent(typeof(InferenceEngine)))
				DestroyImmediate(GetComponent(typeof(RuleBase)));
		}

		public void DebugMessage(string message)
		{
			if (m_DebugMode)
				Debug.Log(message);
		}

		#if UNITY_EDITOR
		
		public void ShowGUI()
		{	
			GUI.backgroundColor = new Color(0.7f,0.7f,0.7f);
			
			ShowRulesGUI();
		}
		
		public void ShowRulesGUI()
		{
			GUI.backgroundColor = new Color(0.8f,0.8f,1);
			
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button("Add Rule", GUILayout.MaxWidth(120), GUILayout.ExpandWidth(false)))
			{
				RuleType xmlRule = new RuleType();
				
				int counter = 0;
				string newName = "New Rule ";
				
				List<Rule> newRules = m_Rules.FindAll(x => x.m_Name.Contains(newName));
				foreach ( Rule rule in newRules)
				{
					string numberPart = rule.m_Name.TrimStart(newName.ToCharArray());
					if (Convert.ToInt32(numberPart) > counter)
						counter = Convert.ToInt32(numberPart);
				}
				
				newName += ++counter;
				
				xmlRule.AddName(new Altova.Types.SchemaString(newName));
				
				Rule newRule = new Rule(xmlRule);
				newRule.SetContext(m_Context);
				
				m_Rules.Add(newRule);
				
				RuleInspector ruleInspector = new RuleInspector(newRule, gameObject);
				ruleInspector.ShowEditor();
			}
			
			GUILayout.FlexibleSpace(); //Set layout passed this point to align to the right
			
			EditorGUIUtility.labelWidth = 80;
			m_DebugMode = EditorGUILayout.Toggle("DebugMode: ", m_DebugMode, GUILayout.MaxWidth(95));
			GUILayout.EndHorizontal ();
			
			if (m_Rules.Count != 0)
			{
				GUILayout.BeginHorizontal();
				GUI.color = Color.yellow;
				GUILayout.Label("Rule", GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true));
				GUI.color = Color.white;
				GUILayout.EndHorizontal();
			} 
			else 
			{
				EditorGUILayout.HelpBox("There are no rules in this rule base. Add some with the Add Rule button", MessageType.Info);
			}
			
			foreach (Rule rule in m_Rules.ToArray())
			{
				if (rule != null)
				{
					GUILayout.BeginHorizontal();
					if (!Application.isPlaying)
					{	
						GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f, 0.3f);
						rule.m_Name = EditorGUILayout.TextField(rule.m_Name, GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true));
						if (GUI.changed == true)
						{
							rule.m_Rule.ReplaceNameAt(new Altova.Types.SchemaString(rule.m_Name), 0);
						}
					} 
					else
					{
						GUI.backgroundColor = new Color(0.7f,0.7f,0.7f);
						GUI.color = new Color(0.8f,0.8f,1f);
						GUILayout.Label(rule.m_Name, GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true));
					}
					
					GUI.color = Color.white;
					GUI.backgroundColor = Color.white;
					
					if (!Application.isPlaying)
					{
						
						GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
						if (GUILayout.Button("E", GUILayout.MaxWidth(20)))
						{
							RuleInspector ruleInspector = new RuleInspector(rule, gameObject);
							ruleInspector.ShowEditor();
						}
						
						GUI.backgroundColor = new Color(1,0.6f,0.6f);
						if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
						{
							if (EditorUtility.DisplayDialog("Delete Rule " + rule.m_Name, "Are you sure?", "Yes", "No"))
								RemoveRule(rule.m_Name);
						}
					}
					
					GUI.backgroundColor = new Color(0.7f,0.7f,0.7f);
					GUILayout.EndHorizontal();
				}
			}
			
			GUI.backgroundColor = Color.white;
			GUI.color = Color.white;
		}
		#endif
    }
}
