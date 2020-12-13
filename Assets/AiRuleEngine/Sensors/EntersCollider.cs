using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiRuleEngine
{
	[ScriptName("EntersCollider")]
	[ScriptCategory("Collision")]
	[ScriptReturnTypeAttribute("boolean")]
	
    public class EntersCollider : BaseSensor
	{
		private bool isColliding = false;
		public string objectTag = "";

		void OnTriggerEnter(Collider other) 
		{
            if ((objectTag.Replace(" ", string.Empty) == "") || (other.gameObject.tag == objectTag))
            {
                isColliding = true;
            }
		}

		void OnTriggerExit(Collider other) 
		{
            if ((objectTag.Replace(" ", string.Empty) == "") || (other.gameObject.tag == objectTag))
            {
                isColliding = false;
            }
		}

		public override object Execute(System.Type type)
		{
			return isColliding;
		}
	}
}
