using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AITask : ScriptedDelegate
{
    public abstract IEnumerator TaskCoroutine();

    //public abstract AIController AIController { get; set; }
    //public abstract SequenceStatus Status { get; set; }

    //public abstract Func<bool> TaskFinished();
}
