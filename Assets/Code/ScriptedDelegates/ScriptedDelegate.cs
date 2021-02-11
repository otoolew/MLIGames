using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptedDelegate : ScriptableObject
{
    public abstract void Init(MonoBehaviour runner);
    public abstract void DelegateTask();
}
