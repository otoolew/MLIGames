//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   12/10/2015 12:49:55 PM
//-----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace SG.GlobalEvents
{
    /// <summary>
    /// Used to raise events on enable or disable of an object.
    /// </summary>
    public class UnityEventRaiser : MonoBehaviour
    {
        #region -- Inspector Variables ----------------------------------------
        [Tooltip("If true, a coroutine will wait until the end of the frame " +
                 "of Start or OnEnable before invoking OnEnableEvents or " +
                 "OnStartEvents.")]
        public bool WaitForEndOfFrame;

        [Tooltip("UnityEvent to invoke when this MonoBehaviour is enabled.")]
        public UnityEvent OnEnableEvents;

        [Tooltip("UnityEvent to invoke when this MonoBehaviour is started.")]
        public UnityEvent OnStartEvents;

        [Tooltip("UnityEvent to invoke when this MonoBehaviour is disabled.")]
        public UnityEvent OnDisableEvents;
        #endregion -- Inspector Variables -------------------------------------

        /// <summary>
        /// Coroutine that waits for the end of the OnEnable frame.
        /// </summary>
        private Coroutine delayEnable;

        /// <summary>
        /// Coroutine that waits for the end of the Start frame.
        /// </summary>
        private Coroutine delayStart;

        private void OnEnable()
        {
            if (WaitForEndOfFrame)
                delayEnable = StartCoroutine(UnityEventRaiserDelayEnable());
            else
                OnEnableEvents.Invoke();
        }
        
        private void Start()
        {
            if (WaitForEndOfFrame)
                delayStart = StartCoroutine(UnityEventRaiserDelayStart());
            else
                OnStartEvents.Invoke();
        }
        
        private void OnDisable()
        {
            OnDisableEvents.Invoke();
        }

        private IEnumerator UnityEventRaiserDelayEnable()
        {
            // TODO: investigate a potential unity bug with waitForEndOfFrame - on startup this triggers on frame 2, in other cases it is 1 frame later, not the end of the calling frame
            //Debug.Log("b " + Time.renderedFrameCount);
            yield return new WaitForEndOfFrame();
            //Debug.Log("a " + Time.renderedFrameCount);
            OnEnableEvents.Invoke();
            //yield break;
        }

        private IEnumerator UnityEventRaiserDelayStart()
        {
            yield return new WaitForEndOfFrame();
            OnStartEvents.Invoke();
        }

        public void CancelDelayedCalls()
        {
            if (delayEnable != null)
                StopCoroutine(delayEnable);
            if (delayStart != null)
                StopCoroutine(delayStart);
        }
    }
}
