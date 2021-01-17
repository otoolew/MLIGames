using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerConsole : MonoBehaviour
{
    [SerializeField] private GameObject consoleScreen;
    public GameObject ConsoleScreen { get => consoleScreen; set => consoleScreen = value; }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(gameObject.name + " OnTriggerEnter -> " + other.gameObject.name);
        PlayerCharacter playerCharacter = other.GetComponent<PlayerCharacter>();
        if (playerCharacter)
        {
            ConsoleScreen.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log(gameObject.name + " OnTriggerExit <- " + other.gameObject.name);
        PlayerCharacter playerCharacter = other.GetComponent<PlayerCharacter>();
        if (playerCharacter)
        {
            ConsoleScreen.SetActive(false);
        }
    }
}
