using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerControls inputActions;
    public PlayerControls InputActions { get => inputActions; set => inputActions = value; }

    [SerializeField] private PlayerCharacter playerCharacter;
    public PlayerCharacter PlayerCharacter { get => playerCharacter; set => playerCharacter = value; }

    [SerializeField] private PlayerHUD playerHUD;
    public PlayerHUD PlayerHUD { get => playerHUD; set => playerHUD = value; }

    #region Monobehaviour 
    private void Awake()
    {
        inputActions = new PlayerControls();
    }

    private void OnEnable()
    {
        inputActions.UI.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        PossessCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        if (PlayerCharacter)
            ReleaseCharacter();
        inputActions.UI.Disable();
    }

    private void OnDestroy()
    {

    }

    #endregion

    #region Character Methods
    public void PossessCharacter()
    {
        PlayerCharacter = FindObjectOfType<PlayerCharacter>();

        if(PlayerCharacter)
            PlayerCharacter.PossessCharacter(this);
    }
    public void ReleaseCharacter()
    {
        PlayerCharacter.ReleaseCharacter(this);
    }
    #endregion
}
