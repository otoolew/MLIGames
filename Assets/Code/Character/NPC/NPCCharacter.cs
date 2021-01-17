using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCharacter : Character
{

    [SerializeField] private NPCMovement movementComp;
    public override CharacterMovement MovementComp { get => movementComp as NPCMovement; set => movementComp = (NPCMovement)value; }

    [SerializeField] private HealthComponent healthComp;
    public override HealthComponent HealthComp { get => healthComp; set => healthComp = value; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
