// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   12/27/2016
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SG.Core.AssetIcons;
using UnityEngine;

namespace SG.GlobalEvents
{
    /// <summary>
    /// The simplest usable global event. This does not pass any data.
    /// </summary>
    [CreateAssetMenu]
    [GenerateAssetIcon(DEFAULT_ASSET_ICON)]
    public class GlobalEvent : BaseGlobalEvent
    {
        #region -- Container Classes ------------------------------------------
        /// <summary>
        /// Delegate definition used for direct code registration.
        /// </summary>
        //public delegate void EventRaisedCallback();

        /// <summary>
        /// Wrapper around any listener that can register with this event.
        /// </summary>
        private abstract class ListenerContainer : 
            IGlobalEventListener, IPrioritizeable
        {
            public int Priority { get; private set; }
            protected ListenerContainer(int priority) { Priority = priority; }
            public abstract void HandleEvent();
        }

        /// <summary>
        /// Wrapper around global event listeners for objects implementing
        /// IGlobalEventListener.
        /// </summary>
        private class ObjectListenerContainer : ListenerContainer
        {
            private readonly IGlobalEventListener listener;

            public ObjectListenerContainer(IGlobalEventListener listener,
                int priority) : base(priority)
            { this.listener = listener; }

            public override void HandleEvent()
            { listener.HandleEvent(); }
        }

        /// <summary>
        /// Wrapper around listeners that simply invoke a delegate.
        /// </summary>
        private class DelegateListenerContainer : ListenerContainer
        {
            private readonly EventRaisedVoidCallback callback;

            public DelegateListenerContainer(EventRaisedVoidCallback callback,
                int priority) : base(priority)
            { this.callback = callback; }

            public override void HandleEvent()
            { callback.Invoke(); }
        }
        #endregion -- Container Classes ---------------------------------------

        #region -- Private Fields ---------------------------------------------
        /// <summary>
        /// The listeners that will respond to this event, either delegate, 
        /// interface or object.
        /// </summary>
        [NonSerialized]
        private readonly List<ListenerContainer> listeners = new List<ListenerContainer>();

        /// <summary>
        /// Mapping of the object that started the listening request (delegate, 
        /// GlobalEventListener, etc) to the container that can trigger it.
        /// </summary>
        [NonSerialized]
        private readonly Dictionary<object, ListenerContainer>
            registeredObjects = new Dictionary<object, ListenerContainer>();
        #endregion -- Private Fields ------------------------------------------

        #region -- Listener Registration --------------------------------------
        /// <summary>
        /// Registers a listener container to respond to an event.
        /// </summary>
        /// <param name="listener">Lister to respond</param>
        /// <param name="triggerSticky">
        /// If true and the event is sticky and has been raised, the listener 
        /// is notified upon registration.
        /// </param>
        private void RegisterListenerContainer(ListenerContainer listener,
            bool triggerSticky)
        {
            AddListener(listener, listeners);

            if (Sticky && triggerSticky && stuck)
                listener.HandleEvent();
        }

        /// <summary>
        /// Registers a listener with this event. All registered listeners are
        /// notified when the event is raised. If the event is declared to be 
        /// "Sticky" and has already been raised, the listener will be notified 
        /// immediately.
        /// </summary>
        /// <param name="listener">
        /// The global event listener that should be notified when this event
        /// is raised.
        /// </param>
        /// <param name="triggerSticky">
        /// If true and the event is sticky and has been raised, the listener 
        /// is notified upon registration. Default is true.
        /// </param>
        /// <param name="priority">
        /// Priority for ordering how listeners are notified. Listeners of a 
        /// lower priority number will be notified before listeners with higher 
        /// numbers. The default is 0 so it is common to have negative numbers 
        /// for priority in order to make a listener get a notification sooner 
        /// than the default. Default is 0.
        /// </param>
        public override void RegisterListener(IGlobalEventListener listener, 
            bool triggerSticky = true, int priority = 0)
        {
            if (!ValidateRegistration(listener))
                return;

            EventRecorder.Record("Register GlobalEventListener");

            ObjectListenerContainer container = 
                new ObjectListenerContainer(listener, priority);

            if (!registeredObjects.ContainsKey(listener))
                registeredObjects.Add(listener, container);

            RegisterListenerContainer(container, triggerSticky);
        }

