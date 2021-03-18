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

    [SerializeField] private float straifeDistance;
    public float StraifeDistance { get => straifeDistance; set => straifeDistance = value; }

    public Vector3 StrafeLeftPosition { get => transform.right * -straifeDistance; }
    public Vector3 StrafeRightPosition { get => transform.right * straifeDistance; }
    public Vector3 CurrentDestination { get => NavAgent.destination; set => NavAgent.destination = value; }


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

    public void ContinueMovement()
    {
        if (navAgent != null)
        {
            navAgent.isStopped = false;
        }
    }

    public void HaltMovement()
    {
        if (navAgent != null)
        {
            navAgent.isStopped = true;
        }
    }

    public void HardStop()
    {
        SetDestination(transform.position);
    }

    public override void SetDestination(Vector3 moveVector)
    {
        if (navAgent.isActiveAndEnabled)
        {
            CurrentDestination = moveVector;
        }
    }

    public bool InRangeOfDestination()
    {
        return Vector3.Distance(transform.position, CurrentDestination) <= navAgent.stoppingDistance;
    }

    #region Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, StrafeLeftPosition);
        Gizmos.DrawRay(transform.position, StrafeRightPosition);
        Gizmos.DrawCube(CurrentDestination + new Vector3(0,0.5f,0), new Vector3(1.0f, 0.5f, 1.0f));
    }
    #endregion
}
