using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverLocation : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playercharacter;
    public PlayerCharacter PlayerCharacter { get => playercharacter; set => playercharacter = value; }

    [SerializeField] private Transform coverPosition;
    public Transform CoverPosition { get => coverPosition; set => coverPosition = value; }

    [SerializeField] private Transform[] attackPositions;
    public Transform[] AttackPositions { get => attackPositions; set => attackPositions = value; }

    [SerializeField] private Vector3 sightLineOffset;
    public Vector3 SightLineOffset { get => sightLineOffset; set => sightLineOffset = value; }

    [SerializeField] private LayerMask layerMask;
    public LayerMask LayerMask { get => layerMask; set => layerMask = value; }

    // Start is called before the first frame update
    void Start()
    {
        playercharacter = GetComponent<PlayerCharacter>();
    }

    public bool CoverHasLineOfSight()
    {
        return HasLineOfSight(coverPosition.position);
    }

    public bool FindAttackPosition(out Vector3 attackPosition )
    {
        attackPosition = Vector3.zero;
        for (int i = 0; i < attackPositions.Length; i++)
        {
            if (HasLineOfSight(attackPositions[i].position))
            {
                attackPosition = attackPositions[i].position;
                return true;
            }
        }
        return false;
    }

    private bool HasLineOfSight(Vector3 position)
    {
        if(Physics.Linecast(position + sightLineOffset, playercharacter.WorldLocation + sightLineOffset, out RaycastHit raycastHit, layerMask))       
            if (raycastHit.collider.GetComponent<PlayerCharacter>())          
                return true;               
        return false;
    }

}
