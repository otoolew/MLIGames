using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MLIGames.AI
{
    public abstract class GoalAction : MonoBehaviour
    {
        [SerializeField] private string actionName;
        public string ActionName { get => actionName; set => actionName = value; }

        [SerializeField] private float cost;
        public float Cost { get => cost; set => cost = value; }

        [SerializeField] private GameObject target;
        public GameObject Target { get => target; set => target = value; }

        [SerializeField] private string targetTag;
        public string TargetTag { get => targetTag; set => targetTag = value; }

        [SerializeField] private float preActionDuration;
        public float PreActionDuration { get => preActionDuration; set => preActionDuration = value; }

        [SerializeField] private float postActionDuration;
        public float PostActionDuration { get => postActionDuration; set => postActionDuration = value; }

        [SerializeField] private float duration;
        public float Duration { get => duration; set => duration = value; }

        [SerializeField] private WorldState[] preconditions;
        public WorldState[] Preconditions { get => preconditions; set => preconditions = value; }

        [SerializeField] private WorldState[] afterEffects;
        public WorldState[] AfterEffects { get => afterEffects; set => afterEffects = value; }

        [SerializeField] private NavMeshAgent navAgent;
        public NavMeshAgent NavAgent { get => navAgent; set => navAgent = value; }

        [SerializeField] private WorldStates agentBeliefs;
        public WorldStates AgentBeliefs { get => agentBeliefs; set => agentBeliefs = value; }

        [SerializeField] private bool isRunning;
        public bool IsRunning { get => isRunning; set => isRunning = value; }

        public Dictionary<string, int> ConditionDictionary { get; set; }

        public Dictionary<string, int> EffectDictionary { get; set; }

        //public GoalAction()
        //{
        //    ConditionDictionary = new Dictionary<string, int>();
        //    EffectDictionary = new Dictionary<string, int>();
        //}

        private void Awake()
        {
            ConditionDictionary = new Dictionary<string, int>();
            EffectDictionary = new Dictionary<string, int>();
            navAgent = gameObject.GetComponent<NavMeshAgent>();
            if(preconditions != null)
            {
                foreach (WorldState item in preconditions)
                {
                    ConditionDictionary.Add(item.Key, item.Value);
                }
            }
            if (afterEffects != null)
            {
                foreach (WorldState item in afterEffects)
                {
                    EffectDictionary.Add(item.Key, item.Value);
                }
            }
        }

        public bool IsAchievable()
        {
            return true;
        }

        public bool IsAchievableGiven(Dictionary<string,int> conditions)
        {
            foreach (KeyValuePair<string,int> item in ConditionDictionary)
            {
                if (!ConditionDictionary.ContainsKey(item.Key))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// This can be refactored into a scriptable Object.
        /// </summary>
        /// <returns></returns>
        public abstract bool PreConditionsMet();
        public abstract bool Perform();
        public abstract bool PostPerform();
    }
}