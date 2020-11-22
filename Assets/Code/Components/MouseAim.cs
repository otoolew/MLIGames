using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MouseAim : MonoBehaviour
{
    [SerializeField] private Transform aimTransform;
    public Transform AimTransform { get => aimTransform; set => aimTransform = value; }

    [SerializeField] private Vector3 crouchOriginPoint;
    public Vector3 CrouchOriginPoint { get => crouchOriginPoint; set => crouchOriginPoint = value; }

    [SerializeField] private LayerMask physicalHitLayer;
    public LayerMask PhysicalHitLayer { get => physicalHitLayer; set => physicalHitLayer = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AimTransformToPoint(Vector2 vector)
    {
        aimTransform.position = MouseToWorldPoint(vector);
    }

    public void AimTransformToPoint(Vector3 location)
    {
        aimTransform.localPosition = location;
    }

    private Vector3 MouseToWorldPoint(Vector2 mouseScreen)
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseScreen);
        if (Physics.Raycast(ray, out RaycastHit rayHit, 100.0f, PhysicalHitLayer))
        {
            return rayHit.point;
        }
        return transform.position;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(aimTransform.position, 0.1f);
    }
}
