// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   12/12/2016
// ----------------------------------------------------------------------------

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SG.Core
{
    /// <summary>
    /// Extension functions for Serialized Properties in the editor.
    /// </summary>
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Gets the value of the property cast (and boxed, if necessary) to 
        /// the type System.object. This will get the value of the associated 
        /// property value field based on the propertyType.
        /// 
        /// For instance, if prop is a float property, the result is 
        /// (object)prop.floatValue.
        /// </summary>
        /// <param name="prop">Property whose value to get.</param>
        /// <returns>
        /// An object that is or is a boxed version of the value of the 
        /// property, based on propertyType.
        /// </returns>
        public static object GetPropertyValue(this SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return prop.intValue;
                case SerializedPropertyType.Boolean:
                    return prop.boolValue;
                case SerializedPropertyType.Float:
                    return prop.floatValue;
                case SerializedPropertyType.String:
                    return prop.stringValue;
                case SerializedPropertyType.Color:
                    return prop.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return prop.objectReferenceValue;
                case SerializedPropertyType.LayerMask:
                    return prop.intValue;
                case SerializedPropertyType.Enum:
                    return prop.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return prop.vector2Value;
                case SerializedPropertyType.Vector3:
                    return prop.vector3Value;
                case SerializedPropertyType.Vector4:
                    return prop.vector4Value;
                case SerializedPropertyType.Rect:
                    return prop.rectValue;
                case SerializedPropertyType.ArraySize:
                    return prop.intValue;
                case SerializedPropertyType.Character:
                    return prop.intValue;
                case SerializedPropertyType.AnimationCurve:
                    return prop.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return prop.boundsValue;
                case SerializedPropertyType.Quaternion:
                    return prop.quaternionValue;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Sets the value of a serializedProperty without needing to know the 
        /// exact type. This will cast the object to the type defined by 
        /// prop's propertyType.
        /// 
        /// For instance, if prop is a float property, the value cast to a 
        /// float and is written to the float field.
        /// </summary>
        /// <param name="prop">Property to set the value of.</param>
        /// <param name="value">
        /// Value to set the property value. If this is null, the property will 
        /// be set to the default value for its type.
        /// </param>
        /// <exception cref="System.InvalidCastException">
        /// Thrown if value is not of the type defined by prop's propertyType.
        /// </exception>
        public static void SetPropertyValue(this SerializedProperty prop, object value)
        {
            if (value == null)
            {
                prop.ClearPropertyValue();
                return;
            }

            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    prop.intValue = (int)value;
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = (bool)value;
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = (float)value;
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = (string)value;
                    break;
                case SerializedPropertyType.Color:
                    prop.colorValue = (Color)value;
                    break;
                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = (Object)value;
                    break;
                case SerializedPropertyType.LayerMask:
                    prop.intValue = (int)value;
                    break;
                case SerializedPropertyType.Enum:
                    prop.enumValueIndex = (int)value;
                    break;
                case SerializedPropertyType.Vector2:
                    prop.vector2Value = (Vector2)value;
                    break;
                case SerializedPropertyType.Vector3:
                    prop.vector3Value = (Vector3)value;
                    break;
                case SerializedPropertyType.Vector4:
                    prop.vector4Value = (Vector4)value;
                    break;
                case SerializedPropertyType.Rect:
                    prop.rectValue = (Rect)value;
                    break;
                case SerializedPropertyType.ArraySize:
                    prop.intValue = (int)value;
                    break;
                case SerializedPropertyType.Character:
                    prop.intValue = (int)value;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    prop.animationCurveValue = (AnimationCurve)value;
                    break;
                case SerializedPropertyType.Bounds:
                    prop.boundsValue = (Bounds)value;
                    break;
                case SerializedPropertyType.Quaternion:
                    prop.quaternionValue = (Quaternion)value;
                    break;
            }
        }

        /// <summary>
        /// Clears the appropriate value field of the property to the default 
        /// of the expected property type.
        /// </summary>
        /// <param name="prop">Property to clear.</param>
        public static void ClearPropertyValue(this SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    prop.intValue = 0;
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = false;
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = 0.0f;
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = "";
                    break;
                case SerializedPropertyType.Color:
                    prop.colorValue = Color.black;
                    break;
                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = null;
                    break;
                case SerializedPropertyType.LayerMask:
                    prop.intValue = 0;
                    break;
                case SerializedPropertyType.Enum:
                    prop.enumValueIndex = 0;
                    break;
                case SerializedPropertyType.Vector2:
                    prop.vector2Value = Vector2.zero;
                    break;
                case SerializedPropertyType.Vector3:
                    prop.vector3Value = Vector3.zero;
                    break;
                case SerializedPropertyType.Vector4:
                    prop.vector4Value = Vector4.zero;
                    break;
                case SerializedPropertyType.Rect:
                    prop.rectValue = Rect.zero;
                    break;
                case SerializedPropertyType.ArraySize:
                    prop.intValue = 0;
                    break;
                case SerializedPropertyType.Character:
                    prop.intValue = 0;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    prop.animationCurveValue = AnimationCurve.Linear(0, 0, 1, 1);
                    break;
                case SerializedPropertyType.Bounds:
                    prop.boundsValue = new Bounds(Vector3.zero, Vector3.zero);
                    break;
                case SerializedPropertyType.Quaternion:
                    prop.quaternionValue = Quaternion.identity;
                    break;
            }
        }

        // TODO: add support for multiedit - use serializedObjects, return object[]
        /// <summary>
        /// Gets a system.object that the property path of the given 
        /// SerializedProperty represents. This is useful when you have defined 
        /// your own serializable types and would like to reference them in a 
        /// property drawer.
        /// 
        /// For example, if the SerializedProperty path is something like
        /// "MyMonoBehaviour.MyArray[2].MySerializable" this will return thei
        /// instance of MySerializable.
        /// </summary>
        /// <param name="property">Property to get the object for.</param>
        /// <returns>
        /// The object referenced by the property's propertyPath.
        /// </returns>
        public static object GetObjectForProperty(this SerializedProperty property)
        {
            System.Type t = property.serializedObject.targetObject.GetType();
            object obj = property.serializedObject.targetObject;
            string[] props = property.propertyPath.Split('.');
            for (int i = 0; i < props.Length; i++)
            {
                FieldInfo fi = t.GetField(props[i], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (fi == null)
                {
                    if (props[i] == "Array")
                        continue;
                    if (props[i].Contains("["))
                    {
                        int index = int.Parse(props[i].Replace("data[", "").Replace("]", ""));
                        object[] obs = (object[])obj;
                        obj = obs[index];

                        if (i == props.Length - 1)
                            return obj;

                        t = t.GetElementType();
                    }
                }
                else
                {
                    obj = fi.GetValue(obj);

                    if (i == props.Length - 1)
                        return obj;

                    t = fi.FieldType;
                }
            }
            return null;
        }
    }
}