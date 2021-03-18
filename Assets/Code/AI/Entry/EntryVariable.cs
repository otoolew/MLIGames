using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
//object[] values = { "word", true, 120, 136.34, 'a' };
//foreach (var value in values)
//    Console.WriteLine("{0} - type {1}", value,
//                      value.GetType().Name);
public abstract class EntryVariable: ScriptableObject, IEntryVariable
{
    public abstract string EntryName { get; set; }
    public abstract Type EntryType { get; }
    public abstract bool IsSet();
    public abstract object GetValue();
}
