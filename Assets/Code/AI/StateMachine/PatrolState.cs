using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : StateMachineBehaviour
{
    [SerializeField] private AIController controller;
    [SerializeField] private AICharacter character;
    [SerializeField] private WaypointCircuit waypointCircuit;
    [SerializeField] private int currentWaypointIndex;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller = animator.GetComponent<AIController>();
        if (controller != null && controller.AssignedCharacter != null)
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
        if (controller != null)
        {

            if (controller.PlayerSense.IsPlayerInSight())
            {
                controller.SetTriggerKey("EnemySighted", true);
                controller.SetBoolKey("HasTarget", true);
                return;
            }

            if (Vector3.Distance(controller.AssignedCharacter.WorldLocation, waypointCircuit.WaypointList[currentWaypointIndex].transform.position) < 1)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypointCircuit.WaypointList.Count)
                {
                    currentWaypointIndex = 0;
                }

                character.MovementComp.SetDestination(waypointCircuit.WaypointList[currentWaypointIndex].transform.position);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
