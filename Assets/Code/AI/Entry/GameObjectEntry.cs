using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class GameObjectEntry : EntryVariable
{
    [SerializeField] private string entryName;
    public override string EntryName { get => entryName; set => entryName = value; }
    public override Type EntryType { get => GetType(); }

    [SerializeField] private Transform value;

    public GameObjectEntry(string entryName, Transform value)
    {
        this.entryName = entryName;
        this.value = value;
    }

    public override bool IsSet()
    {
        return true;
    }

    public override object GetValue()
    {
        return value;
    }
}
