using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointCircuit : MonoBehaviour
{
    [SerializeField] private List<Waypoint> waypointList;
    public List<Waypoint> WaypointList { get => waypointList; set => waypointList = value; }
}
