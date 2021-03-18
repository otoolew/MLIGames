//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: William Roberts, Eric Policaro
//  Date:   09/05/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEditor;

namespace SG.Core.OnGUI
{
    public static class EditorOnGUILayout
    {
        public const string ScriptPropertyName = "m_Script";


        /// <summary>
        /// Draw the properties of a given object, excluding any specified fields.
        /// Properties are drawn using <code>EditorGUILayout.PropertyField</code>.
        /// </summary>
        /// <param name="obj">Object to draw</param>
        /// <param name="includeScriptProperty">Determines if the 'Script' property should be drawn or not.</param>
        /// <param name="propertiesToExclude">Properties that will not be drawn</param>
        public static void DrawPropertiesExcluding(SerializedObject obj, bool excludeScriptProperty, params string[] propertiesToExclude)
        {
            var exclusions = new HashSet<string>();

            if (excludeScriptProperty)
                exclusions.Add(ScriptPropertyName);

            if (propertiesToExclude != null)
            {
                for (int i = 0; i < propertiesToExclude.Length; i++)
                    exclusions.Add(propertiesToExclude[i]);
            }

            SerializedProperty iterator = obj.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                if (!exclusions.Contains(iterator.name))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }

                enterChildren = false;
            }
        }


        /// <summary>
        /// Draw the properties of a given object, excluding any specified fields.
        /// Properties are drawn using <code>EditorGUILayout.PropertyField</code>.
        /// </summary>
        /// <param name="obj">Object to draw</param>
        /// <param name="propertiesToExclude">Properties that will not be drawn</param>
        public static void DrawPropertiesExcluding(SerializedObject obj, params string[] propertiesToExclude)
        {
            DrawPropertiesExcluding(obj, false, propertiesToExclude);
        }
    }
}
