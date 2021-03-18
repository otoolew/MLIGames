using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAim : MonoBehaviour
{
    #region Components
    [SerializeField] private Character owner;
    public Character Owner { get => owner; set => owner = value; }
    #endregion
    [SerializeField] private Transform aimTransform;
    public Transform AimTransform { get => aimTransform; set => aimTransform = value; }

    [SerializeField] private Transform targetTransform;
    public Transform TargetTransform { get => targetTransform; set => targetTransform = value; }

    [SerializeField] private Vector3 defaultPosition;
    public Vector3 DefaultPosition { get => defaultPosition; set => defaultPosition = value; }

    [SerializeField] private bool hasTarget;
    public bool HasTarget { get => hasTarget; set => hasTarget = value; }

    public float DistanceToTarget { get { return Vector3.Distance(transform.position, aimTransform.position); } }

    // Start is called before the first frame update
    void Start()
    {
        ResetAim();
    }
    private void Update()
    {
        SyncAimTransformToTarget();
    }
    public void ResetAim()
    {
        targetTransform = null;
        aimTransform.position = owner.transform.position + defaultPosition;
    }

    private void SyncAimTransformToTarget()
    {
        if (targetTransform != null)
        {
            aimTransform.position = targetTransform.position;
        }
    }
    public void SetTarget(Transform target)
    {
        targetTransform = target;
    }

    #region Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
        Gizmos.DrawSphere(aimTransform.position, 0.1f);
    }
    #endregion
}
