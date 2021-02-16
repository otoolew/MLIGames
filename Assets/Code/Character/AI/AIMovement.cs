using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : CharacterMovement
{
    #region Components
    [SerializeField] private Rigidbody rigidbodyComp;
    public Rigidbody RigidbodyComp { get => rigidbodyComp; set => rigidbodyComp = value; }

    [SerializeField] private NavMeshAgent navAgent;
    public NavMeshAgent NavAgent { get => navAgent; set => navAgent = value; }
    #endregion

    #region Values
    [SerializeField] private float accelerationSpeed;
    public float AccelerationSpeed { get => accelerationSpeed; set => accelerationSpeed = value; }

    [SerializeField] private float stoppingDistance;
    public float StoppingDistance { get => stoppingDistance; set => stoppingDistance = value; }
    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        if (navAgent)
        {
            navAgent.stoppingDistance = stoppingDistance;
            navAgent.speed = MoveSpeed;
            navAgent.angularSpeed = RotationSpeed;
            navAgent.acceleration = accelerationSpeed;

        }
    }

    // Update is called once per frame
    private void Update()
    {

    }

    public override void Move(Vector3 moveVector)
    {
        if (navAgent.isActiveAndEnabled)
        {
            navAgent.destination = moveVector;
        }
        else
        {
            Debug.Log("Path was stale... Calculating new one.");
            SetDestination(moveVector);
        }
    }

    public override void SetDestination(Vector3 moveVector)
    {
        if (navAgent.isActiveAndEnabled)
        {
            navAgent.destination = moveVector;
        }
    }

    public bool InRangeOfDestination(Vector3 location)
    {
        return Vector3.Distance(transform.position, location) < navAgent.stoppingDistance;
    }
}
