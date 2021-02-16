using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AITask : ScriptedDelegate
{
    public abstract string TaskName { get;}
    public bool SuccessfulInit { get;}
    public AITaskStatus TaskStatus { get; set; }
    public UnityAction<AITask> TaskCompleteAction { get; set; }
    public UnityEvent<AITask> OnTaskComplete { get; set; }
    public abstract void Tick(AICharacter character);
    //public abstract AITask Create();
    //public abstract IEnumerator TaskCoroutine();

    //public abstract AIController AIController { get; set; }
    //public abstract SequenceStatus Status { get; set; }

    //public abstract Func<bool> TaskFinished();
}
