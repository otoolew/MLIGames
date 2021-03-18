//-----------------------------------------------------------------------------
//  Copyright © 2012 Schell Games, LLC. All Rights Reserved. 
//
//  Author: William Roberts
//  Date:   09/18/2014
//-----------------------------------------------------------------------------

using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SG.Core.OnGUI
{
    /// <summary>
    /// Similar to the default ReorderableList but adds functionality to the header for allowing
    /// the end user to collapse the list control. Note that you must use the "DoFoldoutLayout()"
    /// method to draw the list.
    /// 
    /// Note: Currently the contol only supports GUILayout.
    /// </summary>
    /// <example>
    /// FoldoutReorderableList list;
    /// 
    /// void OnEnable()
    /// {
    ///     list = FoldoutReorderableList();
    /// }
    /// 
    /// void OnInspectorGUI()
    /// {
    ///     list.DoFoldoutLayout(); // Must be called instead of DoLayoutList() to show the special foldout header.
    /// }
    /// </example>
    public class FoldoutReorderableList : ReorderableList
    {
        /// <summary>
        /// Get or sets the text displayed in the label.
        /// </summary>
        public GUIContent HeaderLabel { get; set; }


        /// <summary>
        /// Determines if the list should be expanded or not. This property will set the SerializedProperty's "isExpanded"
        /// field if an SerializedProperty was specified in the constructor. Otherwise, an internal variable is used to
        /// track the state.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return (serializedProperty != null) ? serializedProperty.isExpanded : _isExpanded;
            }
            set
            {
                if (serializedProperty != null)
                    serializedProperty.isExpanded = value;
                else
                    _isExpanded = value;
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="headerLabel">The text that will be displayed in the header.</param>
        /// <param name="elements">The list to edit.</param>
        /// <param name="elementType">The type the list is composed of.</param>
        public FoldoutReorderableList(string headerLabel, System.Collections.IList elements, System.Type elementType)
            : this(headerLabel, elements, elementType, true, true, true)
        {

        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="headerLabel">The text that will be displayed in the header.</param>
        /// <param name="elements">The list to edit.</param>
        /// <param name="elementType">The type the list is composed of.</param>
        /// <param name="draggable">Determines if the elements in the list can be dragged or not.</param>
        /// <param name="displayAddButton">Determines if the add button is displayed.</param>
        /// <param name="displayRemoveButton">Determines if the remove button is displayed.</param>
        public FoldoutReorderableList(string headerLabel, System.Collections.IList elements, System.Type elementType, bool draggable, bool displayAddButton, bool displayRemoveButton)
            : this(new GUIContent(headerLabel), elements, elementType, draggable, displayAddButton, displayRemoveButton)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="headerLabel">The GUIContent that will be displayed in the header.</param>
        /// <param name="elements">The list to edit.</param>
        /// <param name="elementType">The type the list is composed of.</param>
        /// <param name="draggable">Determines if the elements in the list can be dragged or not.</param>
        /// <param name="displayAddButton">Determines if the add button is displayed.</param>
        /// <param name="displayRemoveButton">Determines if the remove button is displayed.</param>
        public FoldoutReorderableList(GUIContent headerLabel, System.Collections.IList elements, System.Type elementType, bool draggable, bool displayAddButton, bool displayRemoveButton)
            : base(elements, elementType, draggable, true, displayAddButton, displayRemoveButton)
        {
            Initialize(headerLabel);
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="headerLabel">The text that will be displayed in the header.</param>
        /// <param name="serializedObject">The parent serializedObject</param>
        /// <param name="elements">SerializedProperty containing the data this list displays.</param>
        public FoldoutReorderableList(string headerLabel, SerializedObject serializedObject, SerializedProperty elements)
            : this(headerLabel, serializedObject, elements, true, true, true)
        {

        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="headerLabel">The text that will be displayed in the header.</param>
        /// <param name="serializedObject">The parent serializedObject</param>
        /// <param name="elements">SerializedProperty containing the data this list displays.</param>
        /// <param name="draggable">Determines if the elements in the list can be dragged or not.</param>
        /// <param name="displayAddButton">Determines if the add button is displayed.</param>
        /// <param name="displayRemoveButton">Determines if the remove button is displayed.</param>
        public FoldoutReorderableList(string headerLabel, SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayAddButton, bool displayRemoveButton)
            : this(new GUIContent(headerLabel), serializedObject, elements, draggable, displayAddButton, displayRemoveButton)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="headerLabel">The GUIContent that will be displayed in the header.</param>
        /// <param name="elements">The list to edit.</param>
        /// <param name="elementType">The type the list is composed of.</param>
        /// <param name="draggable">Determines if the elements in the list can be dragged or not.</param>
        /// <param name="displayAddButton">Determines if the add button is displayed.</param>
        /// <param name="displayRemoveButton">Determines if the remove button is displayed.</param>
        public FoldoutReorderableList(GUIContent headerLabel, SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayAddButton, bool displayRemoveButton)
            : base(serializedObject, elements, draggable, true, displayAddButton, displayRemoveButton)
        {
            Initialize(headerLabel);
        }


        public virtual void DoFoldoutLayout()
        {
            // DoLayoutList and DoList both initialize s_Defaults. Since we may not call either of them right away,
            // we need to initialize the value ourselves. Unfortunately the field is currently private and has no
            // way to set it. Therefore, we do some trickery and use reflection to get/set the value.
            if (s_DefaultsInfo.GetValue(null) == null)
                s_DefaultsInfo.SetValue(null, new Defaults());

            // If expanded, just allow the list to work as normal. We hook "drawHeaderCallback" in the constructor
            // so that our Foldout is still called.
            if (IsExpanded)
            {
                DoLayoutList();
            }
            else // Let's render our custom 'collapsed' header.
            {
                // Get the area that the header should occupy.
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                Rect headerRect = GUILayoutUtility.GetRect((float)0f, this.headerHeight, options);

                InternalDrawHeader(headerRect);
            }
        }


        /// <summary>
        /// Sets the height of the elements based on the largest element currently avaialble.
        /// </summary>
        public void AutoAdjustElementHeight()
        {
            // Note: The base class only let's us set a global height for all elements currently.
            elementHeight = GetMaximumElementHeight();
        }

        /// <summary>
        /// Renders the controls within the header. This method can be overridden in deriving classes to provide
        /// expanded functionality.
        /// </summary>
        /// <param name="headerRect">The area in which the header should be drawn.</param>
        /// <param name="isExpanded">Is the listed expanded.</param>
        /// <returns>
        /// A value indiciating if the list should be collapsed (false) or expanded (true).
        /// </returns>
        protected virtual bool OnDrawHeader(Rect headerRect, bool isExpanded)
        {
            _headerWithSize.text = string.Format("{0} [{1}]", HeaderLabel.text, count);
            headerRect.xMin = headerRect.xMin + FoldoutIndent; // Bump the foldout over so that it is not right on the edge.

            // Let's catch mouse input on the header so that we can allow the end user to toggle the foldout
            // by clicking anywhere within it.
            if (GUI.enabled)
            {
                Rect hotSpotRect = new Rect(headerRect);

                // Bump the rect out a bit so that the Foldout can continue to handle input on the arrow.
                hotSpotRect.xMin += EditorStyles.foldout.padding.left;

                if (Event.current.type == EventType.MouseUp &&
                    Event.current.button == 0)
                {
                    if (hotSpotRect.Contains(Event.current.mousePosition))
                    {
                        isExpanded = !isExpanded;
                        Event.current.Use();
                    }
                }
            }

            // Store the result in the isExapanded property.
            return EditorGUI.Foldout(headerRect, isExpanded, _headerWithSize);
        }


        /// <summary>
        /// Renders a default view of the element.
        /// </summary>
        /// <param name="rect">The area in which the element should render itself.</param>
        /// <param name="index">The index of the current element.</param>
        /// <param name="isActive">Is this element currently active?</param>
        /// <param name="isFocused">Is this element currently focused?</param>
        protected virtual void DoDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var currentElement = serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(rect, currentElement, true);
        }


        /// <summary>
        /// Controls what happens when the user presses the remove button.
        /// </summary>
        protected virtual void DoRemoveButton(ReorderableList list)
        {
            // The default unity behavior when removing an item from the array
            // is to null it if it isn't null, and actually remove the array
            // slot if it is null. This does both when the user presses the
            // remove button.
            Object objRef = null;
            SerializedProperty serializedProp = list.serializedProperty.GetArrayElementAtIndex(list.index);
            
            // Let's group all of our changes together so that we can do a single ctrl+z to easily undo the remove.
            Undo.IncrementCurrentGroup();

            // Remove the ScriptableObject's reference from the array.
            if ((serializedProp.propertyType == SerializedPropertyType.ObjectReference) && (serializedProp.objectReferenceValue != null))
            {
                objRef = serializedProp.objectReferenceValue;

                // The first time this is called, the array element at index will be set to an null value.
                // This call is only needed if the array element is currently not null.
                list.serializedProperty.DeleteArrayElementAtIndex(list.index);
            }

            // The second time this is called, the array element will be physically deleted from the array (ie:
            // The array is shorted and the data is shifted up an index if needed).
            list.serializedProperty.DeleteArrayElementAtIndex(list.index);
            list.serializedProperty.serializedObject.ApplyModifiedProperties();

            // If the referenced object is an instance (and not an asset on
            // disk), the scriptable object must be destroyed after the
            // serialized property's modifications have been applied. Otherwise,
            // the undo operation will fail to hook the ScriptableObject up to
            // the array item it was previously attached to.
            if ((objRef != null) && !EditorUtility.IsPersistent(objRef))
                Undo.DestroyObjectImmediate(objRef);

            // Update the ReorderableLists index to be the end of the list.
            int nextIndex = this.index - 1;

            if (nextIndex >= 0)
                this.index = this.index - 1;
            else
                this.index = this.serializedProperty.arraySize - 1;
        }

        /// <summary>
        /// Calculates the maximum property drawer height of contained properties.
        /// Useful for properties with conditional drawer heights.
        /// </summary>
        public float GetMaximumElementHeight()
        {
            float height = EditorGUIUtility.singleLineHeight;

            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                SerializedProperty elementProperty = serializedProperty.GetArrayElementAtIndex(i);
                height = Mathf.Max(height, EditorGUI.GetPropertyHeight(elementProperty));
            }

            return height;
        }

        private void Initialize(GUIContent headerLabel)
        {
            HeaderLabel = headerLabel;
            _headerWithSize = new GUIContent(HeaderLabel);

            drawHeaderCallback = DoDrawHeaderCallback; // This is utilized by DoLayoutList() to draw the header when expanded.
            drawElementCallback = DoDrawElementCallback;
            onRemoveCallback = DoRemoveButton;

            s_DefaultsInfo = typeof(ReorderableList).GetField(DefaultFieldName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.IgnoreCase);

            if (s_DefaultsInfo == null)
                throw new System.NullReferenceException("Failed to locate static field 'Default ReorderableList::s_Defaults'. Something changed under the hood.");
        }

        // This method is a copy of the internal "ReorderableList::DoListHeader(Rect headerRect)" 
        // private method with a few modifications. DoListHeader is not public, 
        // therefore the functionality is recreated in order to custom render the header.
        private void InternalDrawHeader(Rect headerRect)
        {
            if (Event.current.type == EventType.Repaint)
                defaultBehaviours.DrawHeaderBackground(headerRect);

            headerRect.xMin += 6f;
            headerRect.xMax -= 6f;
            headerRect.height -= 2f;
            headerRect.y++;

            DoDrawHeaderCallback(headerRect);
        }


        private void DoDrawHeaderCallback(Rect headerRect)
        {
            IsExpanded = OnDrawHeader(headerRect, IsExpanded);
        }

        private const int FoldoutIndent = 10;
        private const string DefaultFieldName = "s_Defaults";

        private bool _isExpanded = true;

        private GUIContent _headerWithSize;

        // ReSharper disable once InconsistentNaming
        // This is a clone of a private ReorderableList member.
        private FieldInfo s_DefaultsInfo;
    }
}
