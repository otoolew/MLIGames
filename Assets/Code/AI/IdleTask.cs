using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game/AI/Task/Idle")]
public class IdleTask : AITask
{
    private AIController controller;

    [SerializeField] private float waitTime;
    public override void Init(MonoBehaviour runner)
    {
        this.controller = (AIController)runner;
    }
    public override void DelegateTask()
    {
        controller.StartCoroutine(TaskCoroutine());
    }

    public override IEnumerator TaskCoroutine()
    {
        Debug.Log(controller.gameObject.name + "'s IdleRoutine -> Waiting for a sec...");
        yield return new WaitForSeconds(waitTime);
        Debug.Log(controller.gameObject.name + "'s IdleRoutine -> COMPLETE!");
        controller.AddAITask(PickNoseTask.Create());
    }
    public static IdleTask Create()
    {
        return CreateInstance<IdleTask>();
    }
    //public override IEnumerator TaskCoroutine()
    //{
    //    throw new NotImplementedException();
    //}

    //public override Func<bool> TaskFinished()
    //{
    //    throw new NotImplementedException();
    //}
}
