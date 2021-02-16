using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[Serializable] public enum AITaskStatus { STARTED, PERFORMING, FINISHED }
public abstract class AIBehavior : MonoBehaviour
{
    public AITaskStatus TaskStatus { get; set; }
    public abstract string BehaviorName { get; set; }
    public abstract void Run();
    public abstract void Tick(AICharacter character);
    public abstract void Finish();
    public abstract UnityEvent<AITask> OnTaskComplete { get; set; }

}
