using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLIGames.AI
{
    [Serializable]
    public class SubGoal
    {
        [SerializeField] private Dictionary<string, int> goals;
        public Dictionary<string, int> Goals { get => goals; set => goals = value; }

        [SerializeField] private bool remove;
        public bool Remove { get => remove; set => remove = value; }

        public SubGoal(string name, int priority, bool remove)
        {
            goals = new Dictionary<string, int>();
            goals.Add(name, priority);
            this.remove = remove;
        }

    }
}