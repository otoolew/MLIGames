using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourcePool<T> : ScriptableObject where T : IPoolable
{
    protected abstract Stack<T> PoolStack { get;}
    protected abstract T PoolablePrefab { get; set; }

    public T FetchFromPool()
    {
        if (PoolStack.Count > 0)
            return PoolStack.Pop();
        else
            return CreatePooledObject();
    }

    public void ReturnToPool(T pooledObject)
    {
        pooledObject.GameObject.gameObject.SetActive(false);
        PoolStack.Push(pooledObject);
    }

    public abstract T CreatePooledObject();
}
