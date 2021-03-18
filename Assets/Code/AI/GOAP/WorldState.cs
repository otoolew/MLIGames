using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLIGames.AI
{
    [Serializable]
    public class WorldState
    {
        [SerializeField] private string key;
        public string Key { get => key; set => key = value; }

        [SerializeField] private int value;
        public int Value { get => value; set => this.value = value; }
    }
}