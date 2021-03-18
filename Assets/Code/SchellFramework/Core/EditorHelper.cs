// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 12/4/2015 12:32:00 PM
// ------------------------------------------------------------------------------

#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

namespace SG.Core
{
    /// <summary>
    /// Some Editor Utility functions.
    /// </summary>
    public static class EditorHelper
    {
        public static T[] FindAll<T>() where T : Object
        {
            return AssetDatabase.FindAssets(string.Concat("t:", typeof(T).FullName))
                // ReSharper disable once RedundantTypeArgumentsOfMethod - Unity compiler can't infer it    
                .Select<string, string>(AssetDatabase.GUIDToAssetPath)
                // ReSharper disable once RedundantTypeArgumentsOfMethod - Unity compiler can't infer it
                .SelectMany<string, Object>(AssetDatabase.LoadAllAssetsAtPath)
                .OfType<T>().ToArray();
        }

        public static T FindFirst<T>() where T : Object
        {
            return FindAll<T>().FirstOrDefault();
        }

        public static void AddPersistentListenerIfNotPresent(UnityEvent unityEvent, Object callComponent, string callMethodName, UnityAction callAction)
        {
            int listeners = unityEvent.GetPersistentEventCount();
            for (int i = 0; i < listeners; i++)
            {
                if (callComponent == unityEvent.GetPersistentTarget(i) &&
                    callMethodName == unityEvent.GetPersistentMethodName(i))
                    return;
            }
            UnityEventTools.AddPersistentListener(unityEvent, callAction);
        }

        public static void AddPersistentListenerIfNotPresent<T0>(UnityEvent<T0> unityEvent, Object callComponent, string callMethodName, UnityAction<T0> callAction)
        {
            int listeners = unityEvent.GetPersistentEventCount();
            for (int i = 0; i < listeners; i++)
            {
                if (callComponent == unityEvent.GetPersistentTarget(i) &&
                    callMethodName == unityEvent.GetPersistentMethodName(i))
                    return;
            }
            UnityEventTools.AddPersistentListener(unityEvent, callAction);
        }
    }
}

#endif
