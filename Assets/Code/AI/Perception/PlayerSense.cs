using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSense : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    public PlayerCharacter PlayerCharacter { get => playerCharacter; set => playerCharacter = value; }

    [SerializeField] private Transform sightTransform;
    public Transform SightTransform { get => sightTransform; set => sightTransform = value; }

    [SerializeField] private float radius;
    public float Radius { get => radius; set => radius = value; }

    [SerializeField] private float viewAngle;
    public float ViewAngle { get => viewAngle; set => viewAngle = value; }

    [SerializeField] private Vector3 lastKnowPosition;
    public Vector3 LastKnowPosition { get => lastKnowPosition; set => lastKnowPosition = value; }

    [SerializeField] private LayerMask detectionLayer;
    public LayerMask DetectionLayer { get => detectionLayer; set => detectionLayer = value; }
    public bool InRange
    {
        get
        {
            if (playerCharacter != null)
            {
                float distance = Vector3.Distance(sightTransform.position, playerCharacter.WorldLocation);
                if (distance <= radius)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
    public bool HasLineOfSight
    {
        get
        {
            if (playerCharacter != null)
            {
                Ray ray = new Ray
                {
                    origin = sightTransform.position,
                    direction = playerCharacter.WorldLocation + new Vector3(0,1,0) - sightTransform.position,
                };

                if (Physics.Raycast(ray, out RaycastHit hit, radius + 10.0f, detectionLayer))
                {
                    PlayerCharacter player = hit.collider.GetComponent<PlayerCharacter>();
                    if (player != null)
                    {
                        lastKnowPosition = player.WorldLocation;
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }
    }

    private void Start()
    {
        playerCharacter = FindObjectOfType<PlayerCharacter>();
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public bool IsPlayerInSight()
    {
        if (playerCharacter != null && playerCharacter.IsValid())
        {
            if (InRange)
            {
                Vector3 directionToTarget = (playerCharacter.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
                {
                    return HasLineOfSight;
                }
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (IsPlayerInSight())
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(transform.position + new Vector3(0, 1, 0), lastKnowPosition + new Vector3(0, 1, 0));


            Gizmos.color = Color.green;
            Ray ray = new Ray
            {
                origin = sightTransform.position,
                direction = playerCharacter.WorldLocation + new Vector3(0, 1, 0) - sightTransform.position,
            };

            Gizmos.DrawRay(ray);
        }
    }
}
