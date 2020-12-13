using System.Collections;
using UnityEngine;

namespace AiRuleEngine
{
	[ScriptName("Base Action")]
	[ScriptCategory("Base")]
	public abstract class BaseAction : BaseScript
    {
        public abstract bool Execute();
	}
}