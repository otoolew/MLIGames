using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private CharacterMovement movementComp;
    public CharacterMovement MovementComp { get => movementComp; set => movementComp = value; }

    [SerializeField] private MouseAim mouseAim;
    public MouseAim MouseAim { get => mouseAim; set => mouseAim = value; }

    [SerializeField] private AimIK aimIK;
    public AimIK AimIK { get => aimIK; set => aimIK = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        movementComp.MoveCharacter(new Vector3(Input.GetAxis("Horizontal"), 0.0f,  Input.GetAxis("Vertical")));

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            movementComp.Crouch();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            movementComp.UnCrouch();
        }
        if (Input.GetMouseButton(1))
        {
            mouseAim.AimTransformToPoint();
        }
        else
        {
            mouseAim.AimTransformToPoint(movementComp.ForwardRestPoint());
        }
    }
}
