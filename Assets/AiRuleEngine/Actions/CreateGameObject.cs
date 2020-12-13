using System.Collections;
using UnityEngine;

namespace AiRuleEngine
{
	[ScriptName("CreateGameObject")]
	[ScriptCategory("SceneManagement")]
	
    public class CreateGameObject : BaseAction
	{
		public GameObject objectPrefab;
		public Vector3 position = Vector3.zero;
		public Quaternion rotation = Quaternion.identity;

		public override bool Execute()
		{
			Instantiate(objectPrefab, position, rotation);
			
            return true;
		}
	}
}
