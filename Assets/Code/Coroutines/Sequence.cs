using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]public enum SequenceStatus {INPROGRESS, COMPLETE}
public abstract class Sequence : ScriptableObject
{
    public abstract SequenceStatus Status { get; set; }
    public abstract IEnumerator SequenceCoroutine(MonoBehaviour runner);
}
