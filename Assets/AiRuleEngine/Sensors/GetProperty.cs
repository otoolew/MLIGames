using System.Collections;
using UnityEngine;

namespace AiRuleEngine
{
	[ScriptName("GetProperty")]
	[ScriptCategory("Query")]
	[ScriptReturnTypeAttribute("float")]

	public class GetProperty : BaseSensor 
	{
		public GameObject targetObject;
		public string property = "position";
        public string field = "x";

		public override object Execute(System.Type type)
		{
            float result = 0;
            Vector3 vector = Vector3.zero;

			if (property.ToLower() == "position")
			{
                vector = targetObject.transform.position;
			}

			if (property.ToLower() == "rotation")
			{
                vector = targetObject.transform.rotation.eulerAngles;
			}

            if ((property.ToLower() == "velocity") && (targetObject.GetComponent<Rigidbody>() != null))
            {
                vector = targetObject.GetComponent<Rigidbody>().velocity;
            }

            if (field.ToLower() == "x")
            {
                result = vector.x;
            }
            else if (field.ToLower() == "y")
            {
                result = vector.y;
            }
            else
            {
                result = vector.z;
            }

			return result;
		}
	}
}
