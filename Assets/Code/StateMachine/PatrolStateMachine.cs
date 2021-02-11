using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolStateMachine : StateMachineBehaviour
{
    [SerializeField] private AIController controller;
    [SerializeField] private AICharacter character;
    [SerializeField] private WaypointCircuit waypointCircuit;
    [SerializeField] private int currentWaypointIndex;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller = animator.GetComponent<AIController>();
        if(controller != null && controller.AssignedCharacter != null)
        {
            character = controller.AssignedCharacter;

            if (controller)
            {
                waypointCircuit = controller.WaypointCircuit;
                currentWaypointIndex = 0;

                character.MovementComp.SetDestination(waypointCircuit.WaypointList[currentWaypointIndex].transform.position);
            }
        }

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character != null)
        {
            if (DistanceToDestination(controller.AssignedCharacter.WorldLocation, waypointCircuit.WaypointList[currentWaypointIndex].transform.position) < 1)
            {              
                MoveToNextWaypoint();
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("OnStateExit");
        Debug.Log("PatrolStateMachine State Info\n" + JsonUtility.ToJson(stateInfo));
    }

    private void MoveToNextWaypoint()
    {
        if (character)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypointCircuit.WaypointList.Count)
            {
                currentWaypointIndex = 0;
            }

            character.MovementComp.SetDestination(waypointCircuit.WaypointList[currentWaypointIndex].transform.position);
        }
   
    }

    public float DistanceToDestination(Vector3 position, Vector3 location)
    {
        return Vector3.Distance(position, location);
    }
}
