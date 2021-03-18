// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   07/20/2016
// ----------------------------------------------------------------------------

using SG.Core.Inspector;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

namespace SG.GlobalEvents
{
    /// <summary>
    /// The base class for all generated global event types.
    /// 
    /// This class relies heavily on templatization in order to simplify code 
    /// generation. The generated code will explicitly define a class 
    /// satisfying the generic arguments that will require no body due to the 
    /// members implied by the generic types.
    /// </summary>
    /// <typeparam name="TGlobalEvent">
    /// Type of the GlobalEvent that this listener responds to.
    /// </typeparam>
    /// <typeparam name="TUnityEvent">
    /// Type of the UnityEvent that this listener invokes.
    /// </typeparam>
    /// <typeparam name="TArgument">
    /// Type of data passed by this global event and handled by TUnityEvent.
    /// </typeparam>
    public abstract class GenericGlobalEventListener<TGlobalEvent, TUnityEvent, TArgument> 
        : BaseGlobalEventListener, IGlobalEventListener<TArgument>
        where TGlobalEvent : GenericGlobalEvent<TArgument>
        where TUnityEvent : UnityEvent<TArgument>
    {
        #region -- Inspector Fields -------------------------------------------
        [Tooltip("GlobalEvent that this listener responds to.")]
        public TGlobalEvent GlobalEvent;

        [Tooltip("The UnityEvent that is invoked when the GlobalEvent is raised.")]
        public TUnityEvent Response;

        [Tooltip("If true, a response is invoked for all met conditions.\n" +
                 "If false, a response is invoked for only the first met condition.")]
        public bool InvokeForAllMetConditions;

        [Tooltip("A list of objects that will be quality checked against an " +
                 "argument passed by a GlobalEvent. If a condition equals " +
                 "the argument, the corresponding response is invoked.")]
        public TArgument[] Conditions = new TArgument[0];

        [Tooltip("Responses to invoke when the corresponding condition is met.")]
        public TUnityEvent[] Responses = new TUnityEvent[0];

        [Tooltip("Invoked if no conditions were met.")]
        public TUnityEvent OnNoConditionsMet;
        #endregion -- Inspector Fields ----------------------------------------

        public override void OnValidate()
        {
#if UNITY_EDITOR
            while(Conditions.Length > Responses.Length)
                UnityEditor.ArrayUtility.RemoveAt(ref Conditions, Conditions.Length - 1);

            while (Responses.Length > Conditions.Length)
                UnityEditor.ArrayUtility.RemoveAt(ref Responses, Responses.Length - 1);
#endif
        }

        #region -- Registration -----------------------------------------------
        protected override bool HasEvent()
        { return GlobalEvent != null; }

        protected override void Register(bool triggerSticky)
        {
            GlobalEvent.RegisterListener(this, triggerSticky, Priority);
        }

        protected override void Unregister()
        {
            GlobalEvent.UnregisterListener(this);
        }        
        #endregion -- Registration --------------------------------------------

        #region -- Event Handling ---------------------------------------------
        /// <summary>
        /// Handle an event raise with data. this will execute the primary 
        /// response and potentially some conditional responses.
        /// </summary>
        /// <param name="arg">Data passed by GlobalEvent.</param>
        public virtual void HandleEvent(TArgument arg)
        {
            if (DelayType == TimeType.None)
                Execute(arg);
            else
                StartCoroutine(DelayInvoke(Delay.GetRandomValue(), arg));
        }

        /// <summary>
        /// Execute the response to the event and any conditional responses.
        /// </summary>
        /// <param name="arg">Data to execute with.</param>
        protected virtual void Execute(TArgument arg)
        {
            Response.Invoke(arg);
            bool metCondition = false;
            for (int i = 0; i < Conditions.Length; i++)
            {
                if (Conditions[i].Equals(arg))
                {
                    metCondition = true;
                    Responses[i].Invoke(arg);
                    if (!InvokeForAllMetConditions)
                        break;
                }
            }
            if (!metCondition)
                OnNoConditionsMet.Invoke(arg);
        }

        /// <summary>
        /// Wait for a specified duration before executing the responses.
        /// </summary>
        /// <param name="delay">Seconds to wait.</param>
        /// <param name="arg">Data to pass to the responses.</param>
        /// <returns>Returns when the delay is complete.</returns>
        private IEnumerator DelayInvoke(float delay, TArgument arg)
        {
            if (DelayType == TimeType.ScaledTime)
                yield return new WaitForSeconds(delay);
            else
                yield return new WaitForSecondsRealtime(delay);
            Execute(arg);
        }

        /// <summary>
        /// Respond to an event with the supplied data. I this listener does 
        /// not care about this event, no action will be taken. 
        /// </summary>
        /// <param name="e">Event to respond to.</param>
        /// <param name="data">Data to respond with.</param>
        /// <returns>True if the event was handled.</returns>
        public override bool GenericHandleEvent(BaseGlobalEvent e, object data)
        {
            if (e != GlobalEvent)
                return false;

            if (data is TArgument)
            {
                HandleEvent((TArgument)data);
                return true;
            }

            // If the argument is null and it is allowed to be (like if it 
            // is an object reference), raise it.
            if (data == null && !typeof(TArgument).IsValueType)
            {
                // ReSharper disable once ExpressionIsAlwaysNull
                // This can not pass null directly since it is not know 
                // that it can cast to TArgument.
                HandleEvent((TArgument)data);
                return true;
            }

            // Otherwise error since value types can not handle null
            string argType = data == null
                ? "null"
                : data.GetType().Name;
            Debug.LogException(new GlobalEventArgumentException(
                "Attempting to handle event on a listener that expects " +
                "type '" + typeof(TArgument).Name + "' with an argument " +
                "of type '" + argType + "'."), this);
            return false;
        }
        #endregion -- Event Handling ------------------------------------------

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

    /// <summary>
    /// Base class for generated global event listener types that pass values 
    /// that are have a ValueRangeSet definition. This makes the generated 
    /// listener able to trigger different responses based on more complex 
    /// comparisons with the passed argument.
    /// </summary>
    /// <typeparam name="TGlobalEvent">
    /// Type of the GlobalEvent that this listener responds to.
    /// </typeparam>
    /// <typeparam name="TUnityEvent">
    /// Type of the UnityEvent that this listener invokes.
    /// </typeparam>
    /// <typeparam name="TArgument">
    /// Type of data passed by this global event and handled by TUnityEvent.
    /// </typeparam>
    /// <typeparam name="TValueRangeSet">
    /// A set of ranges of TArgument.
    /// </typeparam>
    /// <typeparam name="TValueRange">A value range of TArgument.</typeparam>
    /// <typeparam name="TValueComparison">
    /// The comparison of a TArgument into a range of TArgument.
    /// </typeparam>
    public abstract class GenericRangeGlobalEventListener
        <TGlobalEvent, TUnityEvent, TArgument, TValueRangeSet, TValueRange, TValueComparison> 
        : GenericGlobalEventListener<TGlobalEvent, TUnityEvent, TArgument>
        where TGlobalEvent : GenericGlobalEvent<TArgument>
        where TUnityEvent : UnityEvent<TArgument>
        where TArgument : IComparable
        where TValueRange : ValueRange<TArgument>
        where TValueComparison : ValueComparison<TValueRange, TArgument>
        where TValueRangeSet : ValueRangeSet<TValueComparison, TValueRange, TArgument>
    {
        [Tooltip("Set of conditions, that will map to the responses in " +
                 "GenericGlobalEventListener, to test against any argument " +
                 "in order to find a matching response.")]
        public TValueRangeSet ConditionSet;

        public override void OnValidate()
        {
#if UNITY_EDITOR
            while (ConditionSet.Ranges.Length > Responses.Length)
                UnityEditor.ArrayUtility.RemoveAt(ref ConditionSet.Ranges, ConditionSet.Ranges.Length - 1);

            while (Responses.Length > ConditionSet.Ranges.Length)
                UnityEditor.ArrayUtility.RemoveAt(ref Responses, Responses.Length - 1);

            Conditions = new TArgument[0];
#endif
        }

        protected override void Execute(TArgument arg)
        {
            Response.Invoke(arg);
            bool metCondition = false;
            for (int i = 0; i < ConditionSet.Ranges.Length; i++)
            {
                if (ConditionSet.Ranges[i].Evaluate(arg))
                {
                    metCondition = true;
                    Responses[i].Invoke(arg);
                    if (!InvokeForAllMetConditions)
                        break;
                }
                if (!metCondition)
                    OnNoConditionsMet.Invoke(arg);
            }
        }
    }
}