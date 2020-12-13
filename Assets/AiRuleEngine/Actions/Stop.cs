using System.Collections;
using UnityEngine;

namespace AiRuleEngine
{
	[ScriptName("Stop")]
	[ScriptCategory("Movement")]
	
    public class Stop : BaseAction
	{
		public override bool Execute()
		{
			GetGameObject().GetComponent<Rigidbody>().velocity = Vector3.zero;

			return true;
		}
	}
}
