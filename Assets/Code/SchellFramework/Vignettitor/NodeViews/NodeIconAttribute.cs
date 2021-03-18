// ----------------------------------------------------------------------------
//  Copyright © 2017 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   04/14/2017
// ----------------------------------------------------------------------------

using System;
using SG.Core.OnGUI;
using UnityEngine;

namespace SG.Vignettitor.NodeViews
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeIconAttribute : Attribute
    {
        public BuiltInTexture2D Texture;

        public NodeIconAttribute(string texture)
        {
            Texture = new BuiltInTexture2D(texture);
        }

        public static BuiltInTexture2D GetNodeIcon(Type nodeType)
        {
            object[] attributes = nodeType.GetCustomAttributes(true);
            for (int i = 0; i < attributes.Length; i++)
            {
                NodeIconAttribute a = attributes[i] as NodeIconAttribute;
                if (a != null)
                    return a.Texture;
            }
            return null;
        }
    }
}