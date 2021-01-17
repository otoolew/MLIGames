using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private SaveSlot saveSlot;
    public SaveSlot SaveSlot { get => saveSlot; set => saveSlot = value; }

    [SerializeField] private PlayerOptions playerOptions;
    public PlayerOptions PlayerOptions { get => playerOptions; set => playerOptions = value; }

    [SerializeField] private GameMode gameMode;
    public GameMode GameMode { get => gameMode; set => gameMode = value; }

}
