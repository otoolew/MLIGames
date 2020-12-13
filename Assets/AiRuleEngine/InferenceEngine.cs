using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XMLRules;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AiRuleEngine
{
	[ExecuteInEditMode]
	[AddComponentMenu("Inference Engine")]
	[RequireComponent(typeof(State))]
	[RequireComponent(typeof(RuleBase))]

	public class InferenceEngine : MonoBehaviour
	{
        public string m_RuleBaseFilePath = "";
		public int m_RuleBaseTick = 500;
		State m_State = null;
		RuleBase m_RuleBase = null;
		float m_TimeSinceLastTick = 0;

		void Awake()
		{
			// For some reason scripts instantiated during load never get destroyed
			// in the switch between edit and playmode. This bit of hack here destroys
			// any lingering scripts as they will be reloaded in the Open call.
			BaseScript[] scripts = gameObject.GetComponents<BaseScript>();

			foreach (BaseScript script in scripts)
				GameObject.DestroyImmediate(script);

			m_State = GetComponent<State>();
			m_RuleBase = GetComponent<RuleBase>();

			m_RuleBase.SetContext(this);
			Debug.Log("RuleBase: " + m_RuleBaseFilePath);
	
			if (m_RuleBaseFilePath.Length != 0) 
			{
				Open(m_RuleBaseFilePath);
			}
		}

		// Use this for initialization
		void Start()
		{
		}

		// Update is called once per frame
		void Update()
		{
			m_TimeSinceLastTick += Time.deltaTime;
			
			if (m_TimeSinceLastTick * 1000 > m_RuleBaseTick)
			{
				m_RuleBase.DebugMessage("Tick Rulebase");

				Tick();

				m_TimeSinceLastTick = 0;
			}
		}

		public bool Open(string filePath)
		{
			m_State = GetComponent<State>();
			m_RuleBase = GetComponent<RuleBase>();

			if (File.Exists(filePath)) 
			{
				XMLRulesType root;

				try
				{
					XMLRulesDoc doc = new XMLRulesDoc();
					root = new XMLRulesType(doc.Load(filePath));
				}
				catch (Exception e)
				{
					Debug.Log("Error on Open " + e.ToString());
					return false;
				}

				m_State.Clear();
				m_RuleBase.Clear();

				m_RuleBaseFilePath = filePath;
				m_RuleBase.SetContext(this);
				m_RuleBase.Load(root.GetRuleBase());
				m_State.Load(root.GetRuleBase());

				return true;
			}

			return false;
		}

		public bool Save(string fileName)
		{
			try
			{
				XMLRulesDoc document = new XMLRulesDoc();
				XMLRulesType root = new XMLRulesType();

				RuleBaseType ruleBase = new RuleBaseType();

				m_State.Save(ref ruleBase);
				m_RuleBase.Save(ref ruleBase);
				root.AddRuleBase(ruleBase);

				document.SetRootElementName("", "XMLRules");
				document.SetSchemaLocation("XMLRules.xsd"); // optional
				document.Save(fileName, root);
			}
			catch (Exception e)
			{
				Debug.Log("Error in Save " + e.ToString());
				return false;
			}
			
			return true;
		}

        public State GetState()
        {
            return m_State;
        }

		public void SetRuleBase(RuleBase ruleBase)
		{
			m_RuleBase = ruleBase;
			m_RuleBase.SetContext(this);
		}

		public RuleBase GetRuleBase()
		{
			return m_RuleBase;
		}

		public void AddRule(Rule rule)
		{
			m_RuleBase.AddRule(rule);
		}

		public bool RemoveRule (Rule rule)
		{
			return m_RuleBase.RemoveRule (rule);
		}

		public void ShowRuleBase ()
		{
			m_State.ShowVariables ();

			for (int i = 0; i < m_RuleBase.GetRuleCount(); i++) 
			{
				Rule rule = m_RuleBase.GetRuleAtIndex (i);

				if (rule != null)
					m_RuleBase.DebugMessage(rule.ToString ());
			}
		}

		public void Tick()
		{
			bool done = false;

			m_RuleBase.DebugMessage("Entering IE");

			while (!done) 
			{
				done = true;

				Rule rule;

				for (int i = 0; i < m_RuleBase.GetRuleCount(); i++) 
				{
					rule = m_RuleBase.GetRuleAtIndex(i);

					if (rule != null) 
					{
						m_RuleBase.DebugMessage("Evaluating rule");
						m_RuleBase.DebugMessage(rule.ToString());

						if (rule.Eval()) 
						{
							m_RuleBase.DebugMessage("Rule executing");
							rule.Execute ();

							done = false;
						} 
						else
							m_RuleBase.DebugMessage("Rule not executing");
					}
				}

				m_RuleBase.DebugMessage("Finished IE iteration");
			}

			m_RuleBase.DebugMessage("Exiting IE");

			m_RuleBase.Reset ();
		}

		#if UNITY_EDITOR

		public void ShowGUI()
		{	
			GUI.backgroundColor = new Color (0.7f, 0.7f, 0.7f);

			GUI.backgroundColor = new Color(0.8f,0.8f,1);

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button("Open", GUILayout.MaxWidth(120), GUILayout.ExpandWidth(false)))
			{
				string filePath = EditorUtility.OpenFilePanel("Choose RuleBase", Application.dataPath, "xml");
				if (filePath != null)
				{
					Open(filePath);
				}
			}

			if (GUILayout.Button("Save", GUILayout.MaxWidth(120), GUILayout.ExpandWidth(false)))
			{
				if (m_RuleBaseFilePath == String.Empty)
				{
					string filePath = EditorUtility.SaveFilePanel("Save RuleBase As", Application.dataPath, m_RuleBaseFilePath, "xml");

					if (filePath != String.Empty)
					{
						m_RuleBaseFilePath = filePath;
                        m_RuleBase.DebugMessage("Saving to " + filePath);
						Save(filePath);
					}
				}
				else
				{
					if (EditorUtility.DisplayDialog("Save new Rulebase " + m_RuleBaseFilePath, "Are you sure?", "Yes", "No"))
						Save(m_RuleBaseFilePath);
				}
			}

			if (GUILayout.Button("Save As..", GUILayout.MaxWidth(120), GUILayout.ExpandWidth(false)))
			{
					string filePath = EditorUtility.SaveFilePanel("Save RuleBase As", Application.dataPath, m_RuleBaseFilePath, "xml");
					
					if (filePath != String.Empty)
					{
						m_RuleBaseFilePath = filePath;
                        m_RuleBase.DebugMessage("Saving to " + filePath);
						Save(filePath);
					}
			}

			if (GUILayout.Button("New", GUILayout.MaxWidth(120), GUILayout.ExpandWidth(false)))
			{
				m_RuleBaseFilePath = String.Empty;
				m_State.Clear();
				m_RuleBase.Clear();
			}

			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal();
			GUI.color = Color.yellow;
			GUILayout.Label("RuleBase", GUILayout.MaxWidth(100), GUILayout.ExpandWidth(false));
			GUI.color = Color.white;
			m_RuleBaseFilePath = GUILayout.TextField(m_RuleBaseFilePath, GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
				GUILayout.Label("RuleBase Tick (ms):",GUILayout.MaxWidth(120), GUILayout.ExpandWidth(false));
				m_RuleBaseTick = EditorGUILayout.IntSlider(m_RuleBaseTick, 0, 5000, GUILayout.MaxWidth(50), GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();
		}
		#endif
	}
}
