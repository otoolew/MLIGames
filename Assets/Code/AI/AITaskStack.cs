using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Game/AI/Behavior")]
public class AITaskStack : StackCollection<TaskNode>
{
    [SerializeField] private AIController controller;
    public AIController Controller { get => controller; set => controller = value; }

    [SerializeField] private Stack<TaskNode> stateStack;
    public override Stack<TaskNode> StateStack => stateStack;

    [SerializeField] private List<TaskNode> stateList;
    public override List<TaskNode> StateList => stateList;

    private void Awake()
    {
        stateStack = new Stack<TaskNode>();
        stateList = new List<TaskNode>();
    }

    private void OnEnable()
    {
        stateStack = new Stack<TaskNode>();
        stateList = new List<TaskNode>();
    }
}
