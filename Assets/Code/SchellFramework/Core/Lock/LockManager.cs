//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: William Roberts, Eric Policaro
//  Date:   04/14/2015
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace SG.Core.Lock
{
    /// <summary>
    /// A locking manager that provides basic support for semaphors and mutexes for general
    /// parallel processing in the context of Unity. This object was not created 
    /// with threading in mind. Therefore, it is not thread safe and should not be used for
    /// locking resources across threads. 
    /// </summary>
    /// <typeparam name="T">
    /// The object type that will act as an semaphor or mutex.
    /// </typeparam>
    public abstract class LockManager<T> 
    {
        /// <summary>
        /// Determines if there is currently a lock of not.
        /// </summary>
        public bool IsLocked
        {
            get
            {
                return _lockRefCount.Count > 0;
            }
        }

        /// <summary>
        /// Pushes a lock for the specified object onto the stack. Each PushLock must be paired with an
        /// PopLock in order for the resource to be released.
        /// </summary>
        /// <param name="token">
        /// The object to lock on.
        /// </param>
        public void PushLock(T token)
        {
            if (token == null)
                throw new System.ArgumentNullException("token", "Must Not be an null value!");

            ReferenceCount refCount;
            if (!_lockRefCount.TryGetValue(token, out refCount))
            {
                refCount = new ReferenceCount();
                _lockRefCount.Add(token, refCount);
            }

            refCount.Increment();

            if (_lockRefCount.Count == 1 && refCount.Count == 1)
                OnLockStarted(new LockStartedArguments(token));
        }


        /// <summary>
        /// Releases the lock for the specified object. The OnLockReleased method
        /// will be invoked whenever no objects currently hold the lock.
        /// </summary>
        /// <param name="token">
        /// The object to release the lock for.
        /// </param>
        public void PopLock(T token)
        {
            if (token == null)
                throw new System.ArgumentNullException("token", "Must not be an null value!");

            ReferenceCount refCount;
            if (_lockRefCount.TryGetValue(token, out refCount))
            {
                if (refCount.Decrement())
                    _lockRefCount.Remove(token);

                // Allow the operation to resume once there is nothing currently holding the lock.
                if (_lockRefCount.Count == 0)
                    OnLockReleased(new LockReleasedArguments(token));
            }
            else
            {
                Log.Error(
                    token as UnityEngine.Object, 
                    "'{0}' attempted to pop the input lock without pushing first!",
                    token
                );
            }
        }

        /// <summary>
        /// Logger.
        /// </summary>
        protected static readonly Notify Log = NotifyManager.GetInstance("Core");

        /// <summary>
        /// Called when a token is locked for the first time.
        /// </summary>
        /// <param name="args">Data for the lock.</param>
        protected abstract void OnLockStarted(LockStartedArguments args);

        /// <summary>
        /// Called when a token has no more references and is released.
        /// </summary>
        /// <param name="args">Data for the lock.</param>
        protected abstract void OnLockReleased(LockReleasedArguments args);

        private readonly Dictionary<T, ReferenceCount> _lockRefCount = new Dictionary<T, ReferenceCount>();

        /// <summary>
        /// Lock arguments for the lock started callback.
        /// </summary>
        public class LockStartedArguments
        {
            public T Token { get; private set; }

            public LockStartedArguments(T token)
            {
                Token = token;
            }
        }

        /// <summary>
        /// Lock arguments for the lock released callback.
        /// </summary>
        public class LockReleasedArguments
        {
            public T Token { get; private set; }

            public LockReleasedArguments(T token)
            {
                Token = token;
            }
        }
    }
}
