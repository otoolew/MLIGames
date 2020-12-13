using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Altova.Types;
using XMLRules;
using UnityEngine;

namespace AiRuleEngine
{
    public enum VariableType
    {
        BOOLEAN,
        INT,
        FLOAT,
        STRING
    }
	
	public class Variable
    {
		public string m_Name { get; set; }
		public VariableType m_Type { get; private set; }
        object m_Value;
		public bool m_IsDirty { get; set; }
        public BaseSensor m_UpdateScript { get; private set; }
        public Type m_UpdateScriptType { get; set; }
		public GameObject m_GameObject { get; set; }
		
        public Variable() 
        {
            m_Name = "";
            m_IsDirty = false;
            m_Value = new int();
            m_Type = VariableType.INT;
            m_UpdateScript = null;
            m_UpdateScriptType = null;
        }

        public Variable(Variable copy)
        {
            Assign(copy);
        }

		public Variable(string name, VariableType type)
		{
			m_Name = name;
			m_IsDirty = false;
			m_Type = type;
            SetVariableType(type);
            m_UpdateScript = null;
            m_UpdateScriptType = null;
		}

		public Variable(GameObject gameObject, XMLRules.VariableType variable)
		{
            m_GameObject = gameObject;
			m_IsDirty = true;

            Load(variable);
		}

		public Variable(string name, bool value)
        {
            m_Name = name;
            m_Type = VariableType.BOOLEAN;
            m_Value = value;
            m_IsDirty = true;
            m_UpdateScript = null;
            m_UpdateScriptType = null;
        }

		public Variable(string name, int value)
        {
            m_Name = name;
            m_Type = VariableType.INT;
            m_Value = value;
            m_IsDirty = true;
            m_UpdateScript = null;
            m_UpdateScriptType = null;
        }

		public Variable(string name, float value)
        {
            m_Name = name;
            m_Type = VariableType.FLOAT;
            m_Value = value;
            m_IsDirty = true;
            m_UpdateScript = null;
            m_UpdateScriptType = null;
        }

		public Variable(string name, string value)
        {
            m_Name = name;
            m_Type = VariableType.STRING;
            m_Value = value;
            m_IsDirty = true;
            m_UpdateScript = null;
            m_UpdateScriptType = null;
        }

		public Variable(string name, object value)
		{
			m_Name = name;

			if (value.GetType() == typeof(string)) 
				m_Type = VariableType.STRING;
			else if (value.GetType() == typeof(Boolean)) 
				m_Type = VariableType.BOOLEAN;
			else if (value.GetType() == typeof(float)) 
				m_Type = VariableType.FLOAT;
			else if (value.GetType() == typeof(int)) 
				m_Type = VariableType.INT;

			m_Value = value;
			m_IsDirty = true;
            m_UpdateScript = null;
            m_UpdateScriptType = null;
		}

		public void OnDestroy()
		{
			if (m_UpdateScript != null)
				UnityEngine.Object.DestroyImmediate(m_UpdateScript);
		}

        static public ConstantType GetConstantFromValue(object value)
        {
            if (value == null)
            {
                Debug.LogError("Error null value in GetConstantFromValue");
                return null;
            }

            ConstantType constantType = new ConstantType();

            if (value.GetType() == typeof(bool))
            {
                constantType.AddBoolean(new SchemaBoolean((bool)value));
            }
            else if (value.GetType() == typeof(float))
            {
                constantType.AddFloat2(new SchemaDecimal((decimal)(float)value));
            }
            else if (value.GetType() == typeof(int))
            {
                long longValue = (long)(int)value;
                constantType.AddInteger(new SchemaLong(longValue));
            }
            else if (value.GetType() == typeof(string))
            {
                constantType.AddString2(new SchemaString((string)value));
            }
            else
            {
                string objectString;

                if (ObjectToString(value, out objectString))
                    constantType.AddString2(new SchemaString(objectString));
                else
                    constantType.AddString2(new SchemaString(value.GetType().ToString()));
            }

            return constantType;
        }

