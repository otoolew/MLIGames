using System.Collections;
using UnityEngine;

namespace AiRuleEngine
{
	[ScriptName("SetPosition")]
	[ScriptCategory("Movement")]
	
    public class SetRotation : BaseAction 
	{
		public GameObject target;
		public float xRot = 0;
		public float yRot = 0;
		public float zRot = 0;
	
		public override bool Execute()
		{
			Quaternion rotation = new Quaternion();
			rotation.eulerAngles.Set(xRot, yRot, zRot);

			target.transform.rotation = rotation;

			return true;
		}
	}
}
