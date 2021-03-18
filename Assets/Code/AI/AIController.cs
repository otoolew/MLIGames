
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MLIGames.AI;
using UnityEngine.Events;
using SG.Vignettitor;

public class AIController : MonoBehaviour
{
    [SerializeField] private float testFloat;

    //public FloatEntry FloatEntry;
    //public StringEntry StringEntry;
    //public VectorEntry VectorEntry;
    //public TransformEntry TransformEntry;

    #region Components
    [Header("Components")]
    [SerializeField] private AICharacter assignedCharacter;
    public AICharacter AssignedCharacter { get => assignedCharacter; set => assignedCharacter = value; }

    [SerializeField] private Animator animatorComp;
    public Animator AnimatorComp { get => animatorComp; set => animatorComp = value; }

    [Header("AI Board")]
    [SerializeField] private AIBoard aiBoard;
    public AIBoard AIBoard { get => aiBoard; set => aiBoard = value; }



    [Space]
    #region AI GOAP
    [Header("AI GOAP")]
    [SerializeField] private GoalPlanner goalPlanner;
    public GoalPlanner GoalPlanner { get => goalPlanner; set => goalPlanner = value; }

    [SerializeField] private List<GoalAction> actionList;
    public List<GoalAction> ActionList { get => actionList; set => actionList = value; }

    [SerializeField] private Dictionary<SubGoal, int> goals;
    public Dictionary<SubGoal, int> Goals { get => goals; set => goals = value; }

    [SerializeField] private Queue<GoalAction> actionQueue;
    public Queue<GoalAction> ActionQueue { get => actionQueue; set => actionQueue = value; }

    [SerializeField] private GoalAction currentAction;
    public GoalAction CurrentAction { get => currentAction; set => currentAction = value; }

    [SerializeField] private SubGoal currentGoal;
    public SubGoal CurrentGoal { get => currentGoal; set => currentGoal = value; }

    private bool actionInvoked = false;

    #endregion

    #endregion

    #region Values
    [Header("Values")]
    [SerializeField] private PlayerCharacter playerCharacter;
    public PlayerCharacter PlayerCharacter { get => playerCharacter; set => playerCharacter = value; }

    [SerializeField] private Vector3 playerLastKnownLocation;
    public Vector3 PlayerLastKnownLocation { get => playerLastKnownLocation; set => playerLastKnownLocation = value; }

    [SerializeField] private PatrolCircuit patrolCircuit;
    public PatrolCircuit PatrolCircuit { get => patrolCircuit; set => patrolCircuit = value; }

    [SerializeField] private int currentWaypointIndex;
    public int CurrentWaypointIndex { get => currentWaypointIndex; set => currentWaypointIndex = value; }

    [SerializeField] private PlayerSense playerSense;
    public PlayerSense PlayerSense { get => playerSense; set => playerSense = value; }

    [SerializeField] private VignettePlayerExecutor vignette;
    public VignettePlayerExecutor Vignette { get => vignette; set => vignette = value; }


    #endregion

    // Start is called before the first frame update
    void Start()
    {
        AIBoard = AIFactory.Init_AIBoard(AIBoard);
        goals = new Dictionary<SubGoal, int>();
        playerCharacter = FindObjectOfType<PlayerCharacter>();

        GoalAction[] actionComps = this.GetComponents<GoalAction>();
        foreach (GoalAction actionItem in actionComps)
        {
            actionList.Add(actionItem);
        }

        SubGoal s1 = new SubGoal("HasTarget", 1, true);
        goals.Add(s1, 3);
        //entryVariableList.Add(FloatEntry.Create("First Entry", 0.1f));

        //foreach (var item in variables)
        //{         
        //    if (item.GetType() == typeof(FloatEntry))
        //    {
        //        float value = (float)item.GetValue();
        //        Debug.Log(item.EntryName + " : " + value);
        //    }
        //}
        vignette.PlayDefault();
    }

    private void Update()
    {

    }