        /// <summary>
        /// Remove the listener from the event so that it will no longer be 
        /// notified when the event is raised. If the listener is not 
        /// registered, no action is taken.
        /// </summary>
        /// <param name="listener">
        /// The global event listener to remove for the registered list.
        /// </param>
        public override void UnregisterListener(IGlobalEventListener listener)
        {
            if (registeredObjects.ContainsKey(listener))
            {
                EventRecorder.Record("Unregister");
                listeners.Remove(registeredObjects[listener]);
                registeredObjects.Remove(listener);
            }
        }

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
        public override void RegisterDelegateListener(EventRaisedVoidCallback callback,
            bool triggerSticky = true, int priority = 0)
        {
            bool canRegister = ValidateDelegateRegistration(callback);
            if (!canRegister) return;

            EventRecorder.Record("Register Delegate");

            if (!registeredObjects.ContainsKey(callback))
            {
                ListenerContainer dc = new DelegateListenerContainer(callback, priority);
                registeredObjects.Add(callback, dc);
                RegisterListenerContainer(dc, triggerSticky);
            }
        }

        /// <summary>
        /// Remove the delegate from the event so that it will no longer be 
        /// invoked when the event is raised. If the delegate is not 
        /// registered, no action is taken.
        /// </summary>
        /// <param name="callback">
        /// The delegate to remove for the registered list.
        /// </param>
        public override void UnregisterDelegateListener(EventRaisedVoidCallback callback)
        {
            if (registeredObjects.ContainsKey(callback))
            {
                EventRecorder.Record("Unregister Delegate");
                listeners.Remove(registeredObjects[callback]);
                registeredObjects.Remove(callback);
            }
        }

        /// <summary> Checks if there are any listeners. </summary>
        /// <returns>True if there are object or delegate listeners.</returns>
        public override bool HasListeners()
        { return listeners.Count > 0; }
        #endregion -- Listener Registration -----------------------------------

        #region -- Raising ----------------------------------------------------
        public override Type GetDataType()
        { return typeof(void); }

        /// <summary>
        /// Raise this event, notifying all registered listeners to execute 
        /// their responses.
        /// </summary>
        /// <exception cref="GlobalEventListenerException">
        /// Raised if there are no registered listeners and OnNoListeners
        /// is ErrorProcedure.Exception.
        /// </exception>
        /// <exception cref="GlobalEventDeprecationException">
        /// Raised if the event is deprecated and the OnDeprecatedRaise 
        /// procedure is ErrorProcedure.Exception.
        /// </exception>
        public void Raise()
        {
            if (!ValidateRaise())
                return;

            EventRecorder.Record("Raise");

            for (int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].HandleEvent();

            HandleSticky();
        }

        /// <summary>
        /// Raise this event, notifying all registered listeners to execute 
        /// their responses. This treats the data as a System.object for use 
        /// when the exact type that the event handles is not known, but with
        /// this type of global event, the data must be null. 
        /// </summary>
        /// <param name="data"> 
        /// Must be null since this type of global events do not pass data.
        /// </param>
        /// <exception cref="GlobalEventArgumentException">
        /// Raised if "data" is not null since this doesn't expect an argument.
        /// </exception>
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
        public override bool RaiseGeneric(object data)
        {
            if (data == null)
            {
                Raise();
                return true;
            }
            Debug.LogException(new GlobalEventArgumentException(
                "Attempting to raise a void event that expects a null " +
                "argument with an argument of type '" +
                data.GetType().Name + "'."), this);
            return false;
        }
        #endregion -- Raising -------------------------------------------------
    }
}