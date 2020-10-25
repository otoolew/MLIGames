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
    public void AimTransformToPoint()
    {
        aimTransform.position = MouseToWorldPoint();
    }

    public void AimTransformToPoint(Vector3 location)
    {
        aimTransform.localPosition = location;
    }

    private Vector3 MouseToWorldPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit rayHit, 100.0f, PhysicalHitLayer))
        {
            return rayHit.point;
            //Vector3 hitPoint = rayHit.point;
            //Vector3 targetDir = hitPoint - transform.position;
            ////Vector3 newDir = Vector3.RotateTowards(equippedAttack.transform.forward, targetDir, 10f, 0.0f);
            //currentAttack.transform.rotation = Quaternion.LookRotation(targetDir);
        }
        return transform.position;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(aimTransform.position, 0.1f);
    }
}
