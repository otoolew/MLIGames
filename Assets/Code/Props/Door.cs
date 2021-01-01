using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    #region Components
    [SerializeField] private Animator animatorComp;
    public Animator AnimatorComp { get => animatorComp; set => animatorComp = value; }
    #endregion

    #region Variables
    [SerializeField] private bool isOpen;
    public bool IsOpen { get => isOpen; set => isOpen = value; }

    [SerializeField] private bool isBlocked;
    public bool IsBlocked { get => isBlocked; set => isBlocked = value; }
    #endregion

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
        animatorComp.SetTrigger("Open");
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
            animatorComp.SetTrigger("Close");
            IsOpen = false;
            Debug.Log("Closing Door...");
        }
    }
}
