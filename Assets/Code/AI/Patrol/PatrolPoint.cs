using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    [SerializeField] private PatrolCircuit assignedPatrolCircuit;
    public PatrolCircuit AssignedPatrolCircuit { get => assignedPatrolCircuit; set => assignedPatrolCircuit = value; }

    [SerializeField] private int pointIndex;
    public int PointIndex { get => pointIndex; set => pointIndex = value; }

    [SerializeField] private PatrolPoint nextPoint;
    public PatrolPoint NextPoint { get => nextPoint; set => nextPoint = value; }

}
