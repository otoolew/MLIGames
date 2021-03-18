// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   12/27/2016
// ----------------------------------------------------------------------------

using SG.Core;
using SG.Core.Inspector;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace SG.GlobalEvents
{
    /// <summary>
    /// Base class for all Global Event Listener classes. 
    /// BaseGlobalEventListeners can register for events and handle errors with
    /// event registration.
    /// </summary>
    public abstract class BaseGlobalEventListener : MonoBehaviour, IGlobalEventHandler
    {
        /// <summary>
        /// A Unity function called at the beginning of an objects life cycle.
        /// </summary>
        public enum StartFunction
        {
            None,
            Awake,
            OnEnable,
            Start
        }

        /// <summary>
        /// A Unity function called at the end of an objects life cycle.
        /// </summary>
        public enum EndFunction
        {
            None,
            OnDisable,
            OnDestroy
        }

        /// <summary>
        /// When doing time based operations, like delays, this defines what 
        /// time measurement to use.
        /// </summary>
        public enum TimeType
        {
            None,
            ScaledTime,
            UnscaledTime
        }

#if UNITY_EDITOR
        /// <summary>
        /// This caches the last event used on this object in the editor in 
        /// order to prevent data loss when changing types (since the 
        /// GlobalEvent field gets nulled out due to type mismatch.
        /// 
        /// Since it is serialized, it is available to other classes so the 
        /// warning is disabled.
        /// </summary>
        [SerializeField]
        protected Object oldEvent;
#endif
        #region -- Inspector Fields -------------------------------------------
        // TODO: should advanced determine if the advanced behavior is used or just if it is drawn in the editor?
        [Tooltip("Exposes advanced event responses when true.")]
        public bool Advanced;

        [Tooltip("Defines the order ordering that listeners are handle a " +
                 "specific event. Listeners of a lower priority number will " +
                 "handle an event before listeners with higher numbers. The " +
                 "default is 0 so it is common to have negative numbers for " +
                 "priority in order to make a listener handle a notification " +
                 "sooner than the default. Default is 0.")]
        public int Priority;
        
        [Tooltip("How should the response be delayed?\n" +
                 " - None: no delay\n" +
                 " - ScaledTime: delay, taking Time.timscale into account\n" +
                 " - UnscaledTime: delay in real time")]
        public TimeType DelayType = TimeType.None;

        [Tooltip("If the delay type is not none, this is how many seconds to " +
                 "wait after an event is raised before triggering the response.")]
        [IntConditionalDraw("DelayType", ComparisonOperator.NotEqual, 0, AlwaysDraw = true)]
        public FloatRange Delay = new FloatRange(1.0f, true, 1.0f, true);

        [Tooltip("When will this listener register with the GlobalEvent?\n" +
                 " - None: This will not automatically register and needs to " +
                 "be registered manually with a call to PerformRegister.\n" +
                 " - Awake: Registered on Awake; only called once.\n" +
                 " - OnEnable: Registered on OnEnable; called every time this is turned on.\n" +
                 " - Start: Registered on Start; only called once.")]
        public StartFunction RegistrationFunction = StartFunction.OnEnable;

        [Tooltip("When will this listener unregister with the GlobalEvent?\n" +
                 " - None: This will not automatically unregister and needs to " +
                 "be unregister manually with a call to PerformUnregister " +
                 "or it can be left always registered.\n" +
                 " - OnDisable: Unregister whenever the object is disabled, " +
                 "often paired with OnEnable registration.\n" +
                 " - OnDestroy: Unregister when the object is destroyed.")]
        public EndFunction UnregistrationFunction = EndFunction.OnDisable;

        [Tooltip("If the GlobalEvent that this is supposed to register for is " +
                 "null, what should happen?\n" +
                 " - Nothing: Do not handle, will trigger a NullReference.\n" +
                 " - Warning: Throw a warning and skip registration.\n" +
                 " - Exception: Throw an exception and skip registration.")]
        [FormerlySerializedAs("OnNullListener")]
        public ErrorProcedure OnNullGlobalEvent = ErrorProcedure.Exception;
        #endregion -- Inspector Fields ----------------------------------------

        #region -- Registration -----------------------------------------------
        /// <summary>
        /// Error string to display then a listener attempts to register with a 
        /// null event.
        /// </summary>
        protected string NullGlobalEventError
        {
            get
            {
                return GetType().Name + " is unable to register since the " +
                     "supplied GlobalEvent is null.";
            }
        }

        /// <summary>
        /// Does this Listener have a valid GlobalEvent to respond to?
        /// </summary>
        /// <returns>True if there is a non-null GlobalEvent.</returns>
        protected abstract bool HasEvent();

        /// <summary>
        /// Register this listener with its GlobalEvent so that it will be 
        /// invoked each time the GlobalEvent is raised.
        /// </summary>
        /// <param name="triggerSticky">
        /// If the event is sticky and has already been raised, should it 
        /// invoke this listener immediately?
        /// </param>
        protected abstract void Register(bool triggerSticky);

        /// <summary>
        /// Unregister this listener from its GlobalEvent so that it no longer
        /// gets invoked each time the GlobalEvent is raised.
        /// </summary>
        protected abstract void Unregister();

        /// <summary>
        /// Register this listener to respond to a GlobalEvent after 
        /// performing error checking.
        /// </summary>
        public void PerformRegister()
        {
            if (!HasEvent())
            {
                if (OnNullGlobalEvent == ErrorProcedure.Exception)
                    Debug.LogException(new GlobalEventListenerException(
                        NullGlobalEventError), this);
                else if (OnNullGlobalEvent == ErrorProcedure.Warning)
                    Debug.LogWarning(NullGlobalEventError, this);
            }
            else
            {
                Register(true);
            }
        }

        /// <summary>
        /// Unregister this listener to respond to a GlobalEvent after 
        /// performing error checking.
        /// </summary>
        public void PerformUnregister()
        {
            if (!HasEvent())
            {
                if (OnNullGlobalEvent == ErrorProcedure.Exception)
                    Debug.LogException(new GlobalEventListenerException(
                        NullGlobalEventError), this);
                else if (OnNullGlobalEvent == ErrorProcedure.Warning)
                    Debug.LogWarning(NullGlobalEventError, this);
            }
            else
            {
                Unregister();
            }
        }

        #endregion -- Registration --------------------------------------------

        #region -- MonoBehaviour Functions ------------------------------------
        protected virtual void Awake()
        {
            if (RegistrationFunction == StartFunction.Awake)
                PerformRegister();
        }
        
        protected virtual void OnEnable()
        {
            if (RegistrationFunction == StartFunction.OnEnable)
                PerformRegister();
        }

        protected virtual void Start()
        {
            if (RegistrationFunction == StartFunction.Start)
                PerformRegister();
        }
        
        protected virtual void OnDisable()
        {
            if (UnregistrationFunction == EndFunction.OnDisable)
                PerformUnregister();
        }

        protected virtual void OnDestroy()
        {
            if (UnregistrationFunction == EndFunction.OnDestroy)
                PerformUnregister();
        }
        #endregion -- MonoBehaviour Functions ---------------------------------\

        /// <summary>
        /// Stops all pending executions that are waiting for a delay to 
        /// finish when using anything other than DelayType == TimeType.None.
        /// </summary>
        public void CancelDelayInvoke()
        {
            StopAllCoroutines();
        }

        public virtual void OnValidate() { }

        public abstract bool GenericHandleEvent(BaseGlobalEvent e, object data);

        #region -- Gizmo Drawing ----------------------------------------------
#if UNITY_EDITOR
        /// <summary> Has a gizmo been drawn for this event yet? </summary>
        private bool gizmoCached;

        /// <summary> Texture to draw for this gizmo. </summary>
        private Texture2D gizmoCache;
        
        /// <summary>
        /// For a given world position, find a rectangle of size iconSize 
        /// that can be drawn in the field of view but represents the location 
        /// of the world position even when it is not on screen.
        /// 
        /// For instance, when worldPosition is off screen to the right, the 
        /// result rect will pin to the right edge of the screen.
        /// </summary>
        /// <param name="worldPosition">Source position.</param>
        /// <param name="cam">Camera to map the world position into.</param>
        /// <param name="cameraRect">The rect that the camera renders into.</param>
        /// <param name="iconSize">
        /// The size of the icon to draw pinned to the screen edge.
        /// </param>
        /// <returns>
        /// A rect that can render on camera in the camera rect that best 
        /// represents the world position.
        /// </returns>
        private static Rect GetPinnedScreenRect(Vector3 worldPosition, Camera cam, Rect cameraRect, Vector2 iconSize)
        {
            Vector3 screenPosition = cam.WorldToScreenPoint(worldPosition);

            Rect iconRect = new Rect(screenPosition.x - (iconSize.x/2),
                cameraRect.height - screenPosition.y - iconSize.y, iconSize.x, iconSize.y);

            Vector3 listenerVector = (worldPosition - cam.transform.position);
            float lookDirection = Vector3.Dot(listenerVector.normalized, cam.transform.forward);

            Rect s = new Rect(cameraRect);
            s.position = Vector2.zero;
            if (!s.Overlaps(iconRect) || lookDirection <= 0)
            {
                Vector3 dif = (worldPosition - cam.transform.position).normalized;
                Vector3 rd = cam.transform.InverseTransformDirection(dif).normalized*s.width*s.height;

                Vector3 c = s.center;
                Vector3 d = s.center + new Vector2(rd.x, rd.y);

                // Find the point on one of the four screen edges that the 
                // Vector from the center of the screen to the icon point 
                // intersects.
                Vector2 intersect;
                if (SgMath.LineSegIntersect(c, d, new Vector2(s.xMax, s.yMin), new Vector2(s.xMax, s.yMax),
                    out intersect))
                    iconRect.position = new Vector2(intersect.x, s.height - intersect.y);
                if (SgMath.LineSegIntersect(c, d, new Vector2(s.xMin, s.yMin), new Vector2(s.xMin, s.yMax),
                    out intersect))
                    iconRect.position = new Vector2(intersect.x, s.height - intersect.y);
                if (SgMath.LineSegIntersect(c, d, new Vector2(s.xMax, s.yMin), new Vector2(s.xMin, s.yMin),
                    out intersect))
                    iconRect.position = new Vector2(intersect.x, s.height - intersect.y);
                if (SgMath.LineSegIntersect(c, d, new Vector2(s.xMax, s.yMax), new Vector2(s.xMin, s.yMax),
                    out intersect))
                    iconRect.position = new Vector2(intersect.x, s.height - intersect.y);

                if (iconRect.width > cameraRect.width || iconRect.x < 0)
                    iconRect.x = 0;
                else
                    iconRect.x -= Mathf.Max(0.0f, iconRect.xMax - cameraRect.width);

                if (iconRect.height > cameraRect.height || iconRect.y < 0)
                    iconRect.y = 0;
                else
                    iconRect.y -= Mathf.Max(0.0f, iconRect.yMax - cameraRect.height + (iconSize.y/2.0f));
            }
            return iconRect;
        }

        /// <summary>
        /// Draws a gizmo consisting of the global event name and icon this 
        /// listener is listening to. The gizmo is shown only when the global 
        /// event is selected.
        /// </summary>
        /// <param name="globalEvent">Global Event to draw gizmos for.</param>
        protected void DrawListenerGizmo(BaseGlobalEvent globalEvent)
        {
            UnityEditor.SceneView sceneView = UnityEditor.SceneView.currentDrawingSceneView;
            if (sceneView != null)
            {
                UnityEditor.Handles.BeginGUI();

                // Cache an icon to draw for the event listener.
                if (!gizmoCached)
                {
                    gizmoCached = true;
                    gizmoCache = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(
                        "Assets/Gizmos/" + globalEvent.GetType().Name + " icon.png");
                }

                Vector3 screenPosition = sceneView.camera.WorldToScreenPoint(transform.position);
                Vector3 listenerVector = (transform.position - sceneView.camera.transform.position);
                float lookDirection = Vector3.Dot(listenerVector.normalized, sceneView.camera.transform.forward);

                Vector2 nameSize = GUI.skin.label.CalcSize(new GUIContent(globalEvent.name + " "));
                Vector2 iconSize = gizmoCache == null ? 
                    new Vector2(20, 20) : 
                    new Vector2(gizmoCache.width, gizmoCache.height) * 0.5f;

                Rect iconRect = new Rect(screenPosition.x - (iconSize.x / 2),
                    sceneView.position.height - screenPosition.y - iconSize.y, iconSize.x, iconSize.y);

                Rect labelRect = new Rect(screenPosition.x + iconSize.x * 0.5f,
                    sceneView.position.height - screenPosition.y - (iconSize.y*0.85f), nameSize.x, nameSize.y);

                Rect total = Rect.MinMaxRect(
                        Mathf.Min(iconRect.xMin, labelRect.xMin),
                        Mathf.Min(iconRect.yMin, labelRect.yMin),
                        Mathf.Max(iconRect.xMax, labelRect.xMax),
                        Mathf.Max(iconRect.yMax, labelRect.yMax));

                // If looking toward the listener, draw a label with the event
                // name and listen for a click.
                if (lookDirection >= 0)
                {
                    Core.OnGUI.OnGUIUtils.DrawBox(total, "", new Color(5.0f, 5.0f, 5.0f, 0.2f), Color.clear);
                    GUI.color = Color.black;
                    GUI.Label(labelRect, globalEvent.name + " ");
                    GUI.color = Color.white;

                    if (Event.current.type == EventType.MouseUp)
                    {
                        if (total.Contains(Event.current.mousePosition))
                        {
                            UnityEditor.EditorGUIUtility.PingObject(gameObject);
                            UnityEditor.EditorApplication.delayCall += () =>
                            UnityEditor.Selection.activeGameObject = gameObject;
                        }
                    }
                }

                // Draw the icon for the listener pinned to the edge of the 
                // screen if the listener is off screen
                Rect camRect = sceneView.position;
                camRect.position = Vector2.zero;
                iconRect = GetPinnedScreenRect(transform.position, sceneView.camera, camRect, iconSize);
                Core.OnGUI.OnGUIUtils.DrawBox(iconRect, "", new Color(5.0f, 5.0f, 5.0f, 0.2f), Color.clear);
                if (gizmoCache != null)
                    GUI.Label(iconRect, gizmoCache);

                UnityEditor.Handles.EndGUI();
            }
        }
#endif
        #endregion -- Gizmo Drawing -------------------------------------------
    }
}