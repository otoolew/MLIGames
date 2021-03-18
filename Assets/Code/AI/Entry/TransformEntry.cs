using System;
using UnityEngine;

[Serializable]
public class TransformEntry : EntryVariable
{
    [SerializeField] private string entryName;
    public override string EntryName { get => entryName; set => entryName = value; }
    public override Type EntryType { get => GetType(); }

    [SerializeField] private Transform value;

    public TransformEntry(string entryName, Transform value)
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
