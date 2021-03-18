//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   02/20/2014
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace SG.Core.Collections
{
    /// <summary>
    /// A data structure list of a fixed max length that recycles the least 
    /// recently used (in this case added) element when new data needs to be 
    /// added. This is particularly useful when tracking recent history. For 
    /// example, storing the last 20 states that the player character was in.
    /// 
    /// This uses an Array as its backing structure. For editor code
    /// code use <see cref="EditorRecents{T}"/>
    /// </summary>
    /// <typeparam name="T">
    /// The type of element to be contained in the list.
    /// </typeparam>
    public class RecentsList<T>
    {
        /// <summary>Create a new LRU List</summary>
        /// <param name="maxLength">
        /// How many elements to keep in the list.
        /// </param>
        public RecentsList(int maxLength)
        {
            if (maxLength <= 0)
                throw new ArgumentException("Max length must be greater than 0", "maxLength");

            MaxLength = maxLength;
            _list = new T[MaxLength];
        }

        /// <summary>
        /// Returns the oldest entry.
        /// </summary>
        public T Oldest()
        {
            return this[0];
        }

        /// <summary>
        /// Returns the most recently added entry.
        /// </summary>
        public T Newest()
        {
            return this[_usedHistory - 1];
        }

        /// <summary>
        /// Number of items held by the list.
        /// </summary>
        public int Length
        {
            get { return _usedHistory; }
        }

        /// <summary>
        /// Gets the maximum items this list will hold before removing the least
        /// recently added items.
        /// </summary>
        public int MaxLength
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets up index operator overrides for LRUList.
        /// </summary>
        /// <param name="key">List index</param>
        public T this[int key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        /// <summary>
        /// Returns true if this list is empty, false otherwise.
        /// </summary>
        public bool IsEmpty()
        {
            return _usedHistory == 0;
        }

        /// <summary>
        /// Add a new element to the list. If the total elements added is 
        /// greater than the maximum length, this will clear the 
        /// least-recently added element.
        /// </summary>
        /// <param name="item">Item to add to the list.</param>
        public void Add(T item)
        {
            if (_usedHistory < _list.Length)
                _usedHistory++;

            _list[_nextIndex] = item;
            _end = _nextIndex;
            _nextIndex = (_nextIndex + 1) % _list.Length;

            if (_usedHistory == _list.Length)
                _start = (_end + 1) % _list.Length;
        }

        /// <summary>
        /// Gets the item at the given index, where 0 is the least-recently 
        /// added item and <see cref="MaxLength"/> - 1 is the 
        /// most-recently added item.
        /// </summary>
        /// <param name="i">History index of the item to get.</param>
        /// <returns>Item at the given history index.</returns>
        public T Get(int i)
        {
            int index = (_start + i) % _list.Length;
            return _list[index];
        }

        /// <summary>
        /// Sets the item at the given index, where 0 is the least-recently
        /// added item and maxLength - 1 (passed in the constructor) is the
        /// most recently added item.
        /// </summary>
        /// <param name="i">List index to set</param>
        /// <param name="value">Value to set</param>
        public void Set(int i, T value)
        {
            int index = (_start + i) % _list.Length;
            _list[index] = value;
        }

        /// <summary>
        /// Clears all of the content of the list.
        /// </summary>
        public void Clear()
        {
            _usedHistory = 0;
            _nextIndex = 0;
            _start = 0;
            _end = 0;
            for (int i = 0; i < _list.Length; i++)
                _list[i] = default(T);
        }

        public bool Contains(T value)
        {
            for (int i = 0; i < _list.Length; i++)
                if (EqualityComparer<T>.Default.Equals(_list[i], value))
                    return true;
            return false;
        }

        /// <summary>How many of the total elements have been used.</summary>
        private int _usedHistory;

        /// <summary>The index that the next element will be added.</summary>
        private int _nextIndex;

        /// <summary>Index of the least-recently added element.</summary>
        private int _start;

        /// <summary>Index of the most recently added element.</summary>
        private int _end;

        /// <summary>The list of elements added.</summary>
        private T[] _list;
    }
}
