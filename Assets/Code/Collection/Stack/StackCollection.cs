using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StackCollection<T> : ScriptableObject, ISerializationCallbackReceiver
{
    public abstract Stack<T> StateStack { get; }
    public abstract List<T> StateList { get; }

    #region Stack Operations
    public int Count { get { if(StateStack != null){ return StateStack.Count; } return 0; } }
    public T Peek()
    {
        return StateStack.Peek();
    }
    public T Pop()
    {
        return StateStack.Pop();
    }
    public void Push(T item)
    {
        StateStack.Push(item);
    }
    public T[] ToArray()
    {
        return StateStack.ToArray();
    }
    public void Clear()
    {
        StateStack.Clear();
    }
    public bool Contains(T item)
    {
        return StateStack.Contains(item);
    }
    public Stack<T>.Enumerator GetEnumerator()
    {
        return StateStack.GetEnumerator();
    }
    public void TrimExcess()
    {
        StateStack.TrimExcess();
    }
    //public void CopyTo(T[] array, int arrayIndex);
    #endregion

    #region Inspector
    public abstract void OnBeforeSerialize();
    public abstract void OnAfterDeserialize();
    #endregion
}
