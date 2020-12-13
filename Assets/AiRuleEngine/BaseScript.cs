using System;
using System.Reflection;
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
	public abstract class BaseScript : MonoBehaviour
    {
        private List<FieldInfo> m_Fields = null;
		public GameObject GetGameObject() { return gameObject; }
		public Transform GetTransform() { return gameObject.transform; }
		public Vector3 GetPosition() { return gameObject.transform.position; }
		public Quaternion GetRotation() { return gameObject.transform.rotation; }

		public BaseScript()
		{
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
            m_Fields = this.GetType().GetFields(flags).ToList();
		}
        
        void Start()
		{
		}

		void Update()
		{
		}

        public object GetFieldValue(string fieldName)
        {
            object result = null;

            int fieldIndex = m_Fields.FindIndex(field => field.Name == fieldName);

            if (fieldIndex >= 0)
                result = m_Fields[fieldIndex].GetValue(this);

            return result;
        }

        public bool SetFieldValue(string fieldName, object value)
        {
            int fieldIndex = m_Fields.FindIndex(field => field.Name == fieldName);

            if (fieldIndex >= 0)
                m_Fields[fieldIndex].SetValue(this, value);

            return (fieldIndex >= 0);
        }

#if UNITY_EDITOR
        public void ShowScriptGUI()
        {
            GUILayout.BeginVertical();

            foreach (FieldInfo field in m_Fields)
            {
                if (field.FieldType == typeof(float))
                    field.SetValue(this, EditorGUILayout.FloatField(field.Name, (float)field.GetValue(this), GUILayout.ExpandWidth(true)));
                else if (field.FieldType == typeof(string))
                    field.SetValue(this, EditorGUILayout.TextField(field.Name, (string)field.GetValue(this), GUILayout.ExpandWidth(true)));
                else if (field.FieldType == typeof(int))
                    field.SetValue(this, EditorGUILayout.IntField(field.Name, (int)field.GetValue(this), GUILayout.ExpandWidth(true)));
                else if (typeof(UnityEngine.GameObject).IsAssignableFrom(field.FieldType))
                {
                    object value = field.GetValue(this);

                    field.SetValue(this, EditorGUILayout.ObjectField(field.Name, (UnityEngine.GameObject)value, typeof(UnityEngine.GameObject), true, GUILayout.ExpandWidth(true)));
                }
            }

            GUILayout.EndVertical();
        }
#endif

        public static bool Load(ScriptReference xmlScriptReference, GameObject gameObject, out BaseScript newScript)
        {
            bool result = true;
            newScript = null;

            if (xmlScriptReference != null && xmlScriptReference.HasScriptName())
            {
                string name = (string)xmlScriptReference.GetScriptName().Value;
 
                Type scriptType = Type.GetType(name);

                if (scriptType != null)
                {
                    newScript = gameObject.AddComponent(scriptType) as BaseScript;

                    for (int i = 0; i < xmlScriptReference.GetParameterCount(); i++)
                    {
                        ParameterType parameter = xmlScriptReference.GetParameterAt(i);

                        if (parameter.HasName())
                        {
                            string parameterName = parameter.GetName().Value;
                            string parameterType = parameter.GetType2().Value;
							object target = null;

                            if (parameterType == typeof(UnityEngine.GameObject).ToString())
                            {
                                if (!Variable.ObjectFromString(parameter.GetValue().GetString2().Value, out target))
                                {
                                    Debug.LogError("Script " + name + " references an object (" + parameter.GetValue().GetString2().Value + ") that was not found.");
                                }
                            }
                            else
                            {
                                target = Variable.GetValueFromConstant(parameter.GetValue());
                            }

                            if (!newScript.SetFieldValue(parameterName, target))
                            {
                                Debug.LogError("Script " + name + " failed to set a value for " + parameterName);
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("Script (" + name + ") is referenced as a sensor or an action but was not found");
                    result = false;

                }
            }

            return result;
        }

        public static bool Save(BaseScript script, out ScriptReference xmlScriptReference)
        {
            xmlScriptReference = new ScriptReference();

            xmlScriptReference.AddScriptName(new SchemaString(script.GetType().ToString()));

            foreach (FieldInfo field in script.m_Fields)
            {
                ParameterType parameter = new ParameterType();

                parameter.AddName(new SchemaString(field.Name));
                parameter.AddType2(new SchemaString(field.FieldType.ToString()));

                object value = field.GetValue(script);
                if (value != null)
                    parameter.AddValue(Variable.GetConstantFromValue(value));
                else
                    Debug.LogError("Error saving value for field " + field.Name + " in script " + script.GetType().ToString());

                xmlScriptReference.AddParameter(parameter);
            }

            return true;
        }
	}
}

