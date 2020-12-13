using System.Collections;
using UnityEngine;

namespace AiRuleEngine
{
	[ScriptName("DistanceFromSelf")]
	[ScriptCategory("Geography")]
	[ScriptReturnTypeAttribute("float")]
	
    public class DistanceFromSelf : BaseSensor 
	{
		public GameObject targetObject;
		
		public override object Execute(System.Type type)
		{
            float result = 0;

			if (targetObject != null)
				result = Vector3.Distance(GetGameObject().transform.position, targetObject.transform.position);

            return result;
		}
	}
}
