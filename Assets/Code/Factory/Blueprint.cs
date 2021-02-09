using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Blueprint<T> : ScriptableObject where T : Blueprint<T>
{
    public static T CreateInstance(T arg)
    {
        var instance = ScriptableObject.CreateInstance<T>();
        instance.Init(arg);
        return instance;
    }
    public abstract void Init(T arg);
}
