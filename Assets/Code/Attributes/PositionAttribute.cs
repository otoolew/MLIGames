using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class PositionAttribute : Attribute
{
    public readonly Vector2 position;
    public PositionAttribute(int x, int y)
    {
        position.x = x;
        position.y = y;
    }
}
