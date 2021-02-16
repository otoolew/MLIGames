using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITaskStack : StackCollection<AITask>
{
    [SerializeField] private Stack<AITask> stateStack;
    public override Stack<AITask> StateStack => stateStack;

    [SerializeField] private List<AITask> stateList;
    public override List<AITask> StateList => stateList;
    public static AITaskStack Create()
    {
        return ScriptableObject.CreateInstance<AITaskStack>();
    }

    private void Awake()
    {
        stateStack = new Stack<AITask>();
        stateList = new List<AITask>();
    }

    private void OnEnable()
    {
        stateStack = new Stack<AITask>();
        stateList = new List<AITask>();
    }
}
