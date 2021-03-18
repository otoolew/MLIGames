// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   07/20/2016
// ----------------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SG.GlobalEvents
{
    /// <summary>
    /// A GlobalEventListener registers itself with a GlobalEvent on OnEnable
    /// and unregisters itself on OnDisable. When the associated GlobalEvent is
    /// raised, the GlobalEventListener invokes its Response UnityEvent.
    /// </summary>
    public class GlobalEventListener : BaseGlobalEventListener, IGlobalEventListener
    {
        #region -- Inspector Fields -------------------------------------------
        [Tooltip("GlobalEvent that this listener responds to.")]
        public GlobalEvent GlobalEvent;

        [Tooltip("The UnityEvent that is invoked when the GlobalEvent is raised.")]
        public UnityEvent Response;
        #endregion -- Inspector Fields ----------------------------------------

        #region -- Event Handling ---------------------------------------------
        /// <summary>
        /// Respond to the GlobalEvent getting raised by invoking Response.
        /// </summary>
        public virtual void HandleEvent()
        {
            if (DelayType == TimeType.None)
                Response.Invoke();
            else
                StartCoroutine(DelayInvoke(Delay.GetRandomValue()));
        }

        /// <summary>
        /// Wait for a specified duration before executing the responses.
        /// </summary>
        /// <param name="delay">Seconds to wait.</param>
        /// <returns>Returns when the delay is complete.</returns>
        private IEnumerator DelayInvoke(float delay)
        {
            if (DelayType == TimeType.ScaledTime)
                yield return new WaitForSeconds(delay);
            else
                yield return new WaitForSecondsRealtime(delay);
            Response.Invoke();
        }

        public override bool GenericHandleEvent(BaseGlobalEvent e, object data)
        {
            if (GlobalEvent != e)
                return false;

            if (data == null)
            {
                HandleEvent();
                return true;
            }
            Debug.LogException(new GlobalEventArgumentException(
                "Attempting to handle an event with data on a listener that " +
                "expects no data. The argument must be null on basic " +
                "GlobalEventListeners."), this);
            return false;
        }
        #endregion -- Event Handling ------------------------------------------

        #region -- Registration -----------------------------------------------
        protected override bool HasEvent()
        { return GlobalEvent != null; }

        protected override void Register(bool triggerSticky)
        { GlobalEvent.RegisterListener(this, triggerSticky, Priority); }

        protected override void Unregister()
        { GlobalEvent.UnregisterListener(this); }
        #endregion -- Registration --------------------------------------------

        #region -- Gizmo Drawing ----------------------------------------------
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (GlobalEvent != null)
                for (int i = 0; i < UnityEditor.Selection.objects.Length; i++)
                    if (GlobalEvent == UnityEditor.Selection.objects[i])
                        DrawListenerGizmo(GlobalEvent);
        }
#endif
        #endregion -- Gizmo Drawing -------------------------------------------
    }
}