using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlueprintFactory<T> : MonoBehaviour where T : Blueprint<T>
{
    public static T CreateInstance(T blueprint)
    {
        T instance = ScriptableObject.CreateInstance<T>();
        instance.Init(blueprint);
        return instance;
    }
    public abstract void Init(T blueprint);
}
