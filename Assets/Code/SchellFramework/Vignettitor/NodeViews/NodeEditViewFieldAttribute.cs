//-----------------------------------------------------------------------------
//  Copyright © 2017 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   01/25/2017
//-----------------------------------------------------------------------------

using System;

namespace SG.Vignettitor.NodeViews
{
    /// <summary>
    /// An attribute that may be put on any serialized field of a VignetteNode
    /// to indicate that the field should be editable in the node view when 
    /// the node is double clicked. This will only show up if there is no 
    /// NodeEditView defined for the node.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class NodeEditViewFieldAttribute : NodeViewFieldAttribute
    {
        public NodeEditViewFieldAttribute()
        {
            FieldHeightMode = HeightMode.Default;
            DrawFieldName = true;
        }

        public NodeEditViewFieldAttribute(int fieldHeight)
        {
            FieldHeightMode = HeightMode.Specify;
            Height = fieldHeight;
            DrawFieldName = true;
        }

        public NodeEditViewFieldAttribute(bool fillHeight)
        {
            FieldHeightMode = HeightMode.Fill;
            DrawFieldName = true;
        }
    }
}
