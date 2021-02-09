using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Input

    //[SerializeField] private PlayerInput playerInput;
    //public PlayerInput PlayerInput { get => playerInput; set => playerInput = value; }

    [SerializeField] private PlayerControls inputActions;
    public PlayerControls InputActions { get => inputActions; set => inputActions = value; }
    #endregion

    #region Components
    [SerializeField] private PlayerCamera playerCamera;
    public PlayerCamera PlayerCamera { get => playerCamera; set => playerCamera = value; }

    [SerializeField] private PlayerUI playerUI;
    public PlayerUI PlayerUI { get => playerUI; set => playerUI = value; }
    #endregion

    [SerializeField] private bool gamepadEnabled;
    public bool GamepadEnabled { get => gamepadEnabled; set => gamepadEnabled = value; }

    [SerializeField] private PlayerCharacter playerCharacter;
    public PlayerCharacter PlayerCharacter { get => playerCharacter; set => playerCharacter = value; }

    #region Monobehaviour 
    private void Awake()
    {   
        inputActions = new PlayerControls();
    }

    private void OnEnable()
    {
        //Debug.Log(playerInput.actionEvents.ToArray().ToString());
        inputActions.UI.Enable();
        inputActions.UI.Pause.performed += OnPause;
        inputActions.Character.Enable();

        // Get Control Schemes
        InputControlScheme[] schemeList = inputActions.controlSchemes.ToArray();
        for (int i = 0; i < schemeList.Length; i++)
        {
            Debug.Log("Scheme BindingGroup: " + schemeList[i].bindingGroup);
        }

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
        ReleaseCharacter();
        inputActions.UI.Pause.performed -= OnPause;
        inputActions.UI.Disable();
        inputActions.Character.Disable();
 
    }

    private void OnDestroy()
    {
        if (playerCharacter)
        {
            ReleaseCharacter();
        }
    }

    #endregion
    #region PlayerInput Calls
    private void OnPause(InputAction.CallbackContext callbackContext)
    {
        if (playerUI)
        {
            if (callbackContext.performed)
            {
                if (GameManager.Instance.GameMode.CurrentGameState != GameState.PAUSED)
                {
                    PlayerUI.OpenPauseMenu();
                }
                else
                {
                    PlayerUI.ClosePauseMenu();
                }
            }
        }
    }

    #endregion
    #region Character Methods
    public void PossessCharacter(PlayerCharacter character)
    {
        if (character)
        {
            // CharacterInput
            character.PlayerController = this;
            playerCharacter = character;

            character.GetComponent<PlayerMovement>().InputActions = inputActions;
            character.GetComponent<PlayerRotation>().InputActions = inputActions;
            character.GetComponent<PlayerRotation>().GamepadEnabled = gamepadEnabled;

            #region Character Input

            inputActions.Character.Look.started += playerCharacter.CharacterRotation.OnLook;
            inputActions.Character.Look.performed += playerCharacter.CharacterRotation.OnLook;
            inputActions.Character.Look.canceled += playerCharacter.CharacterRotation.OnLook;


            inputActions.Character.Left_PullTrigger.started += playerCharacter.OnLeftPullTrigger;
            inputActions.Character.Right_PullTrigger.performed += playerCharacter.OnRightPullTrigger;
            inputActions.Character.Left_PullTrigger.canceled += playerCharacter.OnLeftPullTrigger;
            inputActions.Character.Left_Reload.started += playerCharacter.OnLeftReload;

            inputActions.Character.Right_PullTrigger.started += playerCharacter.OnRightPullTrigger;
            inputActions.Character.Right_PullTrigger.performed += playerCharacter.OnRightPullTrigger;
            inputActions.Character.Right_PullTrigger.canceled += playerCharacter.OnRightPullTrigger;
            inputActions.Character.Right_Reload.started += playerCharacter.OnRightReload;

            inputActions.Character.UseInteraction.started += playerCharacter.OnUseInteraction;

            inputActions.Character.Dash.started += playerCharacter.OnDashPullTrigger;
            inputActions.Character.Dash.canceled += playerCharacter.OnDashPullTrigger;

            inputActions.Character.Enable();
            #endregion
            playerCharacter.SetUpPlayerUI(PlayerUI);

            PlayerCamera.AssignFollowTarget(playerCharacter.transform);
        }
    }
    public void ReleaseCharacter()
    {
        if (playerCharacter)
        {
            inputActions.Character.Look.started -= playerCharacter.CharacterRotation.OnLook;
            inputActions.Character.Look.performed -= playerCharacter.CharacterRotation.OnLook;
            inputActions.Character.Look.canceled -= playerCharacter.CharacterRotation.OnLook;

            inputActions.Character.Left_PullTrigger.started -= playerCharacter.OnLeftPullTrigger;
            inputActions.Character.Left_PullTrigger.canceled -= playerCharacter.OnLeftPullTrigger;
            inputActions.Character.Left_Reload.started -= playerCharacter.OnLeftReload;

            inputActions.Character.Right_PullTrigger.started -= playerCharacter.OnRightPullTrigger;
            inputActions.Character.Right_PullTrigger.canceled -= playerCharacter.OnRightPullTrigger;
            inputActions.Character.Right_Reload.started -= playerCharacter.OnRightReload;

            inputActions.Character.UseInteraction.started -= playerCharacter.OnUseInteraction;

            inputActions.Character.Dash.started -= playerCharacter.OnDashPullTrigger;
            inputActions.Character.Dash.canceled -= playerCharacter.OnDashPullTrigger;

            inputActions.Character.Disable();
        }
        playerCharacter = null;
    }

    public void EnableCharacterInput()
    {
        inputActions.Character.Enable();
    }
    public void DisableCharacterInput()
    {
        inputActions.Character.Disable();
    }
    #endregion
}