    private void LateUpdate()
    {
        if (currentAction != null && currentAction.IsRunning)
        {

            if (currentAction.NavAgent.hasPath && currentAction.NavAgent.remainingDistance < 0.1f)
            {
                if (!actionInvoked)
                {
                    Invoke(nameof(CompleteAction), currentAction.Duration);
                    actionInvoked = true;
                }
            }
            return;
        }

        if (goalPlanner == null || actionQueue == null)
        {
            goalPlanner = new GoalPlanner();
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;
            foreach (KeyValuePair<SubGoal, int> subgoal in sortedGoals)
            {
                actionQueue = goalPlanner.Plan(actionList, subgoal.Key.Goals, null);
                if (actionQueue != null)
                {
                    currentGoal = subgoal.Key;
                    break;
                }
            }
        }

        if(actionQueue != null && actionQueue.Count == 0)
        {
            if (currentGoal.Remove)
            {
                goals.Remove(currentGoal);
            }
            goalPlanner = null;
        }

        if(actionQueue != null && actionQueue.Count > 0)
        {
            currentAction = actionQueue.Dequeue();

            if (currentAction.PreConditionsMet())
            {
                currentAction.Perform();
                if (currentAction.Target == null && currentAction.TargetTag != "")
                {
                    currentAction.Target = GameObject.FindWithTag(currentAction.TargetTag);
                }

                if (currentAction.TargetTag != null)
                {
                    currentAction.IsRunning = true;
                    currentAction.NavAgent.SetDestination(currentAction.Target.transform.position);
                }
            }
            else
            {
                actionQueue = null;
            }
        }
    }

    #region AI
    private void AssignTask(TaskNode aitask)
    {
        aitask.OnTaskComplete.AddListener(OnTaskComplete);
    }
    private void CompleteAction()
    {
        currentAction.IsRunning = false;
        currentAction.PostPerform();
        actionInvoked = false;
    }
    private void OnTaskComplete(TaskNode aitask)
    {  
        try
        {
            //if (taskStack.Peek() == aitask)
            //{
            //    Debug.Log(aitask.TaskName + " Task is complete!");
            //}
        }
        catch (Exception)
        {
            throw;
        }      
    }
    #endregion
    #region Character Methods
    public bool PossessCharacter(AICharacter character)
    {
        assignedCharacter = character;
        return assignedCharacter;
    }

    public void SetBoolKey(string keyName, bool value)
    {
        animatorComp.SetBool(keyName, value);
    }

    public void SetIntegerKey(string keyName, int value)
    {
        animatorComp.SetInteger(keyName, value);
    }

    public void SetFloatKey(string keyName, int value)
    {
        animatorComp.SetFloat(keyName, value);
    }

    public void SetTriggerKey(string keyName, bool value)
    {
        animatorComp.SetTrigger(keyName);
    }

    public void NextPatrolPoint()
    {
        CurrentWaypointIndex++;
        if (CurrentWaypointIndex >= patrolCircuit.PatrolpointList.Count)
        {
            CurrentWaypointIndex = 0;
        }

        assignedCharacter.MovementComp.SetDestination(patrolCircuit.PatrolpointList[CurrentWaypointIndex].transform.position);
    }
    public void RandomLocationInRadius()
    {

        CurrentWaypointIndex++;
        if (CurrentWaypointIndex >= patrolCircuit.PatrolpointList.Count)
        {
            CurrentWaypointIndex = 0;
        }

        assignedCharacter.MovementComp.SetDestination(patrolCircuit.PatrolpointList[CurrentWaypointIndex].transform.position);
    }

    public void OnPerceptionUpdate(Character character)
    {
        if (character)
        {
            playerLastKnownLocation = character.transform.position;
            if (character.IsValid())
            {
                SetBoolKey("HasTarget", true);

                //taskStack.Push(ScriptableObject.CreateInstance<AttackTask>());
            }
        }    
    }

    public void OnPerceptionUpdate(Vector3 location)
    {
        playerLastKnownLocation = location;      
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
    void Reset()
    {
        //Output the message to the Console
        Debug.Log("Reset");
        if (!playerCharacter)
            playerCharacter = FindObjectOfType<PlayerCharacter>();
    }

    #endregion
}
