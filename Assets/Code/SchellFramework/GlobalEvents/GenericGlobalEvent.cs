//-----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   06/21/2016
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SG.GlobalEvents
{
    /// <summary>
    /// A base class for Global Events that pass arguments.
    /// 
    /// Global events can handle sending data to registered listeners or to 
    /// delegates. In order to handle
    /// </summary>
    /// <typeparam name="TArgument">
    /// The data type passed by this event and that is handled by the 
    /// GlobalEventListener defined by "TListener".
    /// </typeparam>
    public class GenericGlobalEvent<TArgument> : BaseGlobalEvent
    {
        #region -- Container Classes ------------------------------------------
        /// <summary>
        /// Delegate definition used for direct code registration.
        /// </summary>
        public delegate void EventRaisedCallback(TArgument argument);
        
        /// <summary>
        /// Wrapper around any listener that can register with this event.
        /// </summary>
        protected abstract class ListenerContainer : IGlobalEventListener<TArgument>, IPrioritizeable
        {
            public int Priority { get; private set; }

            protected ListenerContainer(int priority)
            { Priority = priority; }
            public abstract void HandleEvent(TArgument arg);
        }

        private class VoidListenerContainer : ListenerContainer
        {
            private IGlobalEventListener listener;

            public VoidListenerContainer(
                IGlobalEventListener listener, int priority)
                : base(priority)
            { this.listener = listener; }

            public override void HandleEvent(TArgument arg)
            { listener.HandleEvent(); }
        }

        /// <summary>
        /// Wrapper around listeners that simply invoke a delegate.
        /// </summary>
        private class VoidDelegateListenerContainer : ListenerContainer
        {
            private EventRaisedVoidCallback callback;

            public VoidDelegateListenerContainer(EventRaisedVoidCallback callback,
                int priority)
                : base(priority)
            { this.callback = callback; }

            public override void HandleEvent(TArgument arg)
            { callback.Invoke(); }
        }

        /// <summary>
        /// Wrapper around global event listeners for objects implementing
        /// IGlobalEventListener.
        /// </summary>
        private class ObjectListenerContainer : ListenerContainer
        {
            private IGlobalEventListener<TArgument> listener;

            public ObjectListenerContainer(
                IGlobalEventListener<TArgument> listener, int priority)
                : base(priority)
            { this.listener = listener; }

            public override void HandleEvent(TArgument arg)
            { listener.HandleEvent(arg); }
        }
        
        /// <summary>
        /// Wrapper around listeners that simply invoke a delegate.
        /// </summary>
        private class DelegateListenerContainer : ListenerContainer
        {
            private EventRaisedCallback callback;

            public DelegateListenerContainer(EventRaisedCallback callback, 
                int priority) : base(priority)
            { this.callback = callback; }

            public override void HandleEvent(TArgument arg)
            { callback.Invoke(arg); }
        }
        #endregion -- Container Classes ---------------------------------------

        #region -- Private Variables ------------------------------------------
        /// <summary>
        /// The IGlobalEventListener that are registered for this event. When 
        /// the event is raised, each on of these listeners is notified.
        /// </summary>
        [NonSerialized]
        private readonly List<ListenerContainer> genericListeners
            = new List<ListenerContainer>();

        /// <summary>
        /// The last argument that this event was raised with. This is stored
        /// so that sticky events can reuse the last argument and so it can be
        ///  displayed or edited in the inspector.
        /// </summary>
        [SerializeField]
        private TArgument lastArgument;

        /// <summary>
        /// If the last argument was a scene object in the editor, this stores 
        /// the instance ID since it is the only way to serialize it.
        /// </summary>
        private int lastArgumentInstanceID;

        /// <summary>
        /// A mapping of registered objects to the container objects used 
        /// to store them.
        /// </summary>
        private readonly Dictionary<object, ListenerContainer>
            registeredObjects = new Dictionary<object, ListenerContainer>();
        #endregion -- Private Variables ---------------------------------------

        #region -- Properties -------------------------------------------------
        /// <summary>
        /// Access the last argument passed by this event.
        /// 
        /// This has special case handling in the editor to allow for better 
        /// inspector debug-ability. This will store the instance ID to allow 
        /// for references to scene objects to be displayed.
        /// </summary>
        public TArgument LastArgument
        {
            protected set
            {
                lastArgument = value;

#if UNITY_EDITOR
                Object o = lastArgument as Object;
                lastArgumentInstanceID = o != null ? o.GetInstanceID() : 0;
#endif
            }
            get
            {
#if UNITY_EDITOR
                if (lastArgumentInstanceID != 0 && lastArgument == null)
                {
                    object o = UnityEditor.EditorUtility.InstanceIDToObject(lastArgumentInstanceID);
                    return (TArgument)o;
                }
#endif
                return lastArgument;
            }
        }
        #endregion -- Properties ----------------------------------------------

        #region -- Listener Registration --------------------------------------
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
        public void RegisterListener(IGlobalEventListener<TArgument> listener,
            bool triggerSticky = true, int priority = 0)
        {
            bool canRegister = ValidateRegistration(listener);
            if (!canRegister) return;

            EventRecorder.Record("Register IGlobalEventListener");

            ObjectListenerContainer container =
                new ObjectListenerContainer(listener, priority);

            if (!registeredObjects.ContainsKey(listener))
                registeredObjects.Add(listener, container);

            RegisterListenerContainer(container, triggerSticky);
        }

        public override void RegisterListener(IGlobalEventListener listener,
            bool triggerSticky = true, int priority = 0)
        {
            bool canRegister = ValidateRegistration(listener);
            if (!canRegister) return;

            EventRecorder.Record("Register IGlobalEventListener");

            VoidListenerContainer container =
                new VoidListenerContainer(listener, priority);

            if (!registeredObjects.ContainsKey(listener))
                registeredObjects.Add(listener, container);

            RegisterListenerContainer(container, triggerSticky);
        }

        public override void UnregisterListener(IGlobalEventListener listener)
        {
            if (registeredObjects.ContainsKey(listener))
            {
                EventRecorder.Record("Unregister");
                genericListeners.Remove(registeredObjects[listener]);
                registeredObjects.Remove(listener);
            }
        }

        public override void RegisterDelegateListener(EventRaisedVoidCallback callback,
            bool triggerSticky = true, int priority = 0)
        {
            bool canRegister = ValidateDelegateRegistration(callback);
            if (!canRegister) return;

            EventRecorder.Record("Register Delegate");

            if (!registeredObjects.ContainsKey(callback))
            {
                ListenerContainer dc = new VoidDelegateListenerContainer(callback, priority);
                registeredObjects.Add(callback, dc);
                RegisterListenerContainer(dc, triggerSticky);
            }
        }

        public override void UnregisterDelegateListener(EventRaisedVoidCallback callback)
        {
            if (registeredObjects.ContainsKey(callback))
            {
                EventRecorder.Record("Unregister Delegate");
                genericListeners.Remove(registeredObjects[callback]);
                registeredObjects.Remove(callback);
            }
        }

        /// <summary>
        /// Registers a listener with this event. All registered listeners are
        /// notified when the event is raised. If the event is declared to be 
        /// "Sticky" and it has already been raised, the listener will be 
        /// notified immediately if triggerStickyEvents is true.
        /// </summary>
        /// <param name="listener">
        /// The global event listener that should be notified when this event
        /// is raised.
        /// </param>
        /// <param name="triggerSticky">
        /// If true, and if the event is declared to be "Sticky" and it has 
        /// already been raised, then the listener will be notified of the 
        /// event with the last argument.
        /// </param>
        protected void RegisterListenerContainer(ListenerContainer listener,
            bool triggerSticky)
        {
            AddListener(listener, genericListeners);

            if (Sticky && triggerSticky && stuck)
                listener.HandleEvent(LastArgument);
        }

        /// <summary>
        /// Remove the listener from the event so that it will no longer be 
        /// notified when the event is raised.
        /// </summary>
        /// <param name="listener">
        /// The global event listener to remove for the registered list.
        /// </param>
        public void UnregisterListener(IGlobalEventListener<TArgument> listener)
        {
            if (registeredObjects.ContainsKey(listener))
            {
                EventRecorder.Record("Unregister");
                genericListeners.Remove(registeredObjects[listener]);
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
        public void RegisterDelegateListener(EventRaisedCallback callback, 
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
        public void UnregisterDelegateListener(EventRaisedCallback callback)
        {
            if (registeredObjects.ContainsKey(callback))
            {
                EventRecorder.Record("Unregister Delegate");
                genericListeners.Remove(registeredObjects[callback]);
                registeredObjects.Remove(callback);
            }
        }

        

        /// <summary> Checks if there are any listeners. </summary>
        /// <returns>True if there are object or delegate listeners.</returns>
        public override bool HasListeners()
        { return genericListeners.Count > 0; }
        #endregion -- Listener Registration -----------------------------------

        #region -- Event Raising ----------------------------------------------
        public override Type GetDataType()
        { return typeof (TArgument); }

        /// <summary>
        /// Raise this event, notifying all registered listeners to execute 
        /// their responses with the supplied data.
        /// </summary>
        /// <param name="data">
        /// The data to send to all of the registered listeners and to cache as 
        /// the last passed argument.
        /// </param>
        /// <exception cref="GlobalEventListenerException">
        /// Raised if there are no registered listeners and ErrorProcedure
        /// is ErrorProcedure.Exception.
        /// </exception>
        /// <exception cref="GlobalEventDeprecationException">
        /// Raised if the event is deprecated and the OnDeprecatedRaise 
        /// procedure is ErrorProcedure.Exception.
        /// </exception>
        public void Raise(TArgument data)
        {
            if (!ValidateRaise())
                return;

            EventRecorder.Record("Raise");

            for (int i = genericListeners.Count - 1; i >= 0; i--)
                genericListeners[i].HandleEvent(data);

            LastArgument = data;
            HandleSticky();
        }

        /// <summary>
        /// Raise this event, notifying all registered listeners to execute 
        /// their responses with the supplied data. This treats the data as a
        /// System.object for use when the exact type that the event handles is
        /// not known.
        /// </summary>
        /// <param name="data">
        ///  Argument to raise the event with. This must be of the type 
        ///  defined by "TArgument".
        /// </param>
        /// <exception cref="GlobalEventArgumentException">
        /// Raised if the argument is not of the expected type (defined by 
        /// "TArgument") or if the argument is null and the expected type is a 
        /// value type and therefore can not be null.
        /// </exception>
        /// <exception cref="GlobalEventListenerException">
        /// Raised if there are no registered listeners and ErrorProcedure
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
            if (data is TArgument)
            {
                Raise((TArgument)data);
                return true;
            }
            
            // If the argument is null and it is allowed to be (like if it 
            // is an object reference), raise it.
            if (data == null && !typeof(TArgument).IsValueType)
            {
                // ReSharper disable once ExpressionIsAlwaysNull
                // This can not pass null directly since it is not know 
                // that it can cast to TArgument.
                Raise((TArgument)data);
                return true;
            }

            // Otherwise error since value types can not handle null
            string argType = data == null
                ? "null"
                : data.GetType().Name;
            Debug.LogException(new GlobalEventArgumentException(
                "Attempting to raise event that expects type '" +
                typeof(TArgument).Name + 
                "' with an argument of type '" + argType + "'."),
                this);
            return false;
        }
        #endregion -- Event Raising -------------------------------------------
    }
}
