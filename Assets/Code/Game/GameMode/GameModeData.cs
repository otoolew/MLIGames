using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Game/Game Mode", fileName ="newGameMode")]
public class GameModeData : ScriptableObject
{
    #region Prefabs
    [Header("Prefabs")]
    [SerializeField] private PlayerCamera playerCameraPrefab;
    public PlayerCamera PlayerCameraPrefab { get => playerCameraPrefab; set => playerCameraPrefab = value; }

    [SerializeField] private PlayerController playerControllerPrefab;
    public PlayerController PlayerControllerPrefab { get => playerControllerPrefab; set => playerControllerPrefab = value; }

    [SerializeField] private PlayerUI playerUIPrefab;
    public PlayerUI PlayerUIPrefab { get => playerUIPrefab; set => playerUIPrefab = value; }

    [SerializeField] private PlayerCharacter playerCharacterPrefab;
    public PlayerCharacter PlayerCharacterPrefab { get => playerCharacterPrefab; set => playerCharacterPrefab = value; }

    [SerializeField] private Character enemyCharacterPrefab;
    public Character EnemyCharacterPrefab { get => enemyCharacterPrefab; set => enemyCharacterPrefab = value; }
    #endregion

    public PlayerController CreatePlayerController()
    {
        return playerControllerPrefab ? Instantiate(playerControllerPrefab, null) : null;
    }
    public PlayerUI CreatePlayerUI()
    {
        return playerUIPrefab ? Instantiate(playerUIPrefab, null) : null;
    }
    public PlayerCharacter CreatePlayerCharacter()
    {
        return playerCharacterPrefab ? Instantiate(playerCharacterPrefab, null) : null;
    }
}
