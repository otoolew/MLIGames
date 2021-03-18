//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: William Roberts
//  Date:   04/14/2015
//-----------------------------------------------------------------------------

namespace SG.Core.Lock
{
    /// <summary>
    /// This class is a trackable reference count that can be
    /// incremented or decremented. Used by <see cref="LockManager{T}"/>.
    /// </summary>
    public class ReferenceCount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceCount"/> class.
        /// </summary>
        public ReferenceCount()
        {
            _refCount = 0;
        }

        /// <summary>
        /// Gets the current reference count.
        /// </summary>
        public int Count
        {
            get { return _refCount; }
        }

        /// <summary>
        /// Increments the reference count.
        /// </summary>
        public void Increment()
        {
            _refCount++;
        }

        /// <summary>
        /// Decrements the reference count.
        /// </summary>
        /// <returns>Returns true whenever the reference count is 0</returns>
        public bool Decrement()
        {
            if(_refCount > 0)
                _refCount--;

            return (_refCount <= 0);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("RefCount={0}", _refCount);
        }

        private int _refCount;
    }
}
