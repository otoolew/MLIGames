using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    PoolComponent AssignedPool { get; set; }
    void Repool();
}
