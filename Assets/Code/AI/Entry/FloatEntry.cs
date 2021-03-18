using System;
using UnityEngine;

public class FloatEntry : EntryVariable
{
    [SerializeField] private string entryName;
    public override string EntryName { get => entryName; set => entryName = value; }
    public override Type EntryType { get => GetType(); }

    [SerializeField] private float value;
    //public FloatEntry(string entryName, float value)
    //{
    //    this.entryName = entryName;
    //    this.value = value;
    //}

    public override bool IsSet()
    {
        return true;
    }

    public override object GetValue()
    {
        return value;
    }

    public static FloatEntry Create(string name, float value)
    {
        FloatEntry floatEntry = CreateInstance<FloatEntry>();
        floatEntry.name = name;
        floatEntry.EntryName = name;
        floatEntry.value = value;
        return floatEntry;
    }
}