		static public ConstantType GetConstantFromValue(VariableType type, object value)
		{
            if (value == null)
            {
                Debug.LogError("Error null value in GetConstantFromValue");
                return null;
            }

			ConstantType constantType = new ConstantType();
			
			switch (type) 
			{
			case VariableType.BOOLEAN:
				constantType.AddBoolean(new SchemaBoolean((bool)value));
				break;
				
			case VariableType.FLOAT:
				constantType.AddFloat2(new SchemaDecimal((decimal)(float)value));
				break;
				
			case VariableType.INT:
                constantType.AddInteger(new SchemaLong((long)(int)value));
				break;
				
			case VariableType.STRING:
				constantType.AddString2(new SchemaString((string)value));
                break;
			}

			return constantType;
		}

        static public object GetValueFromConstant(ConstantType constant)
        {
            object result = null;

            if (constant.HasBoolean())
            {
                result = (bool)constant.GetBoolean().Value;
            }
            else if (constant.HasInteger())
            {
                result = (int)constant.GetInteger().Value;
            }
            else if (constant.HasFloat2())
            {
                result = (float)constant.GetFloat2().Value;
            }
            else if (constant.HasString2())
            {
                result = (string)constant.GetString2().Value;
            }

            return result;
        }

		static public object GetValueFromConstant(VariableType type, ConstantType constant)
		{
			object result = null; 

			if (constant.HasBoolean())
			{
				bool boolValue = (bool)constant.GetBoolean().Value;
				
				switch(type)
				{
				case VariableType.BOOLEAN:
					result = boolValue;
					break;
					
				case VariableType.INT:
					result = (int)(boolValue ? 1 : 0);
					break;
					
				case VariableType.FLOAT:	
					result = (float)(boolValue ? 1 : 0);
					break;
					
				case VariableType.STRING:
					result = (boolValue ? "true" : "false");
					break;
				}
			}
			else if (constant.HasInteger())
			{
				int intValue = (int)constant.GetInteger().Value;
				
				switch(type)
				{
				case VariableType.BOOLEAN:
					result = (intValue == 1) ? true : false;
					break;
					
				case VariableType.INT:
					result = intValue;
					break;
					
				case VariableType.FLOAT:	
					result = intValue;
					break;
					
				case VariableType.STRING:
					result = intValue.ToString();
					break;
				}
			}
			else if (constant.HasFloat2())
			{
				float floatValue = (float)constant.GetFloat2().Value;
				
				switch(type)
				{
				case VariableType.BOOLEAN:
					result = (floatValue == 1) ? true : false;
					break;
					
				case VariableType.INT:
					result = (int)floatValue;
					break;
					
				case VariableType.FLOAT:	
					result = floatValue;
					break;
					
				case VariableType.STRING:
					result = floatValue.ToString();
					break;
				}
			}
			else if (constant.HasString2())
			{
				string stringValue = constant.GetString2().Value;
				
				switch(type)
				{
				case VariableType.BOOLEAN:
					result = (stringValue == "true") ? true : false;
					break;
					
				case VariableType.STRING:
					result = stringValue;
					break;
					
				case VariableType.INT:
					result = stringValue;
					break;
					
				case VariableType.FLOAT:	
					result = Convert.ToSingle(stringValue);
					break;
				}
			}

			return result;
		}

		static public bool ObjectToString(object target, out string objectString)
		{
			objectString = ((UnityEngine.GameObject)target).name;
		
			return true;
		}

		static public bool ObjectFromString(string objectString, out object target)
		{
			target = null;

			if (objectString == String.Empty)
			{
				Debug.Log("Invalid string argument");
				return false;
			}
			
			target = GameObject.Find(objectString);

			return true;
		}

		public bool Load(XMLRules.VariableType variable)
		{
			m_Name = (string)variable.GetName().Value;
			m_Type = (VariableType)variable.GetType2().Value;
			m_Value = GetValueFromConstant((VariableType)variable.GetType2().Value, variable.GetValue());

            if (variable.HasUpdateScript())
            {
                BaseScript updateScript;

				RemoveScript();

                if (BaseScript.Load(variable.GetUpdateScript(), m_GameObject, out updateScript))
                {
                    m_UpdateScriptType = updateScript.GetType();
                    m_UpdateScript = (BaseSensor)updateScript;
                }
            }

			m_IsDirty = true;

			return true;
		}

