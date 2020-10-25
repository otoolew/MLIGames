using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MouseUtility
{
    public static Vector3 MouseScreenToWorldPoint
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.nearClipPlane));
        }
    }
}
