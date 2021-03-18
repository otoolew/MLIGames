//-----------------------------------------------------------------------------
//  Copyright © 2012 Schell Games, LLC. All Rights Reserved. 
//
//  Authors: Ryan Hipple, Howard Kim
//  Date:   06/13/2012
//-----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace SG.Core.OnGUI
{
    /// <summary>
    /// State used to tack scroll and filter string for a filtered scrolled 
    /// button list.
    /// </summary>
    public struct FilteredButtonListState
    {
        public string filterText;
        public Vector2 scroll;
    }

    /// <summary>
    /// Provides a set of functions to perform common OnGUI actions.
    /// </summary>
    public static class OnGUIUtils
    {
        #region -- Constants --------------------------------------------------

        public static float BUTTON_HEIGHT
        {
            get
            {
#if UNITY_IPHONE
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                    return 60f;
#endif
                return 32f;

            }
        }

        public static int FONT_SIZE
        {
            get
            {
#if UNITY_IPHONE
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                    return 24;
#endif
                return 0;
            }
        }

        public static float HEADER_BUTTON_SPACE
        {
            get
            {
#if UNITY_IPHONE
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                    return 64f;
#endif
                return 32f;

            }
        }

        public static float COLLAPSED_WINDOW_HEIGHT
        {
            get
            {
#if UNITY_IPHONE
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                    return 64f;
#endif
                return 50f;

            }
        }

        #endregion -- Constants -----------------------------------------------

        #region -- Styles -----------------------------------------------------

        private static GUIStyle leftTextButton;

        public static GUIStyle LeftTextButton
        {
            get
            {
                if (leftTextButton == null)
                {
                    leftTextButton = new GUIStyle(GUI.skin.button);
                    leftTextButton.alignment = TextAnchor.MiddleLeft;
                }
                return leftTextButton;
            }
        }

        public static readonly GUIStyle BlackTextStyle = new GUIStyle
        {
            fontSize = FONT_SIZE,
            normal = new GUIStyleState { textColor = Color.black },
            hover = new GUIStyleState { textColor = Color.black }
        };

        public static readonly GUIStyle CenteredBlackTextStyle = new GUIStyle
        {
            fontSize = FONT_SIZE,
            normal = new GUIStyleState { textColor = Color.black },
            hover = new GUIStyleState { textColor = Color.black },
            alignment = TextAnchor.MiddleCenter
        };

        public static readonly GUIStyle CenteredBlackWrappedTextStyle = new GUIStyle
        {
            fontSize = FONT_SIZE,
            normal = new GUIStyleState { textColor = Color.black },
            hover = new GUIStyleState { textColor = Color.black },
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };

        public static readonly GUIStyle LeftAlignedWrappedLabel = new GUIStyle
        {
            alignment = TextAnchor.UpperLeft,
            normal = new GUIStyleState { textColor = Color.black },
            wordWrap = true,
            padding = new RectOffset(4, 4, 4, 4),
            clipping = TextClipping.Clip
        };

        public static GUIStyle TextStyle = new GUIStyle
        {
            fontSize = FONT_SIZE,
            normal = new GUIStyleState { background = null },
            alignment = TextAnchor.UpperCenter,
            wordWrap = true,
            clipping = TextClipping.Clip,
        };

        public static GUIStyle TextStyleCenter = new GUIStyle
        {
            fontSize = FONT_SIZE,
            normal = new GUIStyleState { background = null },
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };

        public static readonly GUIStyle RedTextStyle = new GUIStyle
        {
            fontSize = FONT_SIZE,
            normal = new GUIStyleState { textColor = Color.red },
            hover = new GUIStyleState { textColor = Color.red }
        };

        public static readonly GUIStyle MagentaTextStyle = new GUIStyle
        {
            fontSize = FONT_SIZE,
            normal = new GUIStyleState { textColor = Color.magenta },
            hover = new GUIStyleState { textColor = Color.magenta }
        };

        public static readonly GUIStyle YellowTextStyle = new GUIStyle
        {
            fontSize = FONT_SIZE,
            normal = new GUIStyleState { textColor = Color.yellow },
            hover = new GUIStyleState { textColor = Color.yellow }
        };

        public static readonly GUIStyle GrayTextStyle = new GUIStyle
        {
            fontSize = FONT_SIZE,
            normal = new GUIStyleState { textColor = Color.gray },
            hover = new GUIStyleState { textColor = Color.gray }
        };

        private static GUIStyle buttonStyleLeft;
        public static GUIStyle ButtonStyleLeft
        {
            get
            {
                if (buttonStyleLeft == null)
                {
                    buttonStyleLeft = new GUIStyle(GUI.skin.button);
                    buttonStyleLeft.alignment = TextAnchor.UpperLeft;
                    buttonStyleLeft.wordWrap = true;
                    buttonStyleLeft.clipping = TextClipping.Clip;
                    buttonStyleLeft.fontSize = FONT_SIZE;
                }
                return buttonStyleLeft;
            }
        }

        private static GUIStyle textFieldStyleCenter;
        public static GUIStyle TextFieldStyleCenter
        {
            get
            {
                if (textFieldStyleCenter == null)
                {
                    textFieldStyleCenter = new GUIStyle(GUI.skin.textField);
                    textFieldStyleCenter.alignment = TextAnchor.MiddleCenter;
                    textFieldStyleCenter.wordWrap = true;
                    textFieldStyleCenter.fontSize = FONT_SIZE;
                }
                return textFieldStyleCenter;
            }
        }

        private static Texture2D boxTex = null;
        private static GUIStyle boxStyle;
        public static GUIStyle BoxStyle
        {
            get
            {
                if (boxTex == null)
                {
                    boxTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                    boxTex.filterMode = FilterMode.Point;
                    //Color border = Color.black;
                    Color center = Color.white;
                    boxTex.SetPixels(new Color[] { 
                        center
                    });
                    boxTex.Apply();
                    boxStyle = new GUIStyle();
                    boxStyle.normal.background = boxTex;
                    //boxStyle.border = new RectOffset(2, 2, 2, 2);
                    boxStyle.alignment = TextAnchor.UpperCenter;
                }
                return boxStyle;
            }
        }
        private static Texture2D borderTex = null;
        private static GUIStyle borderStyle;
        public static GUIStyle BorderStyle
        {
            get
            {
                if (borderTex == null)
                {
                    borderTex = new Texture2D(3, 3, TextureFormat.RGBA32, false);
                    borderTex.filterMode = FilterMode.Point;
                    Color border = Color.white;
                    Color center = Color.clear;
                    borderTex.SetPixels(new Color[] { 
                        border, border, border,
                        border, center, border,
                        border, border, border
                    });
                    borderTex.Apply();
                    borderStyle = new GUIStyle();
                    borderStyle.normal.background = borderTex;
                    borderStyle.border = new RectOffset(2, 2, 2, 2);
                    borderStyle.alignment = TextAnchor.UpperCenter;
                }
                return borderStyle;
            }
        }
        private static Texture2D circleTex = null;
        private static GUIStyle CircleStyle;
        #endregion -- Styles --------------------------------------------------

        private static void GenerateCircle(int size)
        {
            if (circleTex == null)
            {
                Color32[] colors = new Color32[size * size];
                int current = 0;
                circleTex = new Texture2D(size, size, TextureFormat.ARGB32, false);
                circleTex.filterMode = FilterMode.Point;
                int cx = size / 2;
                int cy = size / 2;
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        int dx = cx - x;
                        int dy = cy - y;
                        Color32 c = new Color32((byte)0, (byte)0, (byte)0, (byte)0);
                        if (Mathf.Sqrt((dx * dx) + (dy * dy)) < size / 2)
                            c = new Color32((byte)255, (byte)255, (byte)255, (byte)255);

                        colors[current] = c;
                        current++;
                    }
                }
                circleTex.SetPixels32(colors);
                circleTex.Apply();
                CircleStyle = new GUIStyle();
                CircleStyle.normal.background = circleTex;
                CircleStyle.fixedWidth = size;
                CircleStyle.hover.background = circleTex;
                CircleStyle.alignment = TextAnchor.MiddleCenter;
            }
        }

        /// <summary>Draws a 32 x 32 dot on the screen.</summary>
        /// <param name="pos">Center of the dot.</param>
        /// <param name="content">TExt to draw inside the dot.</param>
        /// <param name="fillColor">Color of the dot.</param>
        /// <param name="inputPoint">
        /// If true, the position will be treated like it is in input space 
        /// instead of GUI space.
        /// </param>
        public static void DrawDot(Vector2 pos, string content, Color fillColor, bool inputPoint)
        {
            Rect r = inputPoint ? GetRectAroundInputPoint(pos, 32) :
                GetRectAroundPoint(pos, 32, 32);
            GenerateCircle(32);
            Color old = GUI.color;
            GUI.color = fillColor;
            GUI.Box(r, "", CircleStyle);
            GUI.color = old;
            GUI.Box(r, content, CenteredBlackTextStyle);
        }

        /// <summary>Draws a rectangle representing the given rect.</summary>
        /// <param name="r">The rectangle to draw.</param>
        /// <param name="content">The text to display in the box.</param>
        /// <param name="fillColor">
        /// The fill color for the box. This may be Color.clear.
        /// </param>
        /// <param name="borderColor">
        /// The border color for the box. This may be Color.clear.
        /// </param>
        public static void DrawBox(Rect r, string content, Color fillColor, Color borderColor, GUIStyle textStyle)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (content == null) content = string.Empty;
                Color old = GUI.color;
                GUI.color = fillColor;
                GUI.Box(r, "", OnGUIUtils.BoxStyle);
                GUI.color = borderColor;
                GUI.Box(r, "", OnGUIUtils.BorderStyle);
                GUI.color = old;
                GUI.Box(r, content, textStyle);
            }
        }

        public static void DrawBox(Rect r, string content, Color fillColor, Color borderColor)
        {
            DrawBox(r, content, fillColor, borderColor, TextStyle);
        }

        public static bool SwitchButton(bool current, string text, params GUILayoutOption[] layout)
        {
            bool newValue = GUILayout.Toggle(current, text, GUI.skin.button, layout);
            if (newValue && !current)
                return true;
            return false;
        }

        public static Color EditorProSelectedPurple
        {
            get { return new Color(0.2980392156862745f, 0.4941176470588235f, 1f); }
        }

        public static Color EditorProLightGrey
        {
            get { return new Color(0.34765625f, 0.34765625f, 0.34765625f); }
        }

        public static GUIContent[] TempContent(string[] texts)
        {
            GUIContent[] guiContentArray = new GUIContent[texts.Length];
            for (int index = 0; index < texts.Length; ++index)
                guiContentArray[index] = new GUIContent(texts[index]);
            return guiContentArray;
        }

        #region -- Compound Elements ------------------------------------------

        /// <summary>
        /// Draw a scrollable, string-filtered list of buttons. Only buttons 
        /// containing (case-insensitive) the filter string will be drawn.
        /// </summary>
        /// <param name="state">Scroll and filter string state.</param>
        /// <param name="items">Items to draw.</param>
        /// <param name="onSelected">Function to call on selection.</param>
        /// <param name="boxID">
        /// Optional name for the filter text box so that it may be focused.
        /// </param>
        /// <returns>The gui state that should be passed back in.</returns>
        public static FilteredButtonListState FilteredButtonList(FilteredButtonListState state, string[] items, Action<int> onSelected, string boxID = null)
        {            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Filter: ", GUILayout.ExpandWidth(false));
            if (boxID != null)
            {
                GUI.FocusControl(boxID);
                GUI.SetNextControlName(boxID);
            }
            state.filterText = GUILayout.TextField(state.filterText ?? "");
            if (GUILayout.Button("x", GUI.skin.label, GUILayout.ExpandWidth(false)))
                state.filterText = "";
            GUILayout.EndHorizontal();

            state.scroll = GUILayout.BeginScrollView(state.scroll);
            for (int i = 0; i < items.Length; i++)
            {
                if (string.IsNullOrEmpty(state.filterText) ||
                    items[i].ToLower().Contains(state.filterText.ToLower()))
                {
                    if (GUILayout.Button(items[i], ButtonStyleLeft))
                        onSelected(i);
                }
            }
            GUILayout.EndScrollView();
            return state;
        }
     
        /// <summary>
        /// Draws a label, text entry, and slider for setting a float value.
        /// </summary>
        /// <param name="currentValue">
        /// The current value, returned from the last call.
        /// </param>
        /// <param name="min">The minumum value on the slider.</param>
        /// <param name="max">The maximum value on the slider.</param>
        /// <param name="inc">Increment size for the slider.</param>
        /// <param name="label">Text to show on the label.</param>
        /// <returns>A new value that can be passed in next call.</returns>
        public static float LabeledSlider(float currentValue, float min, float max, float inc, string label)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(label + ":", BlackTextStyle);
            try
            {
                string currentString = currentValue.ToString();
                if (!currentString.Contains("."))
                    currentString += ".0";
                string stringVal = GUILayout.TextField(currentString);
                if (stringVal.EndsWith("."))
                    stringVal += "0";
                currentValue = float.Parse(stringVal);
            }
            catch (System.Exception) { }

            if (GUI.changed)
            {
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                return currentValue;
            }

            GUILayout.EndHorizontal();

            inc = 1.0f / inc;
            float sliderValue = (Mathf.Round(GUILayout.HorizontalSlider(currentValue, min, max) * inc)) / inc;
            if (GUI.changed)
                currentValue = sliderValue;

            GUILayout.EndVertical();
            return currentValue;
        }

        /// <summary>
        /// Draws a label, text entry, and slider for setting a float value.
        /// </summary>
        /// <param name="currentValue">
        /// The current value, returned from the last call.
        /// </param>
        /// <param name="min">The minumum value on the slider.</param>
        /// <param name="max">The maximum value on the slider.</param>
        /// <param name="inc">Increment size for the slider.</param>
        /// <param name="label">Text to show on the label.</param>
        /// <returns>A new value that can be passed in next call.</returns>
        public static float LabeledInlineSlider(float currentValue, float min, float max, float inc, string label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label + ":", BlackTextStyle, GUILayout.Width(100.0f));
            try
            {
                string currentString = currentValue.ToString();
                if (!currentString.Contains("."))
                    currentString += ".0";
                string stringVal = GUILayout.TextField(currentString, GUILayout.Width(45.0f));
                if (stringVal.EndsWith("."))
                    stringVal += "0";
                currentValue = float.Parse(stringVal);
            }
            catch (System.Exception) { }

            if (GUI.changed)
            {
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                return currentValue;
            }

            inc = 1.0f / inc;
            float sliderValue = (Mathf.Round(GUILayout.HorizontalSlider(currentValue, min, max) * inc)) / inc;
            if (GUI.changed)
                currentValue = sliderValue;

            GUILayout.EndHorizontal();
            return currentValue;
        }
        #endregion -- Compound Elements ---------------------------------------

        #region -- Line Drawing -----------------------------------------------
        /// <summary>
        /// A cached material used to draw GUI lines. This will be created on 
        /// demand.
        /// </summary>
        private static Material lineMaterial;

        /// <summary>
        /// Draws a one pixel line in screen space.
        /// </summary>
        /// <param name="start">
        /// The first point for the line in screen space.
        /// </param>
        /// <param name="end">
        /// The second point for the line in screen space.
        /// </param>
        /// <param name="color">The color to draw the line.</param>
        public static void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            if (!lineMaterial)
            {
                lineMaterial = new Material(Shader.Find("Sprites/Default"));
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
            }
            lineMaterial.color = color;
            lineMaterial.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(end.x, end.y, 0);
            GL.End();
        }

        public static void DrawLineNoMat(Vector2 start, Vector2 end, Color color)
        {
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(end.x, end.y, 0);
            GL.End();
        }

        public static void DrawLine(Vector2 start, Vector2 end, Color color, Rect constraint)
        {
            start = GUIUtility.GUIToScreenPoint(start);
            end = GUIUtility.GUIToScreenPoint(end);

            if (!ConstrainLineSegToRect(ref start, ref end, constraint))
                return;

            start.y = Screen.height - start.y;
            end.y = Screen.height - end.y;

            //Vector3 s = new Vector3(start.x, start.y, 0);
            //Vector3 e = new Vector3(end.x, end.y, 0);

            GL.PushMatrix();
            GL.LoadPixelMatrix();
            DrawLine(start, end, color);
            GL.PopMatrix();
        }

        /// <summary>
        /// Adjusts the points of a line segment to be contained within a 
        /// rectangle. If this returns true, the start and end points will 
        /// represent the original line, constrained to the given rectangle.
        /// </summary>
        /// <param name="start">Line segment start point.</param>
        /// <param name="end">Line segment end point.</param>
        /// <param name="rect">Constraining rectangle.</param>
        /// <returns>
        /// True if any part of the line segment lies within the rectangle.
        /// </returns>
        public static bool ConstrainLineSegToRect(ref Vector2 start, ref Vector2 end, Rect rect)
        {
            bool intersects = false;
            bool startInside = rect.Contains(start);
            bool endInside = rect.Contains(end);

            if (startInside && endInside)
                return true;

            Vector2 tl = new Vector2(rect.xMin, rect.yMin);
            Vector2 tr = new Vector2(rect.xMax, rect.yMin);
            Vector2 bl = new Vector2(rect.xMin, rect.yMax);
            Vector2 br = new Vector2(rect.xMax, rect.yMax);

            Vector2 right = Vector2.zero;
            if (LineSegIntersect(start, end, tr, br, out right))
            {
                intersects = true;
                if (start.x > rect.xMax) start = right;
                else if (end.x > rect.xMax) end = right;
            }

            Vector2 left = Vector2.zero;
            if (LineSegIntersect(start, end, tl, bl, out left))
            {
                intersects = true;
                if (start.x < rect.xMin) start = left;
                else if (end.x < rect.xMin) end = left;
            }

            Vector2 top = Vector2.zero;
            if (LineSegIntersect(start, end, tl, tr, out top))
            {
                intersects = true;
                if (start.y < rect.yMin) start = top;
                else if (end.y < rect.yMin) end = top;
            }

            Vector2 bottom = Vector2.zero;
            if (LineSegIntersect(start, end, bl, br, out bottom))
            {
                intersects = true;
                if (start.y > rect.yMax) start = bottom;
                else if (end.y > rect.yMax) end = bottom;
            }

            if (!intersects)
                return false;

            return true;
        }

        /// <summary>
        /// Finds the intersection point between two line segments.
        /// </summary>
        /// <param name="aStart">Line segment A's start point.</param>
        /// <param name="aEnd">Line segment A's end point.</param>
        /// <param name="bStart">Line segment B's start point.</param>
        /// <param name="bEnd">Line segment B's end point.</param>
        /// <param name="intersection">
        /// Fills in an intersenction point if true is returned.
        /// </param>
        /// <returns>True if the line segments intersect.</returns>
        public static bool LineSegIntersect(Vector2 aStart, Vector2 aEnd,
            Vector2 bStart, Vector2 bEnd, out Vector2 intersection)
        {
            intersection = Vector2.zero;
            Vector2 aDif = aEnd - aStart;
            Vector2 bDif = bEnd - bStart;
            float aDotbPerp = aDif.x * bDif.y - aDif.y * bDif.x;
            if (aDotbPerp == 0)
                return false;
            Vector2 c = bStart - aStart;
            float t = (c.x * bDif.y - c.y * bDif.x) / aDotbPerp;
            if (t < 0 || t > 1)
                return false;
            float u = (c.x * aDif.y - c.y * aDif.x) / aDotbPerp;
            if (u < 0 || u > 1)
                return false;
            intersection = aStart + t * aDif;
            return true;
        }
        #endregion -- Line Drawing --------------------------------------------

        #region -- Rect Generation --------------------------------------------
        /// <summary>
        /// Returns a new square rect centered around the given point from the 
        /// input system. The input point will have its y coord inverted to 
        /// account for the mismatch in input and gui spaces.
        /// </summary>
        /// <param name="point">
        /// The input-relative point (0,0 in upper left) to center the rect 
        /// around.
        /// </param>
        /// <param name="size">The width and height of the rect.</param>
        /// <returns>A square rect centered around the point.</returns>
        public static Rect GetRectAroundInputPoint(Vector2 point, float size)
        {
            float halfWidth = size / 2.0f;
            return new Rect(point.x - halfWidth,
                Screen.height - point.y - halfWidth,
                size, size);
        }

        /// <summary>
        /// Returns a new square rect centered around the given point.
        /// </summary>
        /// <param name="point">
        /// The screen-relative point to center the rect around.
        /// </param>
        /// <param name="size">The width and height of the rect.</param>
        /// <returns>A square rect centered around the point.</returns>
        public static Rect GetRectAroundPoint(Vector2 point, float width, float height)
        {
            float halfWidth = width / 2.0f;
            float halfHeight = height / 2.0f;
            return new Rect(point.x - halfWidth, point.y - halfHeight,
                width, height);
        }

        /// <summary>
        /// Returns a new square rect centered around the given point from the 
        /// input system. The input point will have its y coord inverted to 
        /// account for the mismatch in input and gui spaces.
        /// </summary>
        /// <param name="point">
        /// The input-relative point (0,0 in upper left) to center the rect 
        /// around.
        /// </param>
        /// <returns>A square rect centered around the point.</returns>
        public static Rect GetRectAroundInputPoint(Vector2 point)
        {
            return GetRectAroundInputPoint(point, BUTTON_HEIGHT);
        }

        /// <summary>
        /// Returns a rect of the given size centered on the screen.
        /// </summary>
        public static Rect GetCenteredRect(float width, float height)
        {
            return GetRectAroundPoint(new Vector2(Screen.width / 2, Screen.height / 2), width, height);
        }
        #endregion -- Rect Generation -----------------------------------------        
    }
}
