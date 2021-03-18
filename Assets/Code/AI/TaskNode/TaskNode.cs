using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[Serializable] public enum TaskStatus { RUNNING, SUCCESS, FAIL, ABORTED }

public abstract class TaskNode : ScriptableObject
{
    public abstract AIBoard AssignedBoard { get; set; }
    public abstract string TaskName { get; set; }
    public abstract TaskStatus TaskStatus { get; set; }
    public abstract Timer TaskTimer { get; set; }
    public abstract bool LoopTask { get; set; }
    public abstract bool IsComplete { get; set; }
    public abstract List<EntryVariable> VariableList { get; set; }
    public abstract UnityEvent<TaskNode> OnTaskComplete { get; set; }
    public abstract void StartTask(AIController character);
    public abstract void UpdateTask(AIController character);
    public abstract void CompleteTask(AIController character);

    protected virtual void OnDestroy()
    {
        if (OnTaskComplete != null)
        {
            OnTaskComplete.RemoveAllListeners();
        }
    }
}
