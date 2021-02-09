using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolCircuit : MonoBehaviour
{
    [SerializeField] private List<PatrolPoint> waypointList;
    public List<PatrolPoint> WaypointList { get => waypointList; set => waypointList = value; }
}
