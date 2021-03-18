// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   12/02/2016
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SG.Core.Inspector
{
    /// <summary>
    /// Draws the field marked by an attribute if all of the 
    /// ConditionalDrawAttributes on the field evaluate to true.
    /// </summary>
    public abstract class ConditionalDrawAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Does the set of attributes evaluate to true or false?
        /// </summary>
        protected bool condition;

        /// <summary>
        /// Does any associated attribute request to be always drawn?
        /// </summary>
        protected bool alwaysDraw;

        /// <summary>
        /// A static cache of what types draw what attributes.
        /// </summary>
        protected static readonly Dictionary<Type, Type> 
            attributeTypeToDrawerType = new Dictionary<Type, Type>();

        protected static readonly Dictionary<string, Type>
            attributeTypeToElementDrawerType = new Dictionary<string, Type>();

        /// <summary>
        /// The logic to determine if the dependent property should be drawn.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        protected abstract bool ShouldDrawProperty(SerializedProperty prop);

        /// <summary>
        /// Gets a drawer instance that is used to draw a given attribute.
        /// </summary>
        /// <param name="cda">Attribute of which a drawer is desired.</param>
        /// <param name="conditionalfi">FieldInfo that the attribute is a part of.</param>
        /// <returns></returns>
        protected static ConditionalDrawAttributeDrawer GetDrawer(ConditionalDrawAttribute cda, FieldInfo conditionalfi)
        {
            Type cdaType = cda.GetType();
            Type drawerType = null;

            if (attributeTypeToDrawerType.ContainsKey(cdaType))
            {
                drawerType = attributeTypeToDrawerType[cdaType];
            }

            if (drawerType == null)
            {
                Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
                for (int i = 0; i < allTypes.Length; i++)
                {
                    object[] atts = allTypes[i].GetCustomAttributes(typeof (CustomPropertyDrawer), true);

                    for (int a = 0; a < atts.Length; a++)
                    {
                        CustomPropertyDrawer drawer = atts[a] as CustomPropertyDrawer;
                        FieldInfo fi = drawer.GetType().GetField("m_Type", BindingFlags.Instance | BindingFlags.NonPublic);
                        Type targetType = fi.GetValue(drawer) as Type;
                        if (targetType == cdaType)
                        {
                            drawerType = allTypes[i];
                            break;
                        }
                    }
                    if (drawerType != null)
                        break;
                }
            }

            if (drawerType != null)
            {
                ConditionalDrawAttributeDrawer result =
                    Activator.CreateInstance(drawerType) as ConditionalDrawAttributeDrawer;
                FieldInfo fi_m_Attribute = drawerType.GetField("m_Attribute",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo fi_m_FieldInfo = drawerType.GetField("m_FieldInfo",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                fi_m_Attribute.SetValue(result, cda);
                fi_m_FieldInfo.SetValue(result, conditionalfi);
                if (!attributeTypeToDrawerType.ContainsKey(cdaType))
                    attributeTypeToDrawerType.Add(cdaType, drawerType);
                return result;
            }
            return null;
        }

        // TODO: move to serializedProperty helper class
        public static PropertyDrawer GetDrawerElement(string typeName, FieldInfo conditionalfi)
        {
            Type drawerType = null;

            if (attributeTypeToElementDrawerType.ContainsKey(typeName))
            {
                drawerType = attributeTypeToElementDrawerType[typeName];

                if (drawerType != null)
                {
                    PropertyDrawer result =
                        Activator.CreateInstance(drawerType) as PropertyDrawer;
                    FieldInfo fi_m_FieldInfo = drawerType.GetField("m_FieldInfo",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    fi_m_FieldInfo.SetValue(result, conditionalfi);
                    if (!attributeTypeToElementDrawerType.ContainsKey(typeName))
                        attributeTypeToElementDrawerType.Add(typeName, drawerType);
                    return result;
                }
                return null;
            }

            if (drawerType == null)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int asm = 0; asm < assemblies.Length; asm++)
                {
                    Type[] allTypes = assemblies[asm].GetTypes();
                    //Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
                    for (int i = 0; i < allTypes.Length; i++)
                    {
                        object[] atts = allTypes[i].GetCustomAttributes(typeof (CustomPropertyDrawer), true);

                        for (int a = 0; a < atts.Length; a++)
                        {
                            CustomPropertyDrawer drawer = atts[a] as CustomPropertyDrawer;
                            FieldInfo fi = drawer.GetType()
                                .GetField("m_Type", BindingFlags.Instance | BindingFlags.NonPublic);
                            Type targetType = fi.GetValue(drawer) as Type;
                            if (targetType.Name == typeName)
                            {
                                drawerType = allTypes[i];
                                break;
                            }

                            for (int asm2 = 0; asm2 < assemblies.Length; asm2++)
                            {
                                IEnumerable<Type> subs =
                                    assemblies[asm2].GetTypes().Where(t => targetType.IsAssignableFrom(t));
                                foreach (Type sub in subs)
                                {
                                    if (sub.Name == typeName)
                                    {
                                        drawerType = allTypes[i];
                                        break;
                                    }
                                }
                            }
                        }
                        if (drawerType != null)
                            break;
                    }
                }
            }

            if (drawerType != null)
            {
                PropertyDrawer result =
                    Activator.CreateInstance(drawerType) as PropertyDrawer;
                FieldInfo fi_m_FieldInfo = drawerType.GetField("m_FieldInfo",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                fi_m_FieldInfo.SetValue(result, conditionalfi);
                if (!attributeTypeToElementDrawerType.ContainsKey(typeName))
                    attributeTypeToElementDrawerType.Add(typeName, drawerType);
                return result;
            }
            if (!attributeTypeToElementDrawerType.ContainsKey(typeName))
                attributeTypeToElementDrawerType.Add(typeName, null);
            return null;
        }

        /// <summary>
        /// Checks if the given property should be displayed in the inspector 
        /// based on the ConditionalDrawAttributes on it.
        /// </summary>
        /// <param name="property">Property to check.</param>
        /// <returns>True if it should be drawn.</returns>
        protected bool ShouldDraw(SerializedProperty property)
        {
            bool result = true;
            
            alwaysDraw = false;

            // In order to support multiple attributes of this type, loop 
            // through all of the attributes on the field rather than just 
            // using the attribute member.
            object[] atts = fieldInfo.GetCustomAttributes(typeof(ConditionalDrawAttribute), false);
            for (int i = 0; i < atts.Length; i++)
            {
                ConditionalDrawAttribute a = atts[i] as ConditionalDrawAttribute;
                string newPath = property.depth == 0 ? a.PropertyName :
                    Path.ChangeExtension(property.propertyPath, a.PropertyName);
                SerializedProperty sp = property.serializedObject.FindProperty(newPath);
                        
                if (sp != null)
                {
                    ConditionalDrawAttributeDrawer d = GetDrawer(a, fieldInfo);
                    bool eval = d.ShouldDrawProperty(sp);
                    condition = a.Invert ? !eval : eval;
                    result &= condition;
                    if (a.AlwaysDraw)
                        alwaysDraw = true;
                }
            }
            condition = result;
            result = result || alwaysDraw;
            
            return result;
        }

        #region -- PropertyDrawer Overrides -----------------------------------
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldDraw(property))
            {
                PropertyDrawer pd = GetDrawerElement(property.type, fieldInfo);
                if (pd != null)
                    return pd.GetPropertyHeight(property, label);
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldDraw(property))
            {
                bool didDraw = GUI.enabled;
                if (alwaysDraw)
                    GUI.enabled = condition;

                PropertyDrawer pd = GetDrawerElement(property.type, fieldInfo);
                if (pd != null)
                    pd.OnGUI(position, property, label);
                else
                    EditorGUI.PropertyField(position, property, label, true);

                if (alwaysDraw)
                    GUI.enabled = didDraw;
            }
        }
        #endregion -- PropertyDrawer Overrides --------------------------------
    }

    #region -- Common Implementations -----------------------------------------
    [CustomPropertyDrawer(typeof(NullConditionalDrawAttribute))]
    public class NullConditionalDrawAttributeDrawer : ConditionalDrawAttributeDrawer
    {
        protected override bool ShouldDrawProperty(SerializedProperty prop)
        {
            if (prop.propertyType == SerializedPropertyType.ObjectReference)
                return prop.objectReferenceValue == null;
            return false;
        }
    }

    [CustomPropertyDrawer(typeof(BoolConditionalDrawAttribute))]
    public class BoolConditionalDrawAttributeDrawer : ConditionalDrawAttributeDrawer
    {
        protected override bool ShouldDrawProperty(SerializedProperty prop)
        {
            return prop.boolValue;
        }
    }

    [CustomPropertyDrawer(typeof(FloatConditionalDrawAttribute))]
    public class FloatConditionalDrawAttributeDrawer : ConditionalDrawAttributeDrawer
    {
        protected override bool ShouldDrawProperty(SerializedProperty prop)
        {
            FloatConditionalDrawAttribute f = attribute as FloatConditionalDrawAttribute;
            return FloatComparison.Evaluate(prop.floatValue, f.Operator, f.Value);
        }
    }

    [CustomPropertyDrawer(typeof(IntConditionalDrawAttribute))]
    public class IntConditionalDrawAttributeDrawer : ConditionalDrawAttributeDrawer
    {
        protected override bool ShouldDrawProperty(SerializedProperty prop)
        {
            IntConditionalDrawAttribute i = attribute as IntConditionalDrawAttribute;
            return IntComparison.Evaluate(prop.intValue, i.Operator, i.Value);
        }
    }
    #endregion -- Common Implementations --------------------------------------
}