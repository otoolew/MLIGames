using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PopupTextFeedback : MonoBehaviour
{
    [SerializeField] private Transform spawnLocation;
    public Transform SpawnLocation { get => spawnLocation; set => spawnLocation = value; }

    public void ActivatePopUpText(string value)
    {
        PopUpText popup = GameAssetManager.Instance.PopUpPool.FetchFromPool();
        popup.gameObject.SetActive(true);
        popup.ChangeText(value);
        popup.transform.position = SpawnLocation.position;
    }

    public void ActivatePopUpText(float value)
    {
        int val = (int)value;
        PopUpText popup = GameAssetManager.Instance.PopUpPool.FetchFromPool();
        popup.gameObject.SetActive(true);
        popup.ChangeText(val);
        popup.transform.position = SpawnLocation.position;
    }
}
