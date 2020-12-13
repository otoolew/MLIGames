#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AiRuleEngine
{
	public class ScriptManager
    {
		public ScriptManager()
		{
		}

        public static List<ScriptDescriptor> GetScriptsByType(Type baseType)
		{	
			List<MonoScript> scriptList = new List<MonoScript>();
            List<ScriptDescriptor> filteredScriptList = new List<ScriptDescriptor>();
			
			foreach (string scriptPath in AssetDatabase.GetAllAssetPaths())
            {
				if (scriptPath.EndsWith(".js") || scriptPath.EndsWith(".cs") || scriptPath.EndsWith(".boo"))
					scriptList.Add(AssetDatabase.LoadAssetAtPath(scriptPath, typeof(MonoScript)) as MonoScript);
			}
			
			foreach (MonoScript script in scriptList)
            {
				if (script != null)
				{
					Type scriptType = script.GetClass();

					if (baseType.IsAssignableFrom(scriptType))
					{
						if ((scriptType != baseType) && (!scriptType.IsAbstract))
						{
							string scriptName = string.Empty;
							string scriptCategory = string.Empty;
							string scriptReturnType = string.Empty;
							
							ScriptNameAttribute nameAttribute = scriptType.GetCustomAttributes(typeof(ScriptNameAttribute), false).FirstOrDefault() as ScriptNameAttribute;
							if (nameAttribute != null)
								scriptName = nameAttribute.name;
							
							ScriptCategoryAttribute categoryAttribute = scriptType.GetCustomAttributes(typeof(ScriptCategoryAttribute), false).FirstOrDefault() as ScriptCategoryAttribute;
							if (categoryAttribute != null)
								scriptCategory = categoryAttribute.category;

							ScriptReturnTypeAttribute returnAttribute = scriptType.GetCustomAttributes(typeof(ScriptReturnTypeAttribute), false).FirstOrDefault() as ScriptReturnTypeAttribute;
							if (returnAttribute != null)
								scriptReturnType = returnAttribute.returnType;

							if (scriptReturnType != null && scriptReturnType != string.Empty)
								filteredScriptList.Add(new ScriptDescriptor(scriptType, scriptName, scriptCategory, scriptReturnType));
							else
								filteredScriptList.Add(new ScriptDescriptor(scriptType, scriptName, scriptCategory));
						}
					}
				}
			}
			
			filteredScriptList = filteredScriptList.OrderBy(script => script.m_Category).ToList();
			
			return filteredScriptList;
		}
	}

	// For friendly names
	[AttributeUsage(AttributeTargets.Class)]
	public class ScriptNameAttribute : Attribute
    {	
		public string name;
		
		public ScriptNameAttribute(string name)
        {	
			this.name = name;
		}
	}
	
	// To categorize scripts
	[AttributeUsage(AttributeTargets.Class)]
	public class ScriptCategoryAttribute : Attribute
    {
		public string category;
		
		public ScriptCategoryAttribute(string category)
        {
			this.category = category;
		}
	}

	// To specify what script returns
	[AttributeUsage(AttributeTargets.Class)]
	public class ScriptReturnTypeAttribute : Attribute
    {
		public string returnType;
		
		public ScriptReturnTypeAttribute(string returnType)
        {
			this.returnType = returnType;
		}
	}
}
#endif