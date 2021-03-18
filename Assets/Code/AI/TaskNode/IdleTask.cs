using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game/AI/Task/Idle")]
public class IdleTask : TaskNode
{
    private AIController controller;

    [SerializeField] private AIBoard assignedBoard;
    public override AIBoard AssignedBoard { get => assignedBoard; set => assignedBoard = value; }

    [SerializeField] private string taskName;
    public override string TaskName { get => taskName; set => taskName = value; }// could be object.name?

    [SerializeField] private TaskStatus taskStatus;
    public override TaskStatus TaskStatus { get => taskStatus; set => taskStatus = value; }

    [SerializeField] private Timer taskTimer;
    public override Timer TaskTimer { get => taskTimer; set => taskTimer = value; }

    [SerializeField] private bool loopTask;
    public override bool LoopTask { get => loopTask; set => loopTask = value; }

    [SerializeField] private bool isComplete;
    public override bool IsComplete { get => isComplete; set => isComplete = value; }

    [SerializeField] private List<EntryVariable> variableList;
    public override List<EntryVariable> VariableList { get => variableList; set => variableList = value; }

    [SerializeField] private UnityEvent<TaskNode> onTaskComplete;
    public override UnityEvent<TaskNode> OnTaskComplete { get => onTaskComplete; set => onTaskComplete = value; }

    public override void StartTask(AIController character)
    {
        this.controller = character;
    }

    public override void UpdateTask(AIController controller)
    {
        Debug.Log("PatrolTask -> Tick -> " + controller.gameObject.name);
    }

    public override void CompleteTask(AIController character)
    {
        this.controller = character;
        onTaskComplete.Invoke(this);
    }

    public IEnumerator TaskCoroutine()
    {
        while (taskTimer.IsFinished == false)
        {
            yield return null;
        }
        Debug.Log(controller.name + "Done Waiting...");
    }
}
