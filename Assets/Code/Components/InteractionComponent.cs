using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class InteractionComponent : MonoBehaviour
{
    public GameObject InteractionIcon;

    public UnityEvent onInteraction;
    public UnityEvent onLeaveConsole;
    #region Monobehaviour
    protected virtual void Start()
    {
        InteractionIcon.SetActive(false);
        if (onInteraction == null)
        {
            onInteraction = new UnityEvent();
        }

        if (onLeaveConsole == null)
        {
            onLeaveConsole = new UnityEvent();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {

        //Debug.Log(gameObject.name + " OnTriggerEnter -> " + other.gameObject.name);
        PlayerCharacter playerCharacter = other.GetComponent<PlayerCharacter>();
        if (playerCharacter)
        {
            playerCharacter.onUseInteractable.AddListener(UseInteraction);
            DisplayInteractionIcon(true);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        //Debug.Log(gameObject.name + " OnTriggerExit <- " + other.gameObject.name);
        PlayerCharacter playerCharacter = other.GetComponent<PlayerCharacter>();
        if (playerCharacter)
        {
            playerCharacter.onUseInteractable.RemoveListener(UseInteraction);
            DisplayInteractionIcon(false);
        }
    }

    protected virtual void OnDestroy()
    {
        onInteraction.RemoveAllListeners();
    }

    #endregion
    protected void DisplayInteractionIcon(bool displayValue)
    {
        InteractionIcon.SetActive(displayValue);
    }

    protected void UseInteraction()
    {
        onInteraction.Invoke();
    }
}
