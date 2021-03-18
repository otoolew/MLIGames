using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataAsset : ScriptableObject
{
    public abstract string AssetName { get; set; }
    public abstract string ToJsonString();

}
