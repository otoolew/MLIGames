using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerTaskNode : TaskNode
{
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

    [SerializeField] private float waitTime;
    public float WaitTime { get => waitTime; set => waitTime = value; }

    private void Awake()
    {
        isComplete = false;
    }

    public override void StartTask(AIController character)
    {
        isComplete = false;
        character.StartCoroutine(TaskCoroutine());
    }

    public override void UpdateTask(AIController character)
    {

    }
    public override void CompleteTask(AIController character)
    {

    }
    public IEnumerator TaskCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
        isComplete = true;
        taskStatus = TaskStatus.SUCCESS;
    }
}
