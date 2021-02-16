using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : StateMachineBehaviour
{
    AIController controller;
    bool isAttacking;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller = animator.GetComponent<AIController>();
        controller.GetComponent<AIMovement>().NavAgent.isStopped = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller.GetComponent<AIMovement>().NavAgent.isStopped = true; // For straifing set isStopped to false;
        if (controller.PlayerSense.IsPlayerInSight())
        {
            Vector3 rotVector = controller.PlayerSense.PlayerCharacter.WorldLocation - controller.transform.position;
            rotVector.y = 0f;
            Quaternion newRotation = Quaternion.LookRotation(rotVector);
            controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, newRotation, 1f);

            if (!isAttacking)
            {
                controller.StopCoroutine(FireRoutine());
                controller.StartCoroutine(FireRoutine());
            }
        }
        else
        {
            controller.SetBoolKey("HasTarget", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller.GetComponent<AIMovement>().NavAgent.isStopped = false;
    }
    IEnumerator FireRoutine()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.5f);
        controller.AssignedCharacter.AbilityController.CurrentAbility.Fire();
        isAttacking = false;
    }
}
