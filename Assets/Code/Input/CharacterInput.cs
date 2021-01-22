using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInput : PlayerInput
{
    //[SerializeField] private CharacterInputActions inputActions;
    //public CharacterInputActions InputActions { get => inputActions; set => inputActions = value; }

    private void Awake()
    {
        //inputActions = new CharacterInputActions();
    }
    private void OnEnable()
    {
        //inputActions.Player.Enable();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDisable()
    {
        
    }
    private void OnDestroy()
    {
        
    }
}
