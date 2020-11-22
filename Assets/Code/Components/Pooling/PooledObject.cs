using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    [SerializeField] private PoolComponent assignedPool;
    public PoolComponent AssignedPool { get => assignedPool; set => assignedPool = value; }

    protected virtual void Repool()
    {

        assignedPool.ReturnToPool(this);
    }
}
