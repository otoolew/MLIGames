using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    #region Components
    [SerializeField] private AICharacter assignedCharacter;
    public AICharacter AssignedCharacter { get => assignedCharacter; set => assignedCharacter = value; }
    #endregion

    #region Values
    [SerializeField] private WaypointCircuit waypointCircuit;
    public WaypointCircuit WaypointCircuit { get => waypointCircuit; set => waypointCircuit = value; }

    [SerializeField] private int waypointIndex;
    public int WaypointIndex { get => waypointIndex; set => waypointIndex = value; }

    [SerializeField] private PlayerCharacter playerCharacter;
    public PlayerCharacter PlayerCharacter { get => playerCharacter; set => playerCharacter = value; }


    [SerializeField] private IdleTask idleTask;
    public IdleTask IdleTask { get => idleTask; set => idleTask = value; }

    [SerializeField] private ScriptedDelegateStack taskStack;
    public ScriptedDelegateStack TaskStack { get => taskStack; set => taskStack = value; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        playerCharacter = FindObjectOfType<PlayerCharacter>();
        idleTask = IdleTask.Create();
        taskStack = ScriptedDelegateStack.Create();
        taskStack.Clear();
        taskStack.Push(ScriptableObject.CreateInstance<IdleTask>());
        taskStack.Peek().Init(this);
        taskStack.Peek().DelegateTask();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Character Methods
    public bool PossessCharacter(AICharacter character)
    {
        assignedCharacter = character;
        return assignedCharacter;
    }

    public void AddAITask(AITask task)
    {
        if (task)
        {
            taskStack.Push(task);
            taskStack.Peek().Init(this);
            taskStack.Peek().DelegateTask();
        }
    }
    #endregion

    #region Editor
    /// <summary>
    /// On Validate is only called in Editor. By performing checks here was can rest assured they will not be null.
    /// Usually what is in the Components region is in here.
    /// </summary>
    protected virtual void OnValidate()
    {
        if (AssignedCharacter == null)
        {
            Debug.LogError("No Character assigned");
        }
    }
    #endregion
}
