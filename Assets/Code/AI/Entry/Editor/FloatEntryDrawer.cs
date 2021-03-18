using UnityEngine;
using UnityEditor;
using System.Collections;
[CustomPropertyDrawer(typeof(FloatEntry))]
public class FloatEntryDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        Rect entryNameRect = new Rect(position.x, position.y, 30, position.height);
        Rect valueRect = new Rect(position.x + 35, position.y, 50, position.height);
        Rect nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        SerializedProperty entryNameProp = property.FindPropertyRelative("entryName");
        SerializedProperty valueProp = property.FindPropertyRelative("value");

        EditorGUI.PropertyField(entryNameRect, entryNameProp, GUIContent.none);
        EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
