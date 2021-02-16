using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game/AI/Task/Idle")]
public class IdleTask : AITask
{
    private AIController controller;
    public override string TaskName { get => "IdleTask"; }// could be object.name?

    [SerializeField] private float waitTime;
    public float WaitTime { get => waitTime; set => waitTime = value; }

    public void Init(MonoBehaviour runner, float waitTime)
    {
        this.controller = (AIController)runner;
        this.waitTime = waitTime;
    }

    public override void Init(MonoBehaviour runner)
    {
        this.controller = (AIController)runner;
    }
    public override void DelegateTask()
    {
        controller.StartCoroutine(TaskCoroutine());
    }

    public IEnumerator TaskCoroutine()
    {
        Debug.Log(controller.gameObject.name + "'s IdleRoutine -> Waiting for a sec...");
        yield return new WaitForSeconds(waitTime);
        Debug.Log(controller.gameObject.name + "'s IdleRoutine -> COMPLETE!");
        //controller.AddAITask(PickNoseTask.Create());
    }

    public override void Tick(AICharacter character)
    {
        Debug.Log("PatrolTask -> Tick -> " + controller.gameObject.name);
    }

    public static IdleTask Create()
    {
        return ScriptableObject.CreateInstance<IdleTask>();
    }
}
