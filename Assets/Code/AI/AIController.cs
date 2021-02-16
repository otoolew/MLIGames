using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIController : MonoBehaviour
{
    #region Components
    [SerializeField] private AICharacter assignedCharacter;
    public AICharacter AssignedCharacter { get => assignedCharacter; set => assignedCharacter = value; }

    [SerializeField] private Animator animatorComp;
    public Animator AnimatorComp { get => animatorComp; set => animatorComp = value; }
    #endregion

    #region Values
    [SerializeField] private PlayerCharacter playerCharacter;
    public PlayerCharacter PlayerCharacter { get => playerCharacter; set => playerCharacter = value; }

    [SerializeField] private Vector3 playerLastKnownLocation;
    public Vector3 PlayerLastKnownLocation { get => playerLastKnownLocation; set => playerLastKnownLocation = value; }

    [SerializeField] private WaypointCircuit waypointCircuit;
    public WaypointCircuit WaypointCircuit { get => waypointCircuit; set => waypointCircuit = value; }

    [SerializeField] private int currentWaypointIndex;
    public int CurrentWaypointIndex { get => currentWaypointIndex; set => currentWaypointIndex = value; }

    [SerializeField] private PlayerSense playerSense;
    public PlayerSense PlayerSense { get => playerSense; set => playerSense = value; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        playerCharacter = FindObjectOfType<PlayerCharacter>();
    }

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

    public void MoveToNextWaypoint()
    {
        currentWaypointIndex++;
        if (currentWaypointIndex >= waypointCircuit.WaypointList.Count)
        {
            currentWaypointIndex = 0;
        }

        assignedCharacter.MovementComp.SetDestination(waypointCircuit.WaypointList[currentWaypointIndex].transform.position);     
    }

    private void OnPerceptionUpdate(Character character)
    {
        if (character)
        {
            playerLastKnownLocation = character.transform.position;
            if (character.IsValid())
            {
                animatorComp.SetBool("HasTarget", true);
            }
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
