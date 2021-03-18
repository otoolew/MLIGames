//-----------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Tim Sweeney
//  Date:   Oct 2014
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SG.Core.Collections
{
    /// <summary>
    /// Class to store and provide a common interface to a "recently used" 
    /// system for various editor purposes.
    /// 
    /// This uses a <see cref="List{T}"/>as its backing structure. For game
    /// code use <see cref="RecentsList{T}"/>
    /// </summary>
    /// <typeparam name="T">Item type contained in the recents list</typeparam>
    public class EditorRecents<T> : IEnumerable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorRecents{T}"/> class.
        /// If a list with the same key has been previously used (and serialized) it 
        /// will populate this instance of the list.
        /// </summary>
        /// <param name="max">Maximum items tracked</param>
        /// <param name="prefsKey">Player prefs key used to store the list</param>
        /// <param name="toStringKey">Delegate used to translate an item into a string key</param>
        /// <param name="fromStringKey">Delegate used to translate an item from a string key</param>
        public EditorRecents(int max, string prefsKey, Func<T, string> toStringKey, Func<string, T> fromStringKey)
        {
            _max = max;
            _prefsKey = prefsKey;
            _toStringKey = toStringKey;
            _fromStringKey = fromStringKey;
            DeserializeRecents();
        }

        /// <summary>
        /// Gets the maximum number elements that this recents list can contain.
        /// </summary>
        public int Max
        {
            get { return _max; }
        }

        /// <summary>
        /// Gets the key used to serialize the list data to <see cref="UnityEngine.PlayerPrefs"/>.
        /// </summary>
        public string PrefsKey
        {
            get { return _prefsKey; }
        }

        /// <summary>
        /// Gets the number of elements contained in this recents list.
        /// </summary>
        public int Count
        {
            get { return _recentValues.Count; } 
        }

        public int Length
        {
            get { return _recentValues.Count; }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        public T this[int index]
        {
            get { return _recentValues[index]; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _recentValues.GetEnumerator();
        }

        /// <summary>
        /// Sets the given element as the most recently used item.
        /// If the list is already full (over <see cref="Max"/>), the last
        /// item in the list will be removed.
        /// </summary>
        /// <param name="recent">Item to add</param>
        public void SetRecentlyUsed(T recent)
        {
            int curIndex = _recentValues.IndexOf(recent);
            if (curIndex == -1)
            {
                // If no room, remove the last from the list
                if (_recentValues.Count >= _max)
                    _recentValues.RemoveAt(_max - 1);
            }
            else
            {
                // Remove it so we can reinsert it at the front
                _recentValues.Remove(recent);
            }
            _recentValues.Insert(0, recent);
            SerializeRecents();
        }

        public void Add(T recent)
        {
            SetRecentlyUsed(recent);
        }

        public bool Contains(T value)
        {
            return _recentValues.Contains(value);
        }

        private void SerializeRecents()
        {
            string recentsString = _recentValues.Select(_toStringKey)
                                                .Aggregate((a, b) => a + ";" + b);

            PlayerPrefs.SetString(_prefsKey, recentsString);
        }

        private void DeserializeRecents()
        {
            string fromPrefs = PlayerPrefs.GetString(_prefsKey);
            if (!string.IsNullOrEmpty(fromPrefs))
            {
                _recentValues = fromPrefs.Split(';')
                                         .Select<string, T>(TryFromStringKey)
                                         .Where(x => x != null).ToList();

                if (_recentValues.Count > _max)
                {
                    _recentValues = _recentValues.GetRange(0, _max);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _recentValues.GetEnumerator();
        }

        private T TryFromStringKey(string key)
        {
            try
            {
                return _fromStringKey(key);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        private readonly int _max;
        private readonly string _prefsKey;
        private readonly Func<T, string> _toStringKey;
        private readonly Func<string, T> _fromStringKey;
        private List<T> _recentValues = new List<T>();
    }
}
