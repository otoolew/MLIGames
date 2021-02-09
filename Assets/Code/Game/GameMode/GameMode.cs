using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[Serializable] public enum GameState { RUNNING, PAUSED, LOADING, GAME_OVER }
[Serializable] public class GameStateChange : UnityEvent<GameState, GameState> { }
/// <summary>
/// GameMode Handles everything about a game. Game Time, Spawn points, Enemy Count, Victory COnditions etc...
/// </summary>
public class GameMode : MonoBehaviour
{
    #region Variables
    [Header("Game State")]
    [SerializeField] private GameState currentGameState;
    public GameState CurrentGameState { get => currentGameState; set => currentGameState = value; }

    [Header("Player Components")]

    [SerializeField] private PlayerController playerController;
    public PlayerController PlayerController { get => playerController; set => playerController = value; }

    [SerializeField] private PlayerCamera playerCamera;
    public PlayerCamera PlayerCamera { get => playerCamera; set => playerCamera = value; }

    [SerializeField] private PlayerUI playerUI;
    public PlayerUI PlayerUI { get => playerUI; set => playerUI = value; }

    [SerializeField] private PlayerCharacter playerCharacter;
    public PlayerCharacter PlayerCharacter { get => playerCharacter; set => playerCharacter = value; }

    [SerializeField] private Transform playerSpawnPoint;
    public Transform PlayerSpawnPoint { get => playerSpawnPoint; set => playerSpawnPoint = value; }
    #endregion

    #region Prefabs
    [Header("Prefabs")]
    [SerializeField] private GameModeData gameModeData;
    public GameModeData GameModeData { get => gameModeData; set => gameModeData = value; }
    #endregion

    private void Awake()
    {
        CurrentGameState = GameState.LOADING;
        GameManager.Instance.AssignGameMode(this);
        InitGame();
    }
    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    #region Level Win Lose Condition

    #endregion

    #region Level Enemies

    #endregion

    #region Game Time Start and Finish
    public void InitGame()
    {
        //Debug.Log("Init Game!");
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            playerController = Instantiate(GameModeData.CreatePlayerController(), null);
            Debug.Log("Player Camera NULL\nCreated " + playerController.name);
        }
        // Player UI
        playerUI = FindObjectOfType<PlayerUI>();
        if (playerUI == null)
        {
            playerUI = Instantiate(GameModeData.CreatePlayerUI(), null);
            Debug.Log("Player UI NULL\nCreated " + playerUI.name);
        }
        playerUI.PauseMenuPanel.ResumeButton.onClick.AddListener(ResumeGame);
        playerUI.PauseMenuPanel.AbortMissionButton.onClick.AddListener(ResumeGame);
        playerUI.PauseMenuPanel.QuitButton.onClick.AddListener(ResumeGame);

        playerController.PlayerUI = playerUI;
        // Camera 
        playerCamera = FindObjectOfType<PlayerCamera>();
        if (playerCamera == null)
        {
            playerCamera = Instantiate(GameModeData.CreatePlayerCamera(), null);
            Debug.Log("Player Camera NULL\nCreated " + playerCamera.name);
        }
        playerController.PlayerCamera = playerCamera;
        // Character
        playerCharacter = FindObjectOfType<PlayerCharacter>();
        if (playerCharacter == null)
        {
            playerCharacter = Instantiate(GameModeData.CreatePlayerCharacter(), null);
        }

        playerController.PlayerCharacter = playerCharacter;
        PlayerController.PossessCharacter(playerCharacter);
        playerCharacter.transform.position = playerSpawnPoint.transform.position;

        StartGame();
    }

    public void StartGame()
    {
        Debug.Log("Start Game!");
        CurrentGameState = GameState.RUNNING;
    }

    public void PauseGame()
    {
        if (playerController)
            playerController.InputActions.Character.Disable();
        CurrentGameState = GameState.PAUSED;
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        if (playerController)
            playerController.InputActions.Character.Enable();
        CurrentGameState = GameState.RUNNING;
        Time.timeScale = 1.0f;
    }

    public void ResetGame()
    {
        Debug.Log("Reset");
        Time.timeScale = 1.0f;
        GameManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1.0f;
        GameManager.Instance.LoadScene("MainMenu");
    }
    #endregion

}
