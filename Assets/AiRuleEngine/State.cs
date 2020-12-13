using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using XMLRules;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AiRuleEngine
{
    [ExecuteInEditMode]
    public class State : MonoBehaviour
    {
		Dictionary<string, Variable> m_Variables = new Dictionary<string, Variable>();
		bool m_DebugMode = false;
		string[] m_BooleanStrings = new string[2] { "false", "true" };
        bool m_AttachScript = false;
        EditMode m_EditMode = EditMode.Nothing;
		Variable m_EditVariable = new Variable();

        public bool Load(RuleBaseType ruleBase)
		{
			Clear();

			int variableCount = ruleBase.GetVariablesCount();
			
			for (int i = 0; i < variableCount; i++)
			{
				Variable variable = new Variable(gameObject, ruleBase.GetVariablesAt(i));
				AddVariable(variable);
			}
	
			return true;
		}

		public bool Save(ref RuleBaseType ruleBase)
		{
			foreach (KeyValuePair<string, Variable> pair in m_Variables) 
			{
				Variable variable = pair.Value;
				XMLRules.VariableType xmlVariable = new XMLRules.VariableType();
				variable.Save(ref xmlVariable);
				ruleBase.AddVariables(xmlVariable);
			}

			return true;
		}

		public void Clear()
		{
			foreach (KeyValuePair<string, Variable> pair in m_Variables) 
				pair.Value.RemoveScript();

			m_Variables.Clear();
		}

        public void ShowVariables()
        {
			if (!m_DebugMode)
				return;

            foreach (KeyValuePair<string, Variable> pair in m_Variables)
            {
                Variable variable = pair.Value;

                string message = variable.m_Name;
                message += " (";

                switch (variable.GetVariableType())
                {
                    case VariableType.BOOLEAN:
                        message += "boolean) ";
                        break;

                    case VariableType.INT:
                        message += "integer) ";
                        break;

                    case VariableType.FLOAT:
                        message += "float) ";
                        break;

                    case VariableType.STRING:
                        message += "string) ";
                        break;
                }

                message += variable.GetValue().ToString();

                System.Console.WriteLine(message);
            }
        }

        public void ResetVariables()
        {
            foreach (KeyValuePair<string, Variable> pair in m_Variables)
            {
                Variable variable = pair.Value;
                variable.m_IsDirty = false;
            }
        }

        public void SetVariables()
        {
            foreach (KeyValuePair<string, Variable> pair in m_Variables)
            {
                Variable variable = pair.Value;
                variable.m_IsDirty = true;
            }
        }

        public bool AddVariable(Variable variable)
        {
            if (variable == null)
                return false;

            if (m_Variables.ContainsKey(variable.m_Name))
                return false;

			variable.m_GameObject = gameObject;

            m_Variables.Add(variable.m_Name, variable);

            return true;
        }

        public bool RemoveVariable(string name)
        {
            if (name.Length == 0)
                return false;

            if (!m_Variables.ContainsKey(name))
                return false;

            return m_Variables.Remove(name);
        }

        public bool GetVariable(string name, out Variable variable)
        {
            variable = null;

            if (name.Length == 0)
                return false;

            if (!m_Variables.ContainsKey(name))
                return false;

            variable = m_Variables[name];

            if (variable == null)
                return false;

            return true;
        }

        public bool SetVariable(Variable variable)
        {
            if (!m_Variables.ContainsKey(variable.m_Name))
                return false;

            m_Variables[variable.m_Name] = variable;

            return true;
        }

        public bool HasVariable(string name)
        {
            if (name.Length == 0)
                return false;

            return m_Variables.ContainsKey(name);
        }

        public bool RenameVariable(string oldName, string newName)
        {
            Variable variable;

            if (HasVariable(newName))
                return false;

            if (GetVariable(oldName, out variable))
            {
                RemoveVariable(oldName);
                variable.m_Name = newName;
                AddVariable(variable);
            }
            else
                return false;

            return true;
        }

        public int GetVariables(out List<string> variableNames)
        {
            variableNames = new List<string>();

            foreach (KeyValuePair<string, Variable> pair in m_Variables)
            {
                variableNames.Add(pair.Key);
            }

            return variableNames.Count;
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (!gameObject.GetComponent(typeof(InferenceEngine)))
                DestroyImmediate(GetComponent(typeof(State)));

			foreach (KeyValuePair<string, Variable> pair in m_Variables)
			{
				if (Application.isPlaying)
					pair.Value.Update();

				if (m_DebugMode)
					Debug.Log("Update Value On " + pair.Value.m_Name + " to " + pair.Value.GetValue());
			}
        }

		#if UNITY_EDITOR

        public enum EditMode
        {
            Nothing,
            AddNewVariable,
            EditExistingVariable
        }

		public void ShowGUI()
		{	
			GUI.backgroundColor = new Color(0.7f,0.7f,0.7f);
			
			ShowStateGUI();
		}

		public void EditVariable(Variable editVariable)
		{
            m_EditVariable = editVariable;
			m_EditMode = EditMode.EditExistingVariable;

            m_AttachScript = editVariable.HasScript();
		}
		
		public void ShowStateGUI()
		{
			GUI.backgroundColor = new Color(0.8f,0.8f,1);
			
			GUILayout.BeginHorizontal();
			
            if (m_EditMode == EditMode.Nothing)
			{
				if (GUILayout.Button("Add Variable", GUILayout.MaxWidth(120), GUILayout.ExpandWidth(false)))
				{
					m_EditMode = EditMode.AddNewVariable;
					m_EditVariable = new Variable();
                    m_EditVariable.m_GameObject = gameObject;
                    m_AttachScript = false;
				}
			}
			else
			{
				if (GUILayout.Button("Cancel", GUILayout.MaxWidth(120), GUILayout.ExpandWidth(false)))
				{
					m_EditVariable.RemoveScript();
					m_EditMode = EditMode.Nothing;
					m_EditVariable = new Variable();
                    m_EditVariable.m_GameObject = gameObject;
				}
			}
			
			if ((m_EditMode == EditMode.AddNewVariable || m_EditMode == EditMode.EditExistingVariable) && GUILayout.Button("Ok", GUILayout.MaxWidth(120), GUILayout.ExpandWidth(false)))
			{
				if (m_EditMode == EditMode.AddNewVariable)
				{
					if (!HasVariable(m_EditVariable.m_Name) && m_EditVariable.m_Name != "")
					{
                        Variable newVariable = new Variable(m_EditVariable) ;

                        AddVariable(newVariable);
						
						m_EditMode = EditMode.Nothing;
					}
					else
					{
						EditorUtility.DisplayDialog("Error", "There is already a variable with this name in this state. Please give the variable a unique name to be called from", "Ok");
					}
				}
				else
				{
					Variable newVariable = new Variable(m_EditVariable.m_Name, m_EditVariable.GetValue());
                    newVariable.m_GameObject = gameObject;

					if (m_AttachScript)
					{
                        newVariable.SetScript(m_EditVariable.m_UpdateScript);
					}

                    SetVariable(newVariable);
					
					m_EditMode = EditMode.Nothing;
				}
			}
			
			GUILayout.FlexibleSpace(); 
			float nameWidth = 100;
			float typeWidth = 100;
			float valueWidth = 60;
			float scriptWidth = 180;
			
			EditorGUIUtility.labelWidth = 80;
			m_DebugMode = EditorGUILayout.Toggle("DebugMode: ", m_DebugMode, GUILayout.MaxWidth (95));
			GUILayout.EndHorizontal ();
			
			GUILayout.BeginHorizontal();
			GUI.color = Color.yellow;
			GUILayout.Label("Name", GUILayout.MaxWidth(nameWidth), GUILayout.MinWidth(nameWidth), GUILayout.ExpandWidth(false));
			GUILayout.Label("Type", GUILayout.MaxWidth(typeWidth), GUILayout.MinWidth(typeWidth), GUILayout.ExpandWidth(false));
			GUILayout.Label("Value", GUILayout.MaxWidth(valueWidth), GUILayout.MinWidth(valueWidth), GUILayout.ExpandWidth(false));
			GUILayout.Label("Script", GUILayout.MaxWidth(scriptWidth), GUILayout.MinWidth(scriptWidth), GUILayout.ExpandWidth(false));
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
			
			if (m_EditMode == EditMode.AddNewVariable || m_EditMode == EditMode.EditExistingVariable)
			{
				GUILayout.BeginHorizontal();
				
				if (m_EditMode == EditMode.AddNewVariable)
					m_EditVariable.m_Name = EditorGUILayout.TextField(m_EditVariable.m_Name, GUILayout.MaxWidth(nameWidth), GUILayout.MinWidth(nameWidth), GUILayout.ExpandWidth(false));
				else
					EditorGUILayout.LabelField(m_EditVariable.m_Name, GUILayout.MaxWidth(nameWidth), GUILayout.MinWidth(nameWidth), GUILayout.ExpandWidth(false));

				VariableType variableType = (VariableType)EditorGUILayout.EnumPopup(m_EditVariable.GetVariableType(), GUILayout.MaxWidth(typeWidth), GUILayout.MinWidth(typeWidth), GUILayout.ExpandWidth(false));
                if (variableType != m_EditVariable.GetVariableType())
                {
                    m_EditVariable.SetVariableType(variableType);
                    m_AttachScript = false;
                }

                if (m_EditVariable.GetVariableType () == VariableType.BOOLEAN)
				{
                    int index = (bool)m_EditVariable.GetValue() ? 1 : 0;

					index = EditorGUILayout.Popup(index, m_BooleanStrings, GUILayout.MaxWidth(valueWidth), GUILayout.MinWidth(valueWidth), GUILayout.ExpandWidth(false));
					
					m_EditVariable.SetValue(index == 1 ? true : false);
				}
				else if (m_EditVariable.GetVariableType() == VariableType.STRING)
				{
                    string stringValue = (string)m_EditVariable.GetValue();
					m_EditVariable.SetValue(EditorGUILayout.TextArea(stringValue, GUILayout.MaxWidth(valueWidth), GUILayout.MinWidth(valueWidth), GUILayout.ExpandWidth(false)));
				}
				else if (m_EditVariable.GetVariableType() == VariableType.INT)
				{
                    int intValue = (int)m_EditVariable.GetValue();
					m_EditVariable.SetValue(EditorGUILayout.IntField((int)intValue, GUILayout.MaxWidth(valueWidth), GUILayout.MinWidth(valueWidth), GUILayout.ExpandWidth(false)));
				}
				else if (m_EditVariable.GetVariableType() == VariableType.FLOAT)
				{
                    float floatValue = (float)m_EditVariable.GetValue();
					m_EditVariable.SetValue(EditorGUILayout.FloatField(floatValue, GUILayout.MaxWidth(valueWidth), GUILayout.MinWidth(valueWidth), GUILayout.ExpandWidth(false)));
				}
				
				if (m_AttachScript = EditorGUILayout.Toggle(m_AttachScript, GUILayout.MaxWidth(10)))
				{	
					List<ScriptDescriptor> sensorScripts = ScriptManager.GetScriptsByType(typeof(BaseSensor));
                    List<ScriptDescriptor> filteredSensorScripts = new List<ScriptDescriptor>();
					List<String> sensorScriptNames = new List<String>();
					
                    for (int i = 0; i < sensorScripts.Count(); i++)
					{
                        if (sensorScripts[i].m_ReturnType.ToUpper() == m_EditVariable.GetVariableType().ToString())
                        {
                            filteredSensorScripts.Add(sensorScripts[i]);
                            sensorScriptNames.Add(sensorScripts[i].m_Name);
                        }
					}

                    if (filteredSensorScripts.Count == 0)
                    {
                        EditorGUILayout.HelpBox("None of the sensor scripts return a type compatible with this variable type", MessageType.Info);
                    }
                    else
                    {
                        int sensorScriptIndex = -1;
                        
                        if (m_EditVariable.m_UpdateScriptType != null)
                            sensorScriptIndex = filteredSensorScripts.FindIndex(sensorScript => sensorScript.m_Type == m_EditVariable.m_UpdateScriptType);
                        
						int newSensorScriptIndex = EditorGUILayout.Popup(Mathf.Max(sensorScriptIndex, 0), sensorScriptNames.ToArray(), GUILayout.MaxWidth(scriptWidth), GUILayout.MinWidth(valueWidth), GUILayout.ExpandWidth(false));

                        if ((newSensorScriptIndex > -1) && (newSensorScriptIndex != sensorScriptIndex))
                        {
                            m_EditVariable.SetScript(filteredSensorScripts.ElementAtOrDefault(newSensorScriptIndex).m_Type);

                            if (m_DebugMode)
                                Debug.Log("Updating AttachedScript: " + filteredSensorScripts.ElementAt(newSensorScriptIndex).m_Name);
                        }
                    }
				}
				else
					m_EditVariable.RemoveScript();

				GUILayout.EndHorizontal();
				
				if (m_AttachScript && m_EditVariable.HasScript())
				{
                    m_EditVariable.m_UpdateScript.ShowScriptGUI();
				}
			}
			
			if (m_Variables.Count == 0)
			{
				EditorGUILayout.HelpBox("There are no variables in this state. Add some with the Add Variable button", MessageType.Info);
			}
			
			foreach (KeyValuePair<string, Variable> pair in m_Variables.ToArray())
			{
				if (pair.Value != null)
				{
					GUILayout.BeginHorizontal();
					GUI.color = Color.white;
					GUI.backgroundColor = Color.white;
					
                    if (Application.isPlaying)
					{	
						GUILayout.Label(pair.Value.m_Name, GUILayout.MaxWidth(nameWidth), GUILayout.MinWidth(nameWidth), GUILayout.ExpandWidth(false));
						GUILayout.Label(pair.Value.GetType().ToString(), GUILayout.MaxWidth(typeWidth), GUILayout.MinWidth(typeWidth), GUILayout.ExpandWidth(false));
						
						if (pair.Value.GetValue() != null)
							GUILayout.Label(pair.Value.GetValue().ToString(), GUILayout.MaxWidth(valueWidth), GUILayout.MinWidth(valueWidth), GUILayout.ExpandWidth(false));
						else
							GUILayout.Label("", GUILayout.MaxWidth(valueWidth), GUILayout.MinWidth(valueWidth), GUILayout.ExpandWidth(false));
						
						if (pair.Value.HasScript())
						{
							ScriptNameAttribute nameAttribute = pair.Value.m_UpdateScriptType.GetCustomAttributes(typeof(ScriptNameAttribute), false).FirstOrDefault() as ScriptNameAttribute;

							GUILayout.Label(nameAttribute.name, GUILayout.MaxWidth(scriptWidth), GUILayout.MinWidth(scriptWidth), GUILayout.ExpandWidth(false));
						}
					} 
					else
					{
						GUILayout.Label(pair.Value.m_Name, GUILayout.MaxWidth(nameWidth), GUILayout.MinWidth(nameWidth), GUILayout.ExpandWidth(false));
						GUILayout.Label(pair.Value.GetTypeString(), GUILayout.MaxWidth(typeWidth), GUILayout.MinWidth(typeWidth), GUILayout.ExpandWidth(false));
						
                        if (pair.Value.GetValue() != null)
							GUILayout.Label(pair.Value.GetValue().ToString(), GUILayout.MaxWidth(valueWidth), GUILayout.MinWidth(valueWidth), GUILayout.ExpandWidth(false));
						
						if (pair.Value.HasScript())
						{
							ScriptNameAttribute nameAttribute = pair.Value.m_UpdateScriptType.GetCustomAttributes(typeof(ScriptNameAttribute), false).FirstOrDefault() as ScriptNameAttribute;
							
							GUILayout.Label(nameAttribute.name, GUILayout.MaxWidth(scriptWidth), GUILayout.MinWidth(scriptWidth), GUILayout.ExpandWidth(false));
						}

						GUILayout.FlexibleSpace(); //Set layout passed this point to align to the right
						GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
						
                        if (GUILayout.Button("E", GUILayout.MaxWidth(20)))
						{
							EditVariable(pair.Value);
						}
						
						GUI.backgroundColor = new Color(1,0.6f,0.6f);
						
                        if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                        {
							if (EditorUtility.DisplayDialog("Delete Variable " + pair.Value.m_Name, "Are you sure?", "Yes", "No"))
							{
								pair.Value.RemoveScript();
								RemoveVariable(pair.Value.m_Name);
							}
						}
					}

					GUILayout.EndHorizontal();
				}
			}
			
			GUI.backgroundColor = Color.white;
			GUI.color = Color.white;
		}
		#endif
    }
}
