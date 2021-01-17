using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class InteractionComponent : MonoBehaviour
{
    public UnityEvent onInteraction;

    #region Monobehaviour
    private void Start()
    {
        if(onInteraction == null)
        {
            onInteraction = new UnityEvent();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(gameObject.name + " OnTriggerEnter -> " + other.gameObject.name);
        PlayerCharacter playerCharacter = other.GetComponent<PlayerCharacter>();
        if (playerCharacter)
        {
            playerCharacter.onUseInteractable.AddListener(UseInteraction);
            Debug.Log(gameObject.name + " OnTriggerEnter -> " + other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log(gameObject.name + " OnTriggerExit <- " + other.gameObject.name);
        PlayerCharacter playerCharacter = other.GetComponent<PlayerCharacter>();
        if (playerCharacter)
        {
            playerCharacter.onUseInteractable.RemoveListener(UseInteraction);
        }
    }

    private void OnDestroy()
    {
        onInteraction.RemoveAllListeners();
    }

    #endregion
    private void UseInteraction()
    {
        onInteraction.Invoke();
    }
}
