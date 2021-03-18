//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   10/03/2014
//-----------------------------------------------------------------------------

using UnityEngine;

namespace SG.Vignettitor.Graph.Config
{
    /// <summary>
    /// Defines the customizable visuals that can change how a graph is 
    /// rendered inside of a vignettitor.
    /// </summary>
    public class GraphVisualConfig : ScriptableObject
    {
        private static string DefaultPath = "Config/GraphVisualConfig";

        public float ZoomSensitivity = 0.01f;

        public float NodeHeight = 220;
        public float NodeWidth = 300;
        public float NodeXSpace = 150;
        public float NodeYSpace = 40;
        [Range(2, 100)]
        public int GridSpace = 20;

        public float MinGridSpacePercent = 0.12f;
        public Color GridColor = new Color32(70, 70, 70, 255);
        public float ConnectionWidth = 3;
        public Color32 ConnectionColor = new Color32(255, 255, 255, 255);
        public Color32 NewConnectionColor = new Color32(0, 255, 20, 255);
        public Color32 HardConnectionColor = new Color32(255, 0, 0, 255);
        public Color32 EditConnectionColor = new Color32(0, 255, 73, 255);
        public Color32 SelectedColor = new Color32(9, 236, 55, 184);
        public Color32 MarqueeColor = new Color32(21, 120, 234, 40);
        public Color32 MarqueeBorderColor = new Color32(21, 120, 234, 210);
        public Color32 LabelColor = new Color32(128, 128, 128, 142);
        public int SelectionBorderWidth = 10;
        public float MinZoom = 0.07f;
        public float MaxZoom = 2.0f;
        public Vector2 PasteOffset = new Vector2(40, 40);
        public bool SnapToGrid = false;
        public bool LinesOnTop = false;
        public Color32[] AnnotationColors = {
            new Color32(57, 168, 223, 60),
            new Color32(42, 81, 156, 60),
            new Color32(180, 180, 180, 60),
            new Color32(242, 76, 39, 60),
            new Color32(242, 144, 39, 60),
            new Color32(93, 161, 15, 60),
        };
        public Vector2 DefaultAnnotationSize = new Vector2(780, 400);

        public Rect BoundAnnotationDefaultOffsetRect = new Rect(40, 40, 160, 40);

        public Color32 TraceColor = new Color32(101, 249, 255, 255);
        public Color32 DebugColor = new Color32(255, 221, 96, 255);
        public Color32 WarningColor = new Color32(255, 185, 0, 255);
        public Color32 ErrorColor = new Color32(255, 24, 230, 255);
        public bool DrawInfo = false;
        public float SmallNudgeDistance = 5.0f;
        public float NormalNudgeDistance = 20.0f;
        public bool DrawNodeIcons = true;
        public AnimationCurve IconFadeCurve = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(0.146f, 1.0f), new Keyframe(0.19f, 0.2f), new Keyframe(1.0f, 0.2f));
        public float MinZoomForLabels = 0.13f;

        public static string GetConfigPath(System.Type graphEditorType)
        {
            return DefaultPath + "_" + graphEditorType.Name;
        }

        public static GraphVisualConfig GetConfig(System.Type graphEditorType)
        {
            GraphVisualConfig result = Resources.Load(GetConfigPath(graphEditorType), typeof(GraphVisualConfig)) as GraphVisualConfig;
            if (result != null)
                return result;
            return CreateInstance<GraphVisualConfig>();
        }

        public RectOffset BoundAnnotationDefaulOffset
        {
            get
            {
                return new RectOffset(
                    (int) BoundAnnotationDefaultOffsetRect.x, (int) BoundAnnotationDefaultOffsetRect.y,
                    (int) BoundAnnotationDefaultOffsetRect.width, (int) BoundAnnotationDefaultOffsetRect.height);
            }
        }
    }
}
