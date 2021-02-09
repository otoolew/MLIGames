using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private AICharacter assignedCharacter;
    public AICharacter AssignedCharacter { get => assignedCharacter; set => assignedCharacter = value; }

    [SerializeField] private AIMovement aiMovement;
    public AIMovement AIMovement { get => aiMovement; set => aiMovement = value; }

    [SerializeField] private VisionPerception visionPerception;
    public VisionPerception VisionPerception { get => visionPerception; set => visionPerception = value; }

    //[SerializeField] private PlayerSense playerSense;
    //public PlayerSense PlayerSense { get => playerSense; set => playerSense = value; }

    [SerializeField] private PatrolCircuit patrolCircuit;
    public PatrolCircuit PatrolCircuit { get => patrolCircuit; set => patrolCircuit = value; }

    [SerializeField] private int currentPatrolIndex;
    public int CurrentPatrolIndex { get => currentPatrolIndex; set => currentPatrolIndex = value; }

    [SerializeField] private TimedSequence timedSequence;
    public TimedSequence TimedSequence { get => timedSequence; set => timedSequence = value; }

    [SerializeField] private FindLocationTask findLocationTask;
    public FindLocationTask FindLocationTask { get => findLocationTask; set => findLocationTask = value; }

    // Start is called before the first frame update
    void Start()
    {
        currentPatrolIndex = 0;
        //MoveToLocation(playerSense.PlayerCharacter.WorldLocation);       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveToLocation(Vector3 location)
    {
        aiMovement.Move(location);
    }


    #region Character Methods
    public void PossessCharacter(AICharacter character)
    {
        if (character)
        {
            assignedCharacter = character;
        }
    }
    #endregion
   
}
