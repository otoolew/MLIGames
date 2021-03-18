using System;
using UnityEngine;

[Serializable]
public class VectorReference
{
    public bool UseConstant = true;
    public Vector3 ConstantValue;
    public VectorVariable Variable;

    public VectorReference()
    { }

    public VectorReference(Vector3 value)
    {
        UseConstant = true;
        ConstantValue = value;
    }

    public Vector3 Value
    {
        get { return UseConstant ? ConstantValue : Variable.Value; }
    }

    public static implicit operator Vector3(VectorReference reference)
    {
        return reference.Value;
    }
}
