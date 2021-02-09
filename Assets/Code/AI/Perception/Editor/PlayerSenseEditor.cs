using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(PlayerSense))]
public class PlayerSenseEditor : Editor
{
    private void OnSceneGUI()
    {
        PlayerSense visionComp = (PlayerSense)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(visionComp.transform.position, Vector3.up, Vector3.forward, 360, visionComp.Radius);

        Vector3 viewAngleA = visionComp.DirectionFromAngle(-visionComp.ViewAngle / 2, false);
        Vector3 viewAngleB = visionComp.DirectionFromAngle(visionComp.ViewAngle / 2, false);
        Handles.DrawLine(visionComp.transform.position, visionComp.transform.position + viewAngleA * visionComp.Radius);
        Handles.DrawLine(visionComp.transform.position, visionComp.transform.position + viewAngleB * visionComp.Radius);
    }
}
