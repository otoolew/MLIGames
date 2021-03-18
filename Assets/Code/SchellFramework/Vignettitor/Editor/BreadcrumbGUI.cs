// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Eric Policaro
// 
//  Date: 03/02/2016
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SG.Vignettitor.VignetteData;
using SG.Vignettitor.VignettitorCore;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor
{
    /// <summary>
    /// GUI drawing functions for breadcrumb navigation.
    /// </summary>
    public class BreadcrumbGUI
    {
        /// <summary>
        /// When the BreadcrumbUI is drawn in a window, this ID may be used as 
        /// an identifier for that window.
        /// </summary>
        public const int DefaultWindowID = 9843;

        /// <summary>
        /// The default crumb width.
        /// </summary>
        public const float DefaultCrumbWidth = 105f;

        /// <summary> Height of the crumb buttons. </summary>
        public const float ButtonHeight = 18.0f;

        /// <summary>
        /// Gets a rectangle to draw BreadcrumbUI within a given container 
        /// window. This will return the smallest rect in the bottom left
        /// that can contain the breadcrumb UI.
        /// </summary>
        /// <param name="container">The container window.</param>
        /// <param name="crumbs">Breadcrumbs to draw.</param>
        /// <returns>A rect that can be used to draw the breadcrumb UI.</returns>
        public static Rect GetRect(Rect container, Breadcrumbs crumbs)
        {
            return GetRect(container, crumbs, DefaultCrumbWidth);
        }

        /// <summary>
        /// Gets a rectangle to draw BreadcrumbUI within a given container 
        /// window. This will return the smallest rect in the bottom left
        /// that can contain the breadcrumb UI.
        /// </summary>
        /// <param name="container">The container window.</param>
        /// <param name="crumbs">Breadcrumbs to draw.</param>
        /// <param name="crumbWidth">Width of each breadcrumb button.</param>
        /// <returns>A rect that can be used to draw the breadcrumb UI.</returns>
        public static Rect GetRect(Rect container, Breadcrumbs crumbs, float crumbWidth)
        {
            int maxCrumbs = Mathf.FloorToInt(container.width / crumbWidth);
            int crumbCount = Math.Min(maxCrumbs, crumbs.Tracked.Length);
            Rect crumbRect = new Rect(
                0, container.height - ButtonHeight,
                container.width, ButtonHeight);
            crumbRect.width = crumbCount * crumbWidth;
            return crumbRect;
        }

        /// <summary>
        /// Draw the provided breadcrumbs at the bottom left of a container.
        /// The breadcrumbs are drawith with the <see cref="DefaultCrumbWidth"/>.
        /// </summary>
        /// <param name="container">GUI container for the breadcrumbs</param>
        /// <param name="breadcrumb">Breadcrumb navigation instance</param>
        /// <param name="onPressed">Fired when a breadcrumb button is pressed.</param>
        public static void Draw(Rect container, Breadcrumbs breadcrumb, Action<VignetteGraph> onPressed)
        {
            Draw(container, breadcrumb, DefaultCrumbWidth, onPressed);
        }

        /// <summary>
        /// Draw the provided breadcrumbs at the bottom left of a container.
        /// </summary>
        /// <param name="container">GUI container for the breadcrumbs</param>
        /// <param name="breadcrumb">Breadcrumb navigation instance</param>
        /// <param name="crumbWidth">Width of each breadcrumb button.</param>
        /// <param name="onPressed">Fired when a breadcrumb button is pressed.</param>
        public static void Draw(Rect container, Breadcrumbs breadcrumb, float crumbWidth, Action<VignetteGraph> onPressed)
        {
            var crumbs = new List<VignetteGraph>(breadcrumb.Tracked);
            if (breadcrumb.AtRoot || crumbs.Count == 0)
                return;
            
            float bcWidth = crumbWidth;
            float bcHeight = ButtonHeight;
            int maxCrumbs = Mathf.FloorToInt(container.width / bcWidth);
            int crumbCount = Math.Min(maxCrumbs, crumbs.Count);
            var crumbRect = new Rect
            {
                x = 0,
                y = container.height - bcHeight,
                width = bcWidth,
                height = bcHeight
            };

            for (int i = crumbCount - 1; i >= 0; i--)
            {
                VignetteGraph crumb = crumbs[i];
                GUIStyle style = i == 0 ? EditorStyles.miniButtonRight : EditorStyles.miniButtonMid;
                GUIUtility.GetControlID(FocusType.Passive, crumbRect);
                if (GUI.Button(crumbRect, crumb.name, style))
                {
                    breadcrumb.Unwind(i);
                    onPressed(crumb);
                }
                crumbRect.x += crumbRect.width;
            }
        }
    }
}