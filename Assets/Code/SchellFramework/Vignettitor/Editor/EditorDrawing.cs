//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

using System;
using SG.Vignettitor.Editor.NodeEditViews;
using SG.Vignettitor.Graph;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor
{
    /// <summary>
    /// A draw adapter that allows editor drawing functionality to be used in 
    /// the vignettitor (which is not defined in editor space).
    /// </summary>
    public class EditorDrawing : DrawAdapter
    {
        public override void DrawConnection(Vector3 start, Vector3 end, Vector3 startTangent, Vector3 endTangent, Color color, float width)
        {
            Handles.DrawBezier(start, end, startTangent, endTangent,
                                color, null, width);
        }

        public override Type DefaultEditView
        {
            get { return typeof(DefaultNodeEditView); }
        }

        public override string TextAreaLayout(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.TextArea(text, style, options);
        }

        public override void AddCursorRect(Rect position, MouseCursor mouse)
        {
            EditorGUIUtility.AddCursorRect(position, (UnityEditor.MouseCursor)(int)mouse);
        }
    }
}
