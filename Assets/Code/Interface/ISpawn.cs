using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawn
{
    Transform SpawnLocation { get; set; }
    void Spawn();
    void Reset();
}
