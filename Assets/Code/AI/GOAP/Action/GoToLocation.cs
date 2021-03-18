using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MLIGames.AI
{
    public class GoToLocation : GoalAction
    {
        [SerializeField] private Vector3 destination;
        public Vector3 Destination { get => destination; set => destination = value; }

        public override bool PreConditionsMet()
        {

            return true;
        }
        public override bool Perform()
        {
            if(NavAgent.hasPath && NavAgent.remainingDistance < 0.1f)
            {
                return true;
            }
            return false;
        }
        public override bool PostPerform()
        {
            return true;    
        }
    }
}