using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolComponent<T> : MonoBehaviour where T : IPoolable
{
    public static Stack<T> PoolStack { get; protected set; }
    public abstract T PoolablePrefab { get; set; }

    protected virtual void Awake()
    {
        PoolStack = new Stack<T>();
    }

    public T FetchFromPool()
    {
        if (PoolStack.Count > 0)      
            return PoolStack.Pop();  
        else
            return CreatePooledObject();
    }

    public void ReturnToPool(T pooledObject)
    {
        pooledObject.GameObject.gameObject.transform.SetParent(transform);
        pooledObject.GameObject.gameObject.SetActive(false);
        PoolStack.Push(pooledObject);
    }

    public abstract T CreatePooledObject();
}
