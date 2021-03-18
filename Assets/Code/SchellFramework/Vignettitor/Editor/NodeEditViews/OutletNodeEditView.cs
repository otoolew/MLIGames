// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Eric Policaro
// 
//  Date: 02/23/2016
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SG.Vignettitor.Graph.NodeViews;
using SG.Vignettitor.Nodes;
using SG.Vignettitor.NodeViews;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor.NodeEditViews
{
    [NodeEditView(typeof(OutletNode))]
    public class OutletNodeEditView : VignetteNodeEditView
    {
        public override void Draw(Rect rect)
        {
            base.Draw(rect);
            OutletNode n = (OutletNode)Node;
            n.OutletName = GUILayout.TextField(n.OutletName);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Order", GUILayout.Width(50f));
            n.Order = EditorGUILayout.IntField(n.Order);
            EditorGUILayout.EndHorizontal();
        }
    }
}