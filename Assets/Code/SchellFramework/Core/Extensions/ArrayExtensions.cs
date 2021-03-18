//------------------------------------------------------------------------------
// Copyright © 2013 Schell Games, LLC. All Rights Reserved.
//
// Contact: Jason Pratt
//
// Created: July 2013
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace SG.Core
{
    /// <summary>
    /// Extension methods for dealing with Arrays. Intended as a replacement for
    /// LINQ functions which would wind up allocating via boxing.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Determines whether the given index is within the bounds of this array.
        /// </summary>
        /// <param name="a">Extension</param>
        /// <param name="index">Index to check</param>
        /// <returns>true, valid index into the array; false, otherwise</returns>
        public static bool IsIndexInBounds(this Array a, int index)
        {
            return index >= 0 && index < a.Length;
        }

        public static bool HasAny(this Array array)
        {
            return array != null && array.Length > 0;
        }

        public static bool Has(this Array array, int index)
        {
            return array != null && index < array.Length && index >= 0;
        }

        public static bool Contains<T>(this T[] array, T value)
        {
            return Array.IndexOf(array, value) != -1;
        }

        public static int SafeLength(this Array array)
        {
            return array != null ? array.Length : 0;
        }

        public static int SafeLength(this IList list)
        {
            return list != null ? list.Count : 0;
        }

        public static T SafeGet<T>(this T[] array, int index)
        {
            if (array == null || index >= array.Length || index < 0)
                return default(T);
            return array[index];
        }

        public static T SafeGet<T>(this List<T> list, int index)
        {
            if (list == null || index >= list.Count || index < 0)
                return default(T);
            return list[index];
        }

        public static bool HasAny(this IList list)
        {
            return list != null && list.Count > 0;
        }

        public static List<T> Shuffle<T>(this List<T> deck)
        {
            int n = deck.Count;
            while (n > 1)
            {
                int k = Random.Range(0, n);
                n--;
                T value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
            return deck;
        }

        public static T[] Shuffle<T>(this T[] deck)
        {
            int n = deck.Length;
            while (n > 1)
            {
                int k = Random.Range(0, n);
                n--;
                T value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
            return deck;
        }
    }
}
