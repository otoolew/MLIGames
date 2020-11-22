using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolComponent : MonoBehaviour
{
    [SerializeField] private PooledObject poolablePrefab;
    public PooledObject PoolablePrefab { get => poolablePrefab; set => poolablePrefab = value; }

    public Stack<PooledObject> PoolStack { get; protected set; }

    private void Awake()
    {
        PoolStack = new Stack<PooledObject>();
    }

    public void FetchFromPool()
    {
        Debug.Log("Pool Count:" + PoolStack.Count);
        if (PoolStack.Count > 0)
        {
            PooledObject pooledObject = PoolStack.Pop();
            pooledObject.transform.position = transform.position;
            pooledObject.gameObject.SetActive(true);
        }
        else
        {
            CreatePooledObject();
        }
    }

    public void ReturnToPool(PooledObject pooledObject)
    {
        Debug.Log("Return To Pool");

        pooledObject.gameObject.SetActive(false);
        PoolStack.Push(pooledObject);
    }

    private PooledObject CreatePooledObject()
    {
        PooledObject pooledObject = Instantiate(PoolablePrefab);
        pooledObject.transform.position = transform.position;
        pooledObject.AssignedPool = this;
        return pooledObject;
    }
}
