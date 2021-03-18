using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SerializeField] public enum ControllerType { GAMEPAD, KEYBOARD_MOUSE}

[CreateAssetMenu(fileName ="newPlayerOptions", menuName ="Game/Player Options")]
public class PlayerOptions : ScriptableObject
{
    [SerializeField] private ControllerType controllerType;
    public ControllerType ControllerType { get => controllerType; set => controllerType = value; }

}
