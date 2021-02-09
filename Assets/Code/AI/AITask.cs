using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AITask : ScriptableObject
{
    public abstract AIController AIController { get; set; }
    public abstract SequenceStatus Status { get; set; }
    public abstract IEnumerator TaskCoroutine();
    public abstract Func<bool> TaskFinished();

}
