using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : CharacterMovement
{
    #region Components
    [SerializeField] private Rigidbody rigidbodyComp;
    public override Rigidbody RigidbodyComp { get => rigidbodyComp; set => rigidbodyComp = value; }

    [SerializeField] private CharacterController characterController;
    public override CharacterController CharacterController { get => characterController; set => characterController = value; }

    [SerializeField] private NavMeshAgent navAgent;
    public NavMeshAgent NavAgent { get => navAgent; set => navAgent = value; }
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Move(Vector3 moveVector)
    {
        navAgent.SetDestination(moveVector);
    }
}
