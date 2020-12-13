using System.Collections;
using UnityEngine;

namespace AiRuleEngine
{
	[ScriptName("LineOfSight")]
	[ScriptCategory("Geography")]
	[ScriptReturnTypeAttribute("boolean")]
	
	public class LineOfSight : BaseSensor 
	{
		public GameObject targetObject;
		public float maxDistance = 0;
	
		public override object Execute(System.Type type)
		{
            bool objectInSight = false;
		    Vector3 forward;

			if (maxDistance > 0)
			{
				forward  = targetObject.transform.TransformDirection(Vector3.forward);
                if (Physics.Raycast(targetObject.transform.position, forward, maxDistance))
                {
                    objectInSight = true;
                }
                else
                {
                    objectInSight = false;
                }
			}

            return objectInSight;
		}
	}
}
