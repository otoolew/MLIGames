using MLIGames.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayerLocation : GoalAction
{
    [SerializeField] private Vector3 destination;
    public Vector3 Destination { get => destination; set => destination = value; }

    public override bool PreConditionsMet()
    {
        PlayerCharacter playerCharacter = FindObjectOfType<PlayerCharacter>();
        if(playerCharacter != null && playerCharacter.IsValid())
        {
            return true;
        }
        return false;
    }
    public override bool Perform()
    {
        if (NavAgent.hasPath && NavAgent.remainingDistance < 0.1f)
        {
            return true;
        }
        return false;
    }
    public override bool PostPerform()
    {
        GameMode.Instance.WorldStates.ModifyState("HasTarget", 1);
        return true;
    }
}
