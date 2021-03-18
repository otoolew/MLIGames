using MLIGames.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVariable<T> : IUnityInterface
{
    string VariableName { get; set; }
    T Value { get; set; }
    void SetValue(T value);
}

/// <summary>
/// Concrete serializable version of interface above
/// </summary>
[Serializable]
public class Variable<T> : UnityInterface<IVariable<T>>
{
}

