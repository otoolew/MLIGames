using System.Collections;
using UnityEngine;

namespace AiRuleEngine
{
	[ScriptName("FindNearestObject")]
	[ScriptCategory("Geography")]
	[ScriptReturnTypeAttribute("string")]

	public class FindNearestObject : BaseSensor
	{
		public string objectTag;
		
		public override object Execute(System.Type type)
		{
            GameObject closestGameObject = null;
		    float distanceFromClosest = 1000000000f;
		    float tempDistance;
		    GameObject[] objects = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
            string result = string.Empty;

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
                            closestGameObject = objects[i];
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
                            closestGameObject = objects[i];
                        }
                    }
                }

                if (closestGameObject != null)
                    result = closestGameObject.tag;
            }

            return result;
		}
	}
}