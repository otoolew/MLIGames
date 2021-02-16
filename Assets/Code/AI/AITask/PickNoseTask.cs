using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PickNoseTask : AITask
{
    private AIController controller;

    [Range(0,1)]
    [SerializeField] private float chance;
    bool DieRoll { get { return Random.value < chance; } }

    bool wellPicked;

    public override string TaskName { get => "PickNoseTask"; }// could be object.name?
    public override void Init(MonoBehaviour runner)
    {
        chance = 0.5f;
        wellPicked = false;
        this.controller = (AIController)runner;
    }

    public override void DelegateTask()
    {
        controller.StartCoroutine(TaskCoroutine());
    }

    public IEnumerator TaskCoroutine()
    {
        while (!wellPicked)
        {
            Debug.Log(controller.gameObject.name + "'s picking nose...");
            yield return null; // wait for frame
            wellPicked = DieRoll;
            Debug.Log(controller.gameObject.name + " pick was success? " + wellPicked);
        }
        Debug.Log(controller.gameObject.name + "'s Nose Picking -> COMPLETE!");
    }
    public static PickNoseTask Create()
    {
        return CreateInstance<PickNoseTask>();
    }

    public override void Tick(AICharacter character)
    {
        throw new System.NotImplementedException();
    }
    //public override Func<bool> TaskFinished()
    //{
    //    throw new NotImplementedException();
    //}
}
