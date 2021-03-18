using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private Transform location;
    public Transform Location { get => location; set => location = value; }

    public void SpawnAt(GameObject go, Vector3 location)
    {
        go.transform.position = location;
    }
}
