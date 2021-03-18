//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   12/10/2015 11:51:30 AM
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SG.Core.Inspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SG.GlobalEvents
{
    #region -- Data Structures ------------------------------------------------
    /// <summary>
    /// Interface used to generically refer to prioritizeable events since 
    /// the actual type may not be known due to generic arguments.
    /// </summary>
    public interface IPrioritizeable
    { int Priority { get; } }

    /// <summary>
    /// Anything that can listen for global events of the given type.
    /// </summary>
    /// <typeparam name="T">Type of data handled by the event.</typeparam>
    public interface IGlobalEventListener<in T>
    { void HandleEvent(T arg); }

    /// <summary>
    /// Anything that can listen for global events that do not pass data.
    /// </summary>
    public interface IGlobalEventListener
    { void HandleEvent(); }

    /// <summary>
    /// Anything that can handle an event and it's data. This is implemented 
    /// by classes that work with a GlobalEventListenerGroup that allows for 
    /// listening objects to be directly given an event to handle.
    /// </summary>
    public interface IGlobalEventHandler
    { bool GenericHandleEvent(BaseGlobalEvent e, object data); }

    /// <summary>
    /// Options to respond when an event has no listeners.
    /// </summary>
    public enum ErrorProcedure
    {
        /// <summary> Listeners are not required, do nothing. </summary>
        Nothing,

        /// <summary> Listeners are required, throw a warning. </summary>
        Warning,

        /// <summary> Listeners are required, raise exception. </summary>
        Exception
    }
    #endregion -- Data Structures ---------------------------------------------

    /// <summary>
    /// The root of all GlobalEvent types. A GlobalEvent represents a generic
    /// instruction that can be broadcast to interested listeners. Each 
    /// listener can respond to the event in a unique way.
    /// 
    /// GlobalEvents are useful in decoupling dependencies between systems.
    /// </summary>
    public abstract class BaseGlobalEvent : ScriptableObject
    {
        public delegate void EventRaisedVoidCallback();

        #region -- Editor Icon Generation -------------------------------------
        /// <summary>
        /// Base64-string based texture used as an icon when new GlobalEvent 
        /// types are generated.
        /// </summary>
        protected const string DEFAULT_ASSET_ICON = "iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAB0ElEQVRoBdWYa26EMAyEl6rX4bSV2gv1JFykraUGhcQkdmac7PIHCGTyzdiwj23f90fE9v11RMhWmu/ViH/gxz+lOWNrXi0uvhXnL3eKGlDTB9rHlb6kjRpgVswNjxpQ0x90NASPGlBZgfZR9XqDz9BCw+kjFVDbZyB9CB4x0Kus5ToMP2pAT//zsECne1SNdNGz5z0Dvjxp63qFGMn5rHbK4TWgyjkeXiq8wFAMqK4mDXoMqO2zMv2ZFaC3Tiqw9feAmn4SMezR+WmJKghPCyWRc+9on3MOcFDBixZkAIDxTlXhrQbU8k9M/xbeasCbFvP+JrzFgJ7+x8GEvNPqwlsM6OLWd5c+2zJqgu8ZUNO3rA7eY4aXdbbGH1thBhovABe8GGg1g1tMBLNNDYAJL2s9y+fAcFhRBjzpD8NPrcBN60DwUQbU9GWxYoPhRS+qhS6sSvoU+GkGLm7+Xt3FOXTKrkDVPkX6VPjwCkTDsw1U6We9QU8+abNbKOk+svTD4NkVOOGzg1B4poFL+/ynHw4vBlrfRrMgu4cXA6LbnUG6gfEMLIOXDBgG8iynJZ8WZRqYDs+oQGqfJfAMA6KxDB41IOkvhUcNLIdHDcj85dsvo+FElJbDVK4AAAAASUVORK5CYII=";

        /// <summary> Color key used to recolor generated icons. </summary>
        protected const string DEFAULT_ASSET_ICON_REPLACE_KEY = "F1C40F";
        #endregion -- Editor Icon Generation ----------------------------------

        #region -- Inspector Variables ----------------------------------------
        /// <summary>
        /// Record actions that occur with this global event for debugging. 
        /// This is only used in the editor so in builds, this will no-op.
        /// </summary>
        public readonly GlobalEventHistory EventRecorder = new GlobalEventHistory();

        [Tooltip("Should this event stick around to be triggered for " +
                 "objects that register for after it is raised but before " +
                 "sticky events are cleared?")]
        public bool Sticky;
        
        [Tooltip("If this event is raised and there are no listeners to " +
                 "respond to it, what should happen?\n" +
                 "Nothing - Listeners are not required, do nothing.\n" +
                 "Warning - Listeners are required, throw a warning.\n" +
                 "Exception - Listeners are required, raise exception.")]
        public ErrorProcedure OnNoListeners = ErrorProcedure.Nothing;

        [Tooltip("If an event is deprecated, it can no longer be added to " +
                 "a listener and will throw warnings or exceptions when " +
                 "things register for it or if it is raised. This is used " +
                 "to phase out events on larger projects.")]
        public bool Deprecated;

        [Tooltip("How to respond if this event is deprecated and is raised.\n" +
                 "Nothing - Event is raised as normal.\n" +
                 "Warning - Event is raised and a warning is logged.\n" +
                 "Exception - Event not raised and an exception is thrown.")]
        [BoolConditionalDraw("Deprecated")]
        public ErrorProcedure OnDeprecatedRaise = ErrorProcedure.Exception;

        [Tooltip("How to respond if this event is deprecated and something " +
                 "attempts to register for it.\n" +
                 "Nothing - Event is registered as normal.\n" +
                 "Warning - Event is registered and a warning is logged.\n" +
                 "Exception - Event not registered and an exception is thrown.")]
        [BoolConditionalDraw("Deprecated")]
        public ErrorProcedure OnDeprecatedRegister = ErrorProcedure.Exception;

        [HideInInspector]
        [Tooltip("Developer notes for an event, not used in game.")]
        public string Notes;
        #endregion -- Inspector Variables -------------------------------------
        
        #region -- Protected Fields -------------------------------------------
        /// <summary>
        /// Records all events that are flagged as sticky and that have been 
        /// raised. This is used to clear all sticky events.
        /// </summary>
        protected static readonly List<BaseGlobalEvent> stuckEvents = 
            new List<BaseGlobalEvent>();

        /// <summary>
        /// If this event is sticky and it has been raised. When this is true, 
        /// anything registering for this event will be immediately invoked.
        /// </summary>
        [NonSerialized] protected bool stuck;
        #endregion -- Protected Fields ----------------------------------------

        #region -- Registration -----------------------------------------------
        /// <summary>
        /// Adds a prioritizeable entry to a list. This will ensure the list 
        /// remains sorted from low priority to high. This is optimized for 
        /// most entries to of priority 0 since it is expected to be the 
        /// average scenario.
        /// </summary>
        /// <typeparam name="T">Type of prioritizeable data.</typeparam>
        /// <param name="element">Element to insert.</param>
        /// <param name="list">List to add the element to.</param>
        protected static void AddListener<T>(T element, List<T> list) 
            where T : IPrioritizeable
        {
            // The list is constructed in reverse order to allow it to be 
            // processed backwards. This means that the first element will 
            // have the highest order number (lowest priority) and the last
            // element will have the lowest order number (highest priority).

            if (list.Count == 0)
            {
                list.Add(element);
            }
            else
            {
                // lowest number, highest priority
                int first = list[list.Count - 1].Priority;

                // highest number, lowest priority
                int last = list[0].Priority;

                // if more important than the most important, 
                // put it at the bottom (processed first)
                if (element.Priority < first) 
                    list.Add(element);

                // if less (or equally) important than the least important, 
                // put it at top (processed last)
                else if (element.Priority >= last) 
                    list.Insert(0, element);

                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        int p = list[i].Priority;
                        if (p <= element.Priority)
                        {
                            list.Insert(i, element);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Indicates if any global event listeners are registered to respond 
        /// to this event.
        /// </summary>
        /// <returns>True if there are more than 0 listeners.</returns>
        public abstract bool HasListeners();

        public virtual void RegisterListener(IGlobalEventListener listener,
            bool triggerSticky = true, int priority = 0)
        {}

        /// <summary>
        /// Remove the listener from the event so that it will no longer be 
        /// notified when the event is raised. If the listener is not 
        /// registered, no action is taken.
        /// </summary>
        /// <param name="listener">
        /// The global event listener to remove for the registered list.
        /// </param>
        public virtual void UnregisterListener(IGlobalEventListener listener)
        {}

        /// <summary>
        /// Registers a delegate with this event. All registered delegates are
        /// invoked when the event is raised. If the event is declared to be 
        /// "Sticky" and has already been raised, the delegate will be invoked
        /// immediately.
        /// </summary>
        /// <param name="callback">
        /// The delegate to invoke when this event is raised.
        /// </param>
        /// <param name="triggerSticky">
        /// If true and the event is sticky and has been raised, the delegate 
        /// is invoked upon registration.
        /// </param>
        /// <param name="priority">
        /// Priority for ordering how listeners are notified. Listeners of a 
        /// lower priority number will be notified before listeners with higher 
        /// numbers. The default is 0 so it is common to have negative numbers 
        /// for priority in order to make a listener get a notification sooner 
        /// than the default.
        /// </param>
        public virtual void RegisterDelegateListener(EventRaisedVoidCallback callback,
            bool triggerSticky = true, int priority = 0)
        {}

        /// <summary>
        /// Remove the delegate from the event so that it will no longer be 
        /// invoked when the event is raised. If the delegate is not 
        /// registered, no action is taken.
        /// </summary>
        /// <param name="callback">
        /// The delegate to remove for the registered list.
        /// </param>
        public virtual void UnregisterDelegateListener(EventRaisedVoidCallback callback)
        {}

        #endregion -- Registration --------------------------------------------
        
        #region -- Event Raising ----------------------------------------------
        /// <summary>
        /// Gets the type of data that may be passed by this event.
        /// </summary>
        /// <returns>
        /// The type of data that this event may be raised with and passed to 
        /// listeners. If this GlobalEvent does not pass data, typeof(void)
        /// is returned .
        /// </returns>
        public abstract Type GetDataType();

        /// <summary> Can this event be raised. </summary>
        /// <returns>True if raising will perform an action.</returns>
        public virtual bool CanRaise()
        {
            return !(Deprecated && 
                OnDeprecatedRaise == ErrorProcedure.Exception);
        }

        /// <summary>
        /// Raise this event, notifying all registered listeners to execute 
        /// their responses. This treats the data as a System.object for use 
        /// when the exact type that the event handles is not known.
        /// </summary>
        /// <param name="data"> Argument to raise the event with. </param>
        /// <exception cref="GlobalEventListenerException">
        /// Raised if there are no registered listeners and OnNoListeners
        /// is ErrorProcedure.Exception.
        /// </exception>
        /// <exception cref="GlobalEventDeprecationException">
        /// Raised if the event is deprecated and the OnDeprecatedRaise 
        /// procedure is ErrorProcedure.Exception.
        /// </exception>
        /// <returns>
        /// True if the event was able to be raised with the passed argument.
        /// </returns>
        public abstract bool RaiseGeneric(object data);

        /// <summary>
        /// If this is a sticky event, this will record that it is now stuck.
        /// </summary>
        protected void HandleSticky()
        {
            if (Sticky)
            {
                stuck = true;
                if (!stuckEvents.Contains(this))
                    stuckEvents.Add(this);
            }
        }

        /// <summary>
        /// If this event is sticky, clear the record that this global event 
        /// has been raised. This will stop subsequent registrations from 
        /// triggering the event raise.
        /// </summary>
        public virtual void ClearStickyEvents()
        {
            EventRecorder.Record("ClearSticky");
            stuck = false;
            if (stuckEvents.Contains(this))
                stuckEvents.Remove(this);
        }

        /// <summary>
        /// Clear all global sticky global events that have been raised. This
        /// clears everything recorded as raised and sticky in a static list 
        /// and will therefor affect all defined GlobalEvents that are sticky.
        /// </summary>
        public virtual void ClearGlobalStickyEvents()
        {
            for (int i = stuckEvents.Count - 1; i >= 0; i++)
                stuckEvents[i].ClearStickyEvents();
        }
        #endregion -- Event Raising -------------------------------------------

        #region -- Validation -------------------------------------------------
        /// <summary>
        /// Validates whether or not an event can be raised.
        /// </summary>
        /// <exception cref="GlobalEventDeprecationException">
        /// Thrown if the event is deprecated and is set to throw exceptions 
        /// on deprecation usage.
        /// </exception>
        /// <exception cref="GlobalEventListenerException">
        /// Thrown if the event has no listeners and the event is set to 
        /// throw exceptions when it has no listeners.
        /// </exception>
        /// <returns>True if it can be raised.</returns>
        protected bool ValidateRaise()
        {
            if (Deprecated)
            {
                string msg = "Attempting to raise deprecated event " + name;
                if (OnDeprecatedRaise == ErrorProcedure.Exception)
                {
                    Debug.LogException(new GlobalEventDeprecationException(
                        msg), this);
                    return false;
                }
                if (OnDeprecatedRaise == ErrorProcedure.Warning)
                {
                    Debug.LogWarning(msg, this);
                }
            }

            if (!HasListeners())
            {
                if (OnNoListeners == ErrorProcedure.Exception)
                {
                    Debug.LogException(new GlobalEventListenerException(
                        "GlobalEvent (" + name + ") was raised that " +
                        "requires listeners but does not have any " +
                        "registered."), this);
                    return false;
                }
                if (OnNoListeners == ErrorProcedure.Warning)
                {
                    Debug.LogWarning("GlobalEvent (" + name +
                        ") was raised that requires listeners but does not " +
                        "have any registered.", this);
                }
            }
            return true;
        }
        /// <summary>
        /// Validates if a listener may register with the event.
        /// </summary>
        /// <param name="listener">
        /// Listener to validate. This may be a something extending 
        /// BaseGlobalEventListener, or something implementing 
        /// IGlobalEventListener.
        /// </param>
        /// <returns>True if the listener may listen for the event.</returns>
        protected bool ValidateRegistration(object listener)
        {
            if (listener == null)
            {
                Debug.LogException(
                    new GlobalEventListenerException(
                    "Attempting to register a null listener with an event."),
                    this);
                return false;
            }

            if (Deprecated)
            {
                Object target = listener is Object ? listener as Object : this;
                string msg = "Attempting to register for deprecated event " +
                    name + " from " + listener.GetType().Name + " on " +
                    (listener is Object ? target.name : listener.ToString());
                if (OnDeprecatedRegister == ErrorProcedure.Exception)
                {
                    Debug.LogException(new GlobalEventDeprecationException(
                        msg), target);
                    return false;
                }
                if (OnDeprecatedRegister == ErrorProcedure.Warning)
                    Debug.LogWarning(msg, target);
            }

            return true;
        }

        /// <summary>
        /// Validates if a delegate listener may register with the event.
        /// </summary>
        /// <param name="callback">
        /// Delegate to check validation on.
        /// </param>
        /// <returns>True if the listener may listen for the event.</returns>
        protected bool ValidateDelegateRegistration(Delegate callback)
        {
            if (callback == null)
            {
                Debug.LogException(new GlobalEventListenerException(
                    "Attempting to register a null delegate with an event."),
                    this);
                return false;
            }

            if (Deprecated)
            {
                Object o = callback.Target as Object;
                Object target = o ?? this;
                string msg = "Attempting to register for deprecated event " +
                    name + " from with a delegate function " +
                    callback.Method.Name + " on " + (callback.Target is
                    Object ? target.name : callback.Target.ToString());
                if (OnDeprecatedRegister == ErrorProcedure.Exception)
                {
                    Debug.LogException(new GlobalEventDeprecationException(
                        msg), target);
                    return false;
                }
                if (OnDeprecatedRegister == ErrorProcedure.Warning)
                    Debug.LogWarning(msg, target);
            }
            return true;
        }
        #endregion -- Validation ----------------------------------------------
    }
}
