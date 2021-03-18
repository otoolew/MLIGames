using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolCircuit : MonoBehaviour
{
    [SerializeField] private List<PatrolPoint> patrolpointList;
    public List<PatrolPoint> PatrolpointList { get => patrolpointList; set => patrolpointList = value; }
}
