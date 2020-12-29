using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float doorSpeed;
    public float DoorSpeed { get => doorSpeed; set => doorSpeed = value; }

    [SerializeField] private bool isOpen;
    public bool IsOpen { get => isOpen; set => isOpen = value; }

    [SerializeField] private bool isBlocked;
    public bool IsBlocked { get => isBlocked; set => isBlocked = value; }

    public void DoorAction()
    {
        if (IsOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        IsOpen = true;
        Debug.Log("Openning Door...");
    }

    public void CloseDoor()
    {
        if (isBlocked)
        {
            Debug.Log("Door is blocked");          
        }
        else
        {
            IsOpen = false;
            Debug.Log("Closing Door...");
        }
    }
}
