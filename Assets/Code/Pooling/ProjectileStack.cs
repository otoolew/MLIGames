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
    //public override void Init()
    //{
    //    stateStack = new Stack<Projectile>();
    //    stateList = new List<Projectile>();
    //}
    //public new static void Init()
    //{
    //    stateStack = new Stack<Projectile>();
    //    stateList = new List<Projectile>();
    //}

}
