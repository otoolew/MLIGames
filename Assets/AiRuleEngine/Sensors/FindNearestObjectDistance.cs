using System.Collections;
using UnityEngine;

namespace AiRuleEngine
{
	[ScriptName("FindNearestObjectDistance")]
	[ScriptCategory("Geography")]
	[ScriptReturnTypeAttribute("float")]
	
	public class FindNearestObjectDistance : BaseSensor
	{
		public string objectTag;
		
		public override object Execute(System.Type type)
		{
		    float distanceFromClosest = float.MaxValue;
		    float tempDistance;
		    GameObject[] objects = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));

			if (objects.Length > 1)
			{
				if (objectTag.Replace(" ", string.Empty) == "")
				{
					for (int i = 0; i < objects.Length; i++)
					{
						tempDistance = Vector3.Distance(GetGameObject().transform.position, objects[i].transform.position);
						if (tempDistance < distanceFromClosest && objects[i] != GetGameObject())
						{
							distanceFromClosest = tempDistance;
						}
					}
				}
				else
				{
					for (int i = 0; i < objects.Length; i++)
					{
						tempDistance = Vector3.Distance(GetGameObject().transform.position, objects[i].transform.position);
						if (tempDistance < distanceFromClosest && objects[i] != GetGameObject() && objects[i].tag == objectTag)
						{
							distanceFromClosest = tempDistance;
						}
					}
				}
			}

            return distanceFromClosest;
		}
	}
}