		public bool Save(ref XMLRules.VariableType variable)
		{
            bool result = true;

			variable.AddName(new SchemaString(m_Name));
			variable.AddType2(new SchemaLong((long)m_Type));
			variable.AddValue(GetConstantFromValue(m_Type, m_Value));
            
            if (m_UpdateScript != null)
            {
                ScriptReference scriptReference;

                if (BaseScript.Save(m_UpdateScript, out scriptReference))
                {
                    variable.AddUpdateScript(scriptReference);
                }
                else
                {
                    Debug.LogError("Error saving update script for variable " + m_Name);
                    result = false;
                }
            }
			
			return result;
		}

        public void SetValue(bool value)
        {
            m_Value = value;
	        m_Type = VariableType.BOOLEAN;
	        m_IsDirty = true;
        }

        public void SetValue(int value)
        {
            m_Value = value;
	        m_Type = VariableType.INT;
	        m_IsDirty = true;
        }

        public void SetValue(float value)
        {
            m_Value = value;
	        m_Type = VariableType.FLOAT;
	        m_IsDirty = true;
        }

        public void SetValue(string value)
        {
            m_Value = value;
	        m_Type = VariableType.STRING;
	        m_IsDirty = true;
        }

		public void SetValue(object value)
		{
			m_Value = value;
			m_IsDirty = true;

			if (value.GetType() == typeof(bool))
				m_Type = VariableType.BOOLEAN;
			else if (value.GetType() == typeof(int))
				m_Type = VariableType.INT;
			else if (value.GetType() == typeof(float))
				m_Type = VariableType.FLOAT;
			else if (value.GetType() == typeof(string))
				m_Type = VariableType.STRING;
		}

        public VariableType GetVariableType()
		{
			return m_Type;
		}

		public void SetVariableType(VariableType type)
		{
			m_Type = type;

            switch (m_Type)
            {
                case VariableType.BOOLEAN:
                    m_Value = new bool();
                    break;

                case VariableType.INT:
                    m_Value = new int();
                    break;

                case VariableType.FLOAT:
                    m_Value = new float();
                    break;

                case VariableType.STRING:
			        m_Value = string.Empty;
                    break;
            }
		}

		public string GetTypeString()
		{
			string result = "";

			switch (m_Type) 
			{
			case VariableType.BOOLEAN:
				result = "boolean";
				break;
	
			case VariableType.STRING:
				result = "string";
				break;
	
			case VariableType.INT:
				result = "int";
				break;
	
			case VariableType.FLOAT:	
				result = "float";
				break;   
			}

			return result;
		}

        public object GetValue()
		{
			return m_Value;
		}

        public void Assign(Variable rhs)
		{
            m_Name = rhs.m_Name;
			m_Type = rhs.m_Type;
			m_Value = rhs.m_Value;
            m_UpdateScriptType = rhs.m_UpdateScriptType;
            m_UpdateScript = rhs.m_UpdateScript;
			m_IsDirty = rhs.m_IsDirty;
            m_GameObject = rhs.m_GameObject;
		}

        public bool IsEqual(Variable rhs)
		{
			return (m_Name == rhs.m_Name);
		}

		public void SetScript(Type scriptType)
		{
			RemoveScript();

            m_UpdateScriptType = scriptType;

            if (m_UpdateScriptType != null)
                m_UpdateScript = (BaseSensor)m_GameObject.AddComponent(m_UpdateScriptType);
            else
                m_UpdateScript = null;
		}

		public void SetScript(BaseSensor script)
		{
			RemoveScript();

			m_UpdateScript = script;

			if (script)
				m_UpdateScriptType = script.GetType();
		}

		public bool HasScript()
		{
			return (m_UpdateScript != null);
		}

		public void RemoveScript()
		{
			if (m_UpdateScript != null)
			{
				UnityEngine.Object.DestroyImmediate(m_UpdateScript);
			}

			m_UpdateScript = null;
			m_UpdateScriptType = null;
		}

		public void Update()
		{
			if (HasScript())
			{
				object newValue = m_UpdateScript.Execute(m_Value.GetType());
				if (newValue != m_Value)
				{
					m_Value = newValue;
					m_IsDirty = true;
				}
			}
		}
    }
}
