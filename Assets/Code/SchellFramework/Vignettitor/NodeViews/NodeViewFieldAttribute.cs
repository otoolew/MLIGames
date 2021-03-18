// ----------------------------------------------------------------------------
//  Copyright © 2017 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   01/25/2017
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace SG.Vignettitor.NodeViews
{
    /// <summary>
    /// An attribute that may be put on any serialized field of a VignetteNode
    /// to indicate that the field should be displayed in the node view.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class NodeViewFieldAttribute : Attribute
    {
        /// <summary>
        /// A mapping of field info and the attribute that was on it.
        /// </summary>
        public class FieldAttributePair
        {
            public FieldInfo Field;
            public NodeViewFieldAttribute Attribute;
        }

        /// <summary>
        /// How the height of the field should be determined.
        /// </summary>
        public enum HeightMode
        {
            Default,
            Fill,
            Specify
        }

        /// <summary> How to select filed height</summary>
        public HeightMode FieldHeightMode { get; protected set; }

        /// <summary> Height to draw the field. </summary>
        public int Height { get; protected set; }

        /// <summary> Should the name of the field be displayed? </summary>
        public bool DrawFieldName;

        /// <summary>
        /// Should this field autofocus all the time? Only one per object 
        /// should have this.
        /// </summary>
        public bool AutoFocus;

        public NodeViewFieldAttribute()
        {
            FieldHeightMode = HeightMode.Default;
            DrawFieldName = false;
        }

        public NodeViewFieldAttribute(int fieldHeight)
        {
            FieldHeightMode = HeightMode.Specify;
            Height = fieldHeight;
            DrawFieldName = false;
        }

        public NodeViewFieldAttribute(bool fillHeight)
        {
            FieldHeightMode = HeightMode.Fill;
            DrawFieldName = false;
        }

        /// <summary>
        /// Get a list of all fields on a node that use the 
        /// NodeEditViewFieldAttribute. THe list will map the field to the 
        /// attribute that was found on it.
        /// </summary>
        /// <param name="node">
        /// Object to inspect for NodeEditViewFieldAttribute fields.
        /// </param>
        /// <returns>
        /// A pairing of fields to NodeEditViewFieldAttributes.
        /// </returns>
        public static List<FieldAttributePair> GetFields<T>(object node) where T : NodeViewFieldAttribute
        {
            List<FieldAttributePair> result = new List<FieldAttributePair>();
            FieldInfo[] fields = node.GetType().GetFields(BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; i++)
            {
                T[] atts = fields[i].GetCustomAttributes(typeof(T), true) as T[];

                for (int a = 0; a < atts.Length; a++)
                {
                    if (atts[a].GetType() == typeof (T))
                    {
                        result.Add(new FieldAttributePair
                        {
                            Field = fields[i],
                            Attribute = atts[a]
                        });
                        break;
                    }
                }
            }
            return result;
        }
    }
}