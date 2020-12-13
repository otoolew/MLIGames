using System.Collections;
using UnityEngine;

namespace AiRuleEngine
{
	[ScriptName("Base Sensor")]
	[ScriptCategory("Base")]
	[ScriptReturnTypeAttribute("object")]
	public abstract class BaseSensor : BaseScript
	{
		public abstract object Execute(System.Type type);
	}
}