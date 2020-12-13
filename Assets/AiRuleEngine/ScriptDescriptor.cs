using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AiRuleEngine
{
	public class ScriptDescriptor
	{
        public Type m_Type;
		public string m_Name;
		public string m_Category;
		public string m_ReturnType;
		
		public ScriptDescriptor(Type type, string name, string category)
		{
            m_Type = type;
            m_Name = name;
            m_Category = category;
            m_ReturnType = string.Empty;
		}

        public ScriptDescriptor(Type type, string name, string category, string returnType)
		{
            m_Type = type;
            m_Name = name;
            m_Category = category;
            m_ReturnType = returnType;
		}
	}
}
