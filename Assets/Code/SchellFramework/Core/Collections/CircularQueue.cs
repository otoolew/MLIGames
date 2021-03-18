//-----------------------------------------------------------------------------
//  Copyright © 2012 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   05/06/2013
//-----------------------------------------------------------------------------

namespace SG.Core.Collections
{
    /// <summary>
    /// A circular buffer that functions like a queue. This provides a simple 
    /// interface to retrieve the least recently used element from the queue.
    /// 
    /// The initialization capacity of the queue, specified with an integer or 
    /// the length of the supplied list, should be kept as small as possible 
    /// since this queue intentionally keeps all elements in memory.
    /// 
    /// This collection uses an array as its backing structure.
    /// </summary>
    /// <typeparam name="T">
    /// The type of element that the circular queue will store and provide 
    /// access to.
    /// </typeparam>
    public class CircularQueue<T>
    {
        /// <summary>
        /// Creates a circular queue with a capacity of the given size. Each 
        /// element in the queue is initialized to its default value.
        /// </summary>
        /// <param name="size">
        /// The total capacity of the circular queue.
        /// </param>
        public CircularQueue(int size)
        {
            if (size <= 0)
                throw new System.ArgumentException("Initial size must greater than zero", "size");

            _elements = new T[size];
            for (int i = 0; i < size; i++)
                _elements[i] = default(T);
        }

        /// <summary>
        /// Create a circular queue where the elements are initialized to the 
        /// given values. The order of the elements in the input list will be 
        /// the order that the elements are accessed by the queue.
        /// </summary>
        /// <param name="list">
        /// The list to initialize the queue elements to.
        /// </param>
        public CircularQueue(T[] list)
        {
            if (list == null)
                throw new System.ArgumentNullException("list");

            if (list.Length == 0)
            {
                throw new System.ArgumentException(
                    "The initialization list must not be zero-length.", "list");
            }
            _elements = list;
        }

        /// <summary>How many elements are available in this Queue.</summary>
        public int Count
        {
            get { return _elements.Length; } 
        }

        /// <summary>Get the elment at the spcified index.</summary>
        /// <param name="index">Index to access.</param>
        /// <returns>Element at that index.</returns>
        public T Get(int index)
        {
            return _elements[index]; 
        }

        /// <summary>
        /// Gets the next element from the queue and advances the queue 
        /// pointer, making the returned element become the current head.
        /// </summary>
        /// <returns>The next available element for use.</returns>
        public T GetNextElement()
        {
            _index = (_index + 1) % _elements.Length;
            return _elements[_index];
        }

        /// <summary>
        /// Get the element currently at the head of the queue without making 
        /// any modifications.
        /// </summary>
        /// <returns>The queue's head element.</returns>
        public T GetHeadElement()
        {
            return _elements[_index];
        }

        /// <summary>
        /// The index pointing to the current element of the queue.
        /// </summary>
        private int _index;

        /// <summary>
        /// The elements of the circular queue that will be accessed 
        /// sequentially.
        /// </summary>
        private readonly T[] _elements;
    }
}