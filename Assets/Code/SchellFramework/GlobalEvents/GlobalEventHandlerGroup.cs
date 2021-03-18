// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   12/29/2016
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace SG.GlobalEvents
{
    /// <summary>
    /// Serves as an entry point for a group of listeners that want to get 
    /// custom GlobalEvent requests. A group indexes a given set of listeners
    /// and, using the ForwardEvent call, can directly send the event to the 
    /// listeners without triggering other listeners that use that event.
    /// </summary>
    public class GlobalEventHandlerGroup : MonoBehaviour
    {
        [Tooltip("A list of MonoBehaviours that are either " +
                 "GlobalEventListeners or implement IGlobalEventHandler that" +
                 "this object can forward events to.")]
        public List<MonoBehaviour> Handlers;

        /// <summary>
        /// IGlobalEventHandler stored from the 
        /// </summary>
        private IGlobalEventHandler[] handlers;
        
        /// <summary>
        /// Removes invalid items from the Listeners list.
        /// </summary>
        private void OnValidate()
        {
            for (int i = Handlers.Count-1; i >= 0; i--)
            {
                IGlobalEventHandler h = Handlers[i] as IGlobalEventHandler;
                if (h == null)
                    Handlers.RemoveAt(i);
            }
        }

        /// <summary>
        /// Cache all of the objects that can handle getting the global event 
        /// data. This must be done on start since interface references can 
        /// not be serialized.
        /// </summary>
        private void Start()
        {
            handlers = new IGlobalEventHandler[Handlers.Count];
            for (int i = 0; i < Handlers.Count; i++)
            {
                handlers[i] = Handlers[i] as IGlobalEventHandler;
            }
        }

        /// <summary>
        /// Forwards a global event and data to all known listeners. Only 
        /// listeners that handle the passed event will process the data.
        /// </summary>
        /// <param name="e">The event to forward.</param>
        /// <param name="data">The data to pass.</param>
        /// <returns>True if any listener handled the data.</returns>
        public bool ForwardEvent(BaseGlobalEvent e, object data)
        {
            bool handled = false;
            for (int i = 0; i < handlers.Length; i++)
            {
                if (handlers[i].GenericHandleEvent(e, data))
                {
                    handled = true;
                }
            }
            return handled;
        }
    }
}