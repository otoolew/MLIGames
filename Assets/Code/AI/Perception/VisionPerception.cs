using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionPerception : MonoBehaviour
{
    #region Components
    [SerializeField] private SphereCollider visionCollider;
    public SphereCollider VisionCollider { get => visionCollider; set => visionCollider = value; }
    #endregion

    #region Values

    [SerializeField] private float radius;
    public float Radius { get => radius; set => radius = value; }

    [Range(20, 360)]
    [SerializeField] private float viewAngle;
    public float ViewAngle { get => viewAngle; set => viewAngle = value; }
    [Range(5, 180)]
    [SerializeField] private float focusAngle;
    public float FocusAngle { get => focusAngle; set => focusAngle = value; }

    [SerializeField] private LayerMask detectionLayer;
    public LayerMask DetectionLayer { get => detectionLayer; set => detectionLayer = value; }

    [SerializeField] private string[] ignoreTags;
    public string[] Ignoretags { get => ignoreTags; set => ignoreTags = value; }

    [SerializeField] private List<Character> detectedList;
    public List<Character> DetectedList { get => detectedList; set => detectedList = value; }

    [SerializeField] private List<Character> visableTargetList;
    public List<Character> VisableTargetList { get => visableTargetList; set => visableTargetList = value; }

    [SerializeField] private Character bestTarget;
    public Character BestTarget { get => bestTarget; set => bestTarget = value; }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        detectedList = new List<Character>();
        visableTargetList = new List<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshVisableTargets();
    }

    private void OnTriggerEnter(Collider other)
    {
        Character detectedCharacter = other.GetComponent<Character>();
        if (detectedCharacter != null && !IsTagIgnored(detectedCharacter.tag))
        {
            if (!detectedList.Contains(detectedCharacter))
            {
                detectedList.Add(detectedCharacter);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Character detectedCharacter = other.GetComponent<Character>();
        if (detectedCharacter != null)
        {
            if (detectedList.Contains(detectedCharacter))
            {
                detectedList.Remove(detectedCharacter);
            }
        }
    }
    public bool IsTagIgnored(string tagValue) 
    {
        Debug.Log("Tag: " + tagValue);
        for (int i = 0; i < ignoreTags.Length; i++)
        {
            if (tagValue.Equals(ignoreTags[i]))
            {
                return true;
            }
        }
        return false;
   
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void RefreshVisableTargets()
    {
        visableTargetList.Clear();
        float closestDistance = 1000;
        bestTarget = null;
        for (int i = 0; i < detectedList.Count; i++)
        {
            Vector3 directionToTarget = (detectedList[i].transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distance = Vector3.Distance(transform.position, detectedList[i].transform.position);
                Ray ray = new Ray
                {
                    origin = transform.position + new Vector3(0,1,0),
                    direction = directionToTarget,
                };

                if (Physics.Raycast(ray, out RaycastHit raycastHit, distance, detectionLayer))
                {  
                    Character detectedCharacter = raycastHit.collider.GetComponent<Character>();
                    if (detectedCharacter != null && !IsTagIgnored(detectedCharacter.tag))
                    {
                        if (!visableTargetList.Contains(detectedCharacter))
                        {
                            visableTargetList.Add(detectedCharacter);
                        }
                        if(distance < closestDistance)
                        {
                            bestTarget = detectedCharacter;
                        }
                    }
                }
            }
        }
        if(bestTarget)
            Debug.Log("Best Target: " + bestTarget.name);
    }
    private void OnValidate()
    {
        if (visionCollider)
        {
            visionCollider.radius = radius;
        }
    }
}
