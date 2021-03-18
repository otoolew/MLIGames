using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MLIGames.AI
{
    [Serializable]
    public class WorldStates
    {
        [SerializeField] private Dictionary<string,int> stateDictionary;
        public Dictionary<string,int> StateDictionary { get => stateDictionary; set => stateDictionary = value; }

        [SerializeField] private List<string> keyList;
        [SerializeField] private List<int> valueList;

        public WorldStates()
        {
            stateDictionary = new Dictionary<string, int>();
        }

        public bool HasState(string key)
        {
            return stateDictionary.ContainsKey(key);
        }
        private void AddState(string key, int value)
        {
            stateDictionary.Add(key, value);
        }
        private void RemoveState(string key)
        {
            if (stateDictionary.ContainsKey(key))
            {
                stateDictionary.Remove(key);
            }

        }
        public void SetState(string key, int value)
        {
            if (stateDictionary.ContainsKey(key))
            {
                stateDictionary[key] = value;
            }
            else
            {
                stateDictionary.Add(key, value);
            }
        }
        public void ModifyState(string key, int value)
        {
            if (stateDictionary.ContainsKey(key))
            {
                stateDictionary[key] += value;
                if(stateDictionary[key] <= 0)
                {

                }
            }
            else
            {
                stateDictionary.Add(key, value);
            }
        }
        public Dictionary<string,int> GetStates()
        {
            return stateDictionary;
        }

        //public void OnBeforeSerialize()
        //{
        //    keyList.Clear();
        //    valueList.Clear();

        //    foreach (KeyValuePair<string,int> kvp in StateDictionary)
        //    {
        //        keyList.Add(kvp.Key);
        //        valueList.Add(kvp.Value);
        //    }
        //}

        //public void OnAfterDeserialize()
        //{
        //    stateDictionary = new Dictionary<string, int>();

        //    for (int i = 0; i != Math.Min(keyList.Count, valueList.Count); i++)
        //        stateDictionary.Add(keyList[i], valueList[i]);
        //}

        //void OnGUI()
        //{
        //    GUILayoutOption[] layoutOption = new GUILayoutOption[]
        //    {
        //        GUILayout.Height(48),
        //    };
        //    foreach (var kvp in stateDictionary)
        //        GUILayout.Label("Key: " + kvp.Key + " Value: " + kvp.Value, new GUILayoutOption[] { });
        //}
    }
}