using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MenuPanel
{
    [SerializeField] private Button startGameButton;
    public Button StartGameButton { get => startGameButton; set => startGameButton = value; }

    [SerializeField] private Button optionsButton;
    public Button OptionsButton { get => optionsButton; set => optionsButton = value; }

    [SerializeField] private Button quitButton;
    public Button QuitButton { get => quitButton; set => quitButton = value; }

    // Start is called before the first frame update
    void Start()
    {
        startGameButton.onClick.AddListener(StartButton_Clicked);
        optionsButton.onClick.AddListener(OptionButton_Clicked);
        quitButton.onClick.AddListener(QuitButton_Clicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartButton_Clicked()
    {
        Debug.Log("Start Clicked");
        GameManager.Instance.LoadScene("MissionHub");
    }
    public void OptionButton_Clicked()
    {
        Debug.Log("Open Options");
    }
    public void QuitButton_Clicked()
    {
        Debug.Log("Quit Selected!");
        Application.Quit();
    }
    public override void Open()
    {
        
    }

    public override void Close()
    {
        
    }
}
