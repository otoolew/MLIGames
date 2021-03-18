using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MLIGames.AI
{
    public class Node
    {
        [SerializeField] private Node parent;
        public Node Parent { get => parent; set => parent = value; }

        [SerializeField] private float cost;
        public float Cost { get => cost; set => cost = value; }

        [SerializeField] private Dictionary<string, int> stateDictionary;
        public Dictionary<string, int> StateDictionary { get => stateDictionary; set => stateDictionary = value; }

        [SerializeField] private GoalAction goalAction;
        public GoalAction GoalAction { get => goalAction; set => goalAction = value; }

        public Node(Node parent, float cost, Dictionary<string, int> stateDictionary, GoalAction goalAction)
        {
            this.parent = parent;
            this.cost = cost;
            this.stateDictionary = stateDictionary;
            this.goalAction = goalAction;
        }
    }

    public class GoalPlanner
    {
        public Queue<GoalAction> Plan(List<GoalAction> actions, Dictionary<string,int> goal, WorldStates states)
        {
            List<GoalAction> usableActions = new List<GoalAction>();
            // Filter out unusable actions
            foreach (GoalAction item in actions)
            {
                if (item.IsAchievable())
                {
                    usableActions.Add(item);
                }
            }
            List<Node> leaves = new List<Node>();
            Node start = new Node(null, 0, GameMode.Instance.WorldStates.StateDictionary, null);

            bool success = BuildGraph(start, leaves, usableActions, goal);
            if (!success)
            {
                Debug.Log("NO PLAN");
                return null;
            }
            Node cheapest = null;
            foreach (Node leaf in leaves)
            {
                if(cheapest == null)
                {
                    cheapest = leaf;
                }
                else
                {
                    if(leaf.Cost < cheapest.Cost)
                    {
                        cheapest = leaf;
                    }
                }
            }
            List<GoalAction> result = new List<GoalAction>();
            Node n = cheapest;
            while(n != null)
            {
                if(n.GoalAction != null)
                {
                    result.Insert(0, n.GoalAction);
                }
                n = n.Parent;
            }
            Queue<GoalAction> queue = new Queue<GoalAction>();
            foreach (GoalAction item in result)
            {
                queue.Enqueue(item);
            }

            // Debugging
            PrintPlan(queue);

            return queue;
        }

        private bool BuildGraph(Node parent, List<Node> leaves, List<GoalAction> usableActions, Dictionary<string, int> goal)
        {
            bool foundPath = false;
            foreach (GoalAction action in usableActions)
            {
                if (action.IsAchievableGiven(parent.StateDictionary))
                {
                    Dictionary<string, int> currentState = new Dictionary<string, int>(parent.StateDictionary);
                    foreach (KeyValuePair<string,int> effects in action.EffectDictionary)
                    {
                        if (!currentState.ContainsKey(effects.Key))
                        {
                            currentState.Add(effects.Key, effects.Value);
                        }
                        Node node = new Node(parent, parent.Cost + action.Cost, currentState, action);
                        if(GoalAchieved(goal, currentState))
                        {
                            leaves.Add(node);
                            foundPath = true;
                        }
                        else
                        {
                            List<GoalAction> subset = ActionSubset(usableActions, action);
                            bool found = BuildGraph(node, leaves, subset, goal);
                            if (found)
                            {
                                foundPath = true;
                            }

                        }
                    }
                }
            }

            return foundPath;
        }

        private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> currentState)
        {
            foreach (KeyValuePair<string,int> item in goal)
            {
                if (!currentState.ContainsKey(item.Key))
                {
                    return false;
                }
            }
            return true;
        }

        private List<GoalAction> ActionSubset(List<GoalAction> actions, GoalAction removeAction)
        {
            List<GoalAction> subset = new List<GoalAction>();
            foreach (GoalAction item in actions)
            {
                if (!item.Equals(removeAction))
                {
                    subset.Add(item);
                }
            }
            return subset;
        }

        private void PrintPlan(Queue<GoalAction> queue)
        {
            foreach (GoalAction item in queue)
            {
                Debug.Log("Action: " + item.ActionName);
            }
        }
    }
}