using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindLocationTask : AITask
{
    [SerializeField] private SequenceStatus status;
    public override SequenceStatus Status { get => status; set => status = value; }

    [SerializeField] private AIController controller;
    public override AIController AIController { get => controller; set => controller = value; }

    [SerializeField] private Vector3 location;
    public Vector3 Location { get => location; set => location = value; }

    [SerializeField] private float range;
    public float Range { get => range; set => range = value; }

    public override IEnumerator TaskCoroutine()
    {
        controller.AIMovement.Move(location);
        yield return new WaitUntil(TaskFinished());
        Debug.Log("Finished");
    }
    public override Func<bool> TaskFinished()
    {
        Vector3 characterLocation = controller.AssignedCharacter.WorldLocation;
        float distance = Vector3.Distance(characterLocation, location);
        if(distance <= range)
        {
            Status = SequenceStatus.COMPLETE;
            return ()=> true;
        }
        return () => false;
    }

    public static FindLocationTask CreateInstance(AIController controller, Vector3 location)
    {
        FindLocationTask findLocationTask = ScriptableObject.CreateInstance<FindLocationTask>();
        findLocationTask.AIController = controller;
        findLocationTask.Location = location;
        findLocationTask.Status = SequenceStatus.INPROGRESS;
        return findLocationTask;
    }


}
