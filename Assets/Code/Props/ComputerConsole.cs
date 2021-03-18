using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerConsole : InteractionComponent
{
    public GameObject ConsolePanel;
    public Button SubmitButton;
    public Button CloseButton;

    protected override void Start()
    {
        base.Start();
        SubmitButton.onClick.AddListener(SumbitForm);
        CloseButton.onClick.AddListener(CloseConsole);
    }

    public void OpenConsole()
    {
        Debug.Log("ComputerConsole -> Open Console");
        GameManager.Instance.GameMode.PlayerController.DisableCharacterInput();
        ConsolePanel.gameObject.SetActive(true);
        CloseButton.Select();
    }
    public void CloseConsole()
    {
        Debug.Log("ComputerConsole -> Close Console");
        GameManager.Instance.GameMode.PlayerController.EnableCharacterInput();
        ConsolePanel.gameObject.SetActive(false);
    }

    public void SumbitForm()
    {
        Debug.Log("ComputerConsole -> Thank you for your cooperation...");
        CloseConsole();
    }
}
