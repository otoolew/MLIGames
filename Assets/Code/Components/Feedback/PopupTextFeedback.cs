using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PopupTextFeedback : MonoBehaviour
{
    [SerializeField] private Vector3 spawnOffset;
    public Vector3 SpawnOffset { get => spawnOffset; set => spawnOffset = value; }

    public void ActivatePopUpText(string value)
    {
        PopUpText popup = GameAssetManager.Instance.PopUpPool.FetchFromPool();
        popup.gameObject.SetActive(true);
        popup.ChangeText(value);
        popup.transform.position = transform.position + spawnOffset;
    }

    public void ActivatePopUpText(float value)
    {
        int val = (int)value;
        PopUpText popup = GameAssetManager.Instance.PopUpPool.FetchFromPool();
        popup.gameObject.SetActive(true);
        popup.ChangeText(val);
        popup.transform.position = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawIcon(transform.position + spawnOffset, "PopUp Spawn");
    }
}