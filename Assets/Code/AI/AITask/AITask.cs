using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[Serializable] public enum AITaskStatus { STARTED, PERFORMING, FINISHED }
public abstract class AITask : ScriptableObject
{
    public abstract string TaskName { get; set; }
    public abstract AITaskStatus TaskStatus { get; set; }
    public abstract Timer TaskTimer { get; set; }
    public abstract bool LoopTask { get; set; }
    public abstract UnityEvent<AITask> OnTaskComplete { get; set; }
    public abstract void StartTask(AIController character);
    public abstract void UpdateTask(AIController character);

    protected virtual void OnDestroy()
    {
        if(OnTaskComplete != null)
        {
            OnTaskComplete.RemoveAllListeners();
        }
    }
}
