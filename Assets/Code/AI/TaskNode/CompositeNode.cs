using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(menuName = "Game/AI/Composite Task")]
public class CompositeNode : TaskNode
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

    [SerializeField] private List<TaskNode> taskList;
    public List<TaskNode> TaskList { get => taskList; set => taskList = value; }

    public override void StartTask(AIController character)
    {
        Debug.Log(taskName + " Composite Node -> StartTask -> " + controller.gameObject.name);
        this.controller = character;
        character.StartCoroutine(TaskCoroutine());
    }

    public override void UpdateTask(AIController controller)
    {
        Debug.Log(taskName + " Composite Node -> UpdateTask -> " + controller.gameObject.name);
    }
    public override void CompleteTask(AIController character)
    {
        Debug.Log(taskName + " Composite Node -> AbortTask -> " + controller.gameObject.name);
    }

    public IEnumerator TaskCoroutine()
    {
        for (int i = 0; i < taskList.Count; i++)
        {
            Debug.Log(taskList[i].TaskName + " TaskCoroutine -> Begin " + controller.gameObject.name);
            taskList[i].UpdateTask(controller);
            yield return new WaitUntil(()=> taskList[i].TaskStatus == TaskStatus.SUCCESS);
            Debug.Log(taskList[i].TaskName + " TaskCoroutine -> Finished -> Get Next... " + controller.gameObject.name);
        }
    }
}
