using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// GameMMode Handles everything about a game. Spawn points, Enemy Count, Victory COnditions etc...
/// </summary>
public class GameMode : MonoBehaviour
{

    [SerializeField] private Transform playerSpawnPoint;
    public Transform PlayerSpawnPoint { get => playerSpawnPoint; set => playerSpawnPoint = value; }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        Debug.Log("Start Game!");
    }
}
