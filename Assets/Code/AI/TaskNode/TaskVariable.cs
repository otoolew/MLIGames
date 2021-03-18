using UnityEngine;

public abstract class TaskVariable<T> : ScriptableObject
{
    public abstract T Value { get; set; }
    public abstract bool IsSet();
    public abstract void SetValue(T value);
    public abstract bool TryGetValue(out T output);
}
