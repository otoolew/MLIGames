using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTask : AITask
{
    private AIController controller;
    public override string TaskName { get => "AttackTask"; }// could be object.name?

    [SerializeField] private float sequenceTime;
    public float SequenceTime { get => sequenceTime; set => sequenceTime = value; }

    [SerializeField] private AITask rootTask;
    public AITask RootTask { get => rootTask; set => rootTask = value; }

    [SerializeField] private AITaskStack taskStack;
    public AITaskStack TaskStack { get => taskStack; set => taskStack = value; }

    public static AttackTask Create()
    {
        return ScriptableObject.CreateInstance<AttackTask>();
    }
    public void Init(MonoBehaviour runner, int sequenceTime)
    {
        Init(runner);
        this.sequenceTime = sequenceTime;
        taskStack = AITaskStack.Create();
    }
    public override void Init(MonoBehaviour runner)
    {
        this.controller = runner.GetComponent<AIController>();
    }
    public override void Tick(AICharacter character)
    {

        Debug.Log("PatrolTask -> Tick -> " + controller.gameObject.name);
    }

    public override void DelegateTask()
    {
        Debug.Log("DelegateTask IdleRoutine -> " + controller.gameObject.name);
        controller.StartCoroutine(TaskCoroutine());
    }

    public IEnumerator TaskCoroutine()
    {
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            Debug.Log(controller.name + "Waiting...");
            normalizedTime += Time.deltaTime / sequenceTime;
            yield return null;


        }

        Debug.Log(controller.name + "Done Waiting...");
    }
}
