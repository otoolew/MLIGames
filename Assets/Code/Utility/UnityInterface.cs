using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MLIGames.Utilities
{
    /// <summary>
    /// Base interface from which all serializable interfaces must derive
    /// </summary>
    public interface IUnityInterface
    {
    }

    /// <summary>
    /// Abstract base for serializable interface wrapper objects
    /// </summary>
    public abstract class UnityInterface
    {
        /// <summary>
        /// Unity component that gets serialized that is of our interface type
        /// </summary>
        public UnityEngine.Object unityObjectReference;
    }
    /// <summary>
    /// A generic solution to allow the serialization of interfaces in Unity game objects
    /// </summary>
    /// <typeparam name="T">Any interface implementing ISerializableInterface</typeparam>
    [Serializable]
    public class UnityInterface<T> : UnityInterface where T: IUnityInterface
    {
        T interfaceReference;

        /// <summary>
        /// Retrieves the interface from the unity component and caches it
        /// </summary>
        public T GetInterface()
        {
            if (interfaceReference == null && unityObjectReference != null)
            {
                interfaceReference = (T)(IUnityInterface)unityObjectReference;
            }

            return interfaceReference;
        }
    }
}

