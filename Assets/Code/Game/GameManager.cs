using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
[Serializable] public class SceneChangeComplete : UnityEvent { }

public class GameManager : Singleton<GameManager>
{
    #region Variables
    [SerializeField] private SaveSlot saveSlot;
    public SaveSlot SaveSlot { get => saveSlot; set => saveSlot = value; }

    [SerializeField] private PlayerOptions playerOptions;
    public PlayerOptions PlayerOptions { get => playerOptions; set => playerOptions = value; }

    [SerializeField] private GameMode gameMode;
    public GameMode GameMode { get => gameMode; set => gameMode = value; }
    #endregion

    #region Scene Values
    [Header("Scene Management")]
    //public SceneItem[] Scenes;
    [SerializeField] private CanvasGroup fadeScreen;
    public CanvasGroup FadeScreen { get => fadeScreen; set => fadeScreen = value; }

    [SerializeField] private float fadeDuration;
    public float FadeDuration { get => fadeDuration; set => fadeDuration = value; }

    [SerializeField] private string currentSceneName;
    public string CurrentSceneName { get => currentSceneName; set => currentSceneName = value; }

    [SerializeField] private bool isFading;
    public bool IsFading { get => isFading; set => isFading = value; }

    public SceneChangeComplete OnSceneChangeComplete;

    #endregion

    #region Monobehaviour
    private void Start()
    {
        Debug.Log("[GAME MANAGER] Start");
        //FadeScreen.gameObject.SetActive(false);
        OnSceneChangeComplete.AddListener(SceneChangeComplete);
    }
    #endregion

    #region Game Management
    public void AssignGameMode(GameMode gameMode)
    {
        GameMode = gameMode;
    }

    public void StartGameLevel()
    {
        if(gameMode == null)
        {
            gameMode = FindObjectOfType<GameMode>();
        }
        else
        {
            gameMode.StartGame();
        }
    }
    #endregion

    #region Scene Management

    private void SceneChangeComplete()
    {
        StartGameLevel();
    }
    public void LoadScene(string sceneName)
    {
        FadeAndLoadScene(sceneName);
    }

    private void FadeAndLoadScene(string sceneName)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName));
        }
    }

    private IEnumerator FadeAndSwitchScenes(string sceneName)
    {
        yield return StartCoroutine(Fade(1f));
        Debug.Log("[SCENE CHANGE] LoadSceneAsync");
        yield return SceneManager.LoadSceneAsync(sceneName);
        Debug.Log("[SCENE CHANGE] Begin Fade OUT");
        yield return StartCoroutine(Fade(0f));
        Debug.Log("[SCENE CHANGE] FINISH");
        OnSceneChangeComplete.Invoke();
        //SceneChangeComplete();
    }

    private IEnumerator Fade(float finalAlpha)
    {
        // TODO: SCREEN FADE COMPONENT
        isFading = true;
        FadeScreen.blocksRaycasts = true; // Blocks player Clicking on other Scene or UI GameObjects
        float fadeSpeed = Mathf.Abs(FadeScreen.alpha - finalAlpha) / fadeDuration;
        while (!Mathf.Approximately(FadeScreen.alpha, finalAlpha))
        {
            FadeScreen.alpha = Mathf.MoveTowards(FadeScreen.alpha, finalAlpha,
                fadeSpeed * Time.deltaTime);
            yield return null; //Lets the Coroutine finish
        }
        isFading = false;
        FadeScreen.blocksRaycasts = false;
    }
    #endregion
}
