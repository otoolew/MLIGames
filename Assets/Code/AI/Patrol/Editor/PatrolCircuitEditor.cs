using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(PatrolCircuit))]
public class PatrolCircuitEditor : Editor
{
    private void OnSceneGUI()
    {
        PatrolCircuit patrolCircuit = (PatrolCircuit)target;

        if(patrolCircuit.PatrolpointList.Count > 0)
        {
            Handles.color = Color.white;
            PatrolPoint firstPoint = null;

            for (int i = 0; i < patrolCircuit.PatrolpointList.Count; i++)
            {
                if(i == 0)
                {
                    firstPoint = patrolCircuit.PatrolpointList[0];
                    continue;
                }

                Handles.DrawLine(patrolCircuit.PatrolpointList[i - 1].transform.position, patrolCircuit.PatrolpointList[i].transform.position);
            }
            Handles.DrawLine(patrolCircuit.PatrolpointList[0].transform.position, patrolCircuit.PatrolpointList[patrolCircuit.PatrolpointList.Count-1].transform.position);
        }
    }
}
