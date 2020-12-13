using System.Collections;
using UnityEngine;

namespace AiRuleEngine
{
	[ScriptName("DistanceBetweenTwoObjects")]
	[ScriptCategory("Geography")]
	[ScriptReturnTypeAttribute("float")]
    public class DistanceBetweenTwoObjects : BaseSensor 
	{
		public GameObject gameObject1;
		public GameObject gameObject2;
		
		public override object Execute(System.Type type)
		{
            float result = 0;

			if (gameObject1 != null && gameObject2 != null )
				result = Vector3.Distance(gameObject1.transform.position, gameObject2.transform.position);

            return result;
		}
	}
}
