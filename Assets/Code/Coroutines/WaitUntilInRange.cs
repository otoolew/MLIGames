using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitUntilInRange : CustomYieldInstruction
{
    private Transform origin;
    private Vector3 destination;
    private float range;
    public override bool keepWaiting
    {
        get
        {
            return !(Vector3.Distance(origin.position, destination) <= range);
        }
    }

    public WaitUntilInRange(Transform origin, Vector3 destination, float range)
    {
        this.origin = origin;
        this.destination = destination;
        this.range = range;
    }
}
