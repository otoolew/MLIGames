using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStack : StackCollection<Projectile>
{
    [SerializeField] private Stack<Projectile> stateStack;
    public override Stack<Projectile> StateStack => stateStack;

    [SerializeField] private List<Projectile> stateList;
    public override List<Projectile> StateList => stateList;

    private void OnEnable()
    {
        Debug.Log("Projectile Stack Enabled");
        stateStack = new Stack<Projectile>();
        stateList = new List<Projectile>();
    }

    public override void OnBeforeSerialize()
    {
        StateList.Clear();

        foreach (var kvp in StateStack)
        {
            StateList.Add(kvp);
        }
    }
    public override void OnAfterDeserialize()
    {
        stateStack = new Stack<Projectile>();

        for (int i = StateList.Count; i >= 0; i--)
            stateStack.Push(StateList[i]);
    }
}
