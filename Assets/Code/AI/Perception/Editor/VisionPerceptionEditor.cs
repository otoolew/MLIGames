using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(VisionPerception))]
public class VisionPerceptionEditor : Editor
{
    private void OnSceneGUI()
    {
        VisionPerception visionComp = (VisionPerception)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(visionComp.transform.position, Vector3.up, Vector3.forward, 360, visionComp.Radius);

        Vector3 viewAngleA = visionComp.DirectionFromAngle(-visionComp.ViewAngle / 2, false);
        Vector3 viewAngleB = visionComp.DirectionFromAngle(visionComp.ViewAngle / 2, false);
        Handles.DrawLine(visionComp.transform.position, visionComp.transform.position + viewAngleA * visionComp.Radius);
        Handles.DrawLine(visionComp.transform.position, visionComp.transform.position + viewAngleB * visionComp.Radius);

        Handles.color = Color.red;
        Vector3 viewAngleC = visionComp.DirectionFromAngle(-visionComp.FocusAngle / 2, false);
        Vector3 viewAngleD = visionComp.DirectionFromAngle(visionComp.FocusAngle / 2, false);
        Handles.DrawLine(visionComp.transform.position, visionComp.transform.position + viewAngleC * visionComp.Radius);
        Handles.DrawLine(visionComp.transform.position, visionComp.transform.position + viewAngleD * visionComp.Radius);

        Handles.color = Color.red;
        foreach (Character item in visionComp.VisableTargetList)
        {
            Handles.DrawLine(visionComp.transform.position, item.FocusPoint.position);
        }
    }
}
