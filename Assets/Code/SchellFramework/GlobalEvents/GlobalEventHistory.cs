// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   12/22/2016
// ----------------------------------------------------------------------------
#if UNITY_EDITOR && !GLOBAL_EVENT_DISABLE_HISTORY
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using SG.Core.Collections;
#endif

namespace SG.GlobalEvents
{
    /// <summary>
    /// Base global event history recorder. This has no implementation for 
    /// recording event history, it must be implemented by inheriting classes.
    /// </summary>
    public abstract class BaseGlobalEventHistory
    {
        /// <summary>
        /// Record that something meaningful has happened with a global event.
        /// </summary>
        /// <param name="name">Name of the actions that has happened.</param>
        public virtual void Record(string name) { }
    }

#if UNITY_EDITOR && !GLOBAL_EVENT_DISABLE_HISTORY
    /// <summary>
    /// Records global event history by taking a snapshot of the stack trace,
    /// time, and frame number to aid in debugging.
    /// </summary>
    public class GlobalEventHistory : BaseGlobalEventHistory
    {
        #region -- Recording Classes ------------------------------------------
        /// <summary>
        /// Class that stores a record of any meaningful action that a global 
        /// event does for use in debugging.
        /// </summary>
        public class GlobalEventRecord
        {
            /// <summary>
            /// Name for the action that the global event performed.
            /// </summary>
            public string ActionName;

            /// <summary>
            /// Formatted stack trace that lead to the event raise.
            /// </summary>
            public string Trace;

            /// <summary> Time that the action occurred. </summary>
            public float Time;

            /// <summary> Frame number that the action occurred. </summary>
            public int Frame;

            public GlobalEventRecord(string actionName, string trace, 
                float time, int frame)
            {
                ActionName = actionName;
                Trace = trace;
                Time = time;
                Frame = frame;
            }
        }

        /// <summary>
        /// Class that can store a list of recently used global event records.
        /// </summary>
        public class RecordRecentsList : RecentsList<GlobalEventRecord>
        { public RecordRecentsList(int maxLength) : base(maxLength) { } }
        #endregion -- Recording Classes ---------------------------------------

        /// <summary>
        /// The total amount actions recorded for an event.
        /// </summary>
        public int ActionCount;

        /// <summary>
        /// A list of the most recent recorded actions for an event.
        /// </summary>
        public RecordRecentsList RecentRecords = new RecordRecentsList(20);

        /// <summary>
        /// Records an action for a global event. This will store the stack 
        /// trace that lead up to the action, the time and frame, and the name 
        /// of the action.
        /// </summary>
        /// <param name="actionName">Name to record for the action.</param>
        public override void Record(string actionName)
        {
            ActionCount++;
            string stackString = "";
            StackTrace st = new StackTrace(true);
            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame f = st.GetFrame(i);
                MethodBase method = f.GetMethod();
                string methodName = method.Name;
                string className = method.ReflectedType == null 
                    ? "<no type found>" :
                    method.ReflectedType.Name;
                string callText = className + "." + methodName;
                stackString += callText + " : " + f.GetFileLineNumber() + "\n";
            }
            RecentRecords.Add(
                new GlobalEventRecord( actionName, stackString, 
                Time.realtimeSinceStartup, Time.frameCount));
        }
    }
#else
    /// <summary>
    /// Stub global event history class used in builds that has no 
    /// implementation and therefore performs no actions.
    /// </summary>
    public class GlobalEventHistory : BaseGlobalEventHistory { }
#endif

}