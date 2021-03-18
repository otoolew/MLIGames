using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSequence : Sequence
{
    [SerializeField] private SequenceStatus status;
    public override SequenceStatus Status { get => status; set => status = value; }

    [SerializeField] private float sequenceTime;
    public float SequenceTime { get => sequenceTime; set => sequenceTime = value; }

    public override IEnumerator SequenceCoroutine(MonoBehaviour runner)
    {
        status = SequenceStatus.INPROGRESS;
        Debug.Log("TimedSequence : Status = " + Status.ToString());
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / sequenceTime;
            yield return null;
        }
        status = SequenceStatus.COMPLETE;
        Debug.Log("TimedSequence : Status = " + Status.ToString());
    }
}
