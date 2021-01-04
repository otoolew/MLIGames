using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Quaternion MoveDirection { get { return transform.rotation; }}
}
