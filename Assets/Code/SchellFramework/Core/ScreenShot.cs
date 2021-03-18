//------------------------------------------------------------------------------
// Copyright © 2016 Schell Games, LLC. All Rights Reserved.
// Contact: William Roberts
//
// Created: 01/21/2016
//------------------------------------------------------------------------------

using System.IO;
using UnityEngine;

namespace SG.Core
{
    /// <summary>
    /// Provides helper functionality for capturing a screenshot at the highest quality possible.
    /// 
    /// Issue(s):
    /// Unity currently does not allow the rendering of the UI Canvas object to a render texture whenever it 
    /// is set to the screen space mode. It does not matter if the canvas is set to either "Screen Space - Overlay"
    /// or "Screen Space - Camera". Neither option can be rendered to a render texture do to the way th screen space
    /// rendering process works currently. Note that rendering a worldspace Canvas will work with current versions
    /// of Unity. Although there was a bug for a period of time that prevented world space canvases from rendering as
    /// expected. It seems okay in versions newer then 5.2.3p3. I have not tested in a version older then that. 
    /// 
    /// Reference about the issue:
    /// + http://forum.unity3d.com/threads/capture-screenshots-which-include-the-ui.273862/
    /// + http://forum.unity3d.com/threads/issue-screenshots-of-gui-elements-with-rendertexture.278693/
    /// + http://forum.unity3d.com/threads/canvas-render-to-texture-problems.284549/
    /// </summary>
    public class ScreenShot
    {
        /// <summary>
        /// Captures a screenshot using the scenes main camera.
        /// </summary>
        /// <param name="width">Width of the outputted image.</param>
        /// <param name="height">Height of the outputted image.</param>
        /// <param name="filePath">Name and location of the outtputted image.</param>
        public static void Capture(int width, int height, string filePath)
        {
            Capture(Camera.main, width, height, filePath);
        }


        /// <summary>
        /// Captures a screenshot using the specified camera.
        /// </summary>
        /// <param name="viewpoint">The camera to capture the image from.</param>
        /// <param name="width">Width of the outputted image.</param>
        /// <param name="height">Height of the outputted image.</param>
        /// <param name="filePath">Name and location of the outtputted image.</param>
        public static void Capture(Camera viewpoint, int width, int height, string filePath)
        {
            Texture2D screenshot = null;

            try
            {
                screenshot = Capture(viewpoint, width, height, false);

                byte[] data = screenshot.EncodeToPNG();

                File.WriteAllBytes(filePath, data);
            }
            finally
            {
                if(screenshot != null)
                    Object.Destroy(screenshot);
            }
        }


        /// <summary>
        /// Captures a screenshot from the point of view of the specified camera.
        /// </summary>
        /// <param name="viewpoint">The camera to capture the screenshot from.</param>
        /// <param name="width">Height of the texture to generate.</param>
        /// <param name="height">Width of the texture to generate.</param>
        /// <param name="useAlphaChannel">
        /// Determines if the outputted texture will contain an alpha channel out not. Note that if this is enabled,
        /// you may not see pixels in the skybox areas if the clear color or skybox pixels where written with an
        /// alpha value less then 255. The pixel data will be the, you just not may be able to see the color without
        /// removing the alpha channel. In some cases, this may be desirable if you want to crop out the skybox or background
        /// color to just capture a model or character.
        /// </param>
        /// <returns>
        /// A new texture 2D containing the screen shot. Be sure to destroy (ie: Object.Destory(...)this object
        /// when it is not longer needed. Otherwise, you will leak memory.
        /// </returns>
        public static Texture2D Capture(Camera viewpoint, int width, int height, bool useAlphaChannel)
        {
            if (viewpoint == null)
                throw new System.ArgumentNullException("viewpoint", "The specified camera was a NULL value! A valid camera reference must be passed to capture a screenshot.");

            RenderTexture renderTexture = null;
            Texture2D screenShot = null;
            GameObject go = null;
            RenderTexture previous = RenderTexture.active;

            try
            {
                renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 8); // Max out the anti aliasing :)

                // Create a new camera for use to use. This allows us to render without any stereo settings enabled, and
                // allows us to have a different setup without needing to worry about restoring the games camera.
                go = new GameObject();
                go.transform.position = viewpoint.gameObject.transform.position;
                go.transform.rotation = viewpoint.gameObject.transform.rotation;

                Camera captureCamera = go.AddComponent<Camera>();
                captureCamera.enabled = false; // Camera rendering will be manually triggered.

                // Clone the cameras rendering settings so that we duplicate the view.
                captureCamera.clearFlags = viewpoint.clearFlags;
                captureCamera.backgroundColor = viewpoint.backgroundColor;
                captureCamera.nearClipPlane = viewpoint.nearClipPlane;
                captureCamera.farClipPlane = viewpoint.farClipPlane;
                captureCamera.cullingMask = viewpoint.cullingMask;
                captureCamera.transparencySortMode = viewpoint.transparencySortMode;
                captureCamera.renderingPath = viewpoint.renderingPath;
                captureCamera.useOcclusionCulling = viewpoint.useOcclusionCulling;
                captureCamera.clearStencilAfterLightingPass = viewpoint.clearStencilAfterLightingPass;
        
                // Note: Do not apply any of the stereo settings since we want to capture the screenshot at a
                // high resolution without seeing the two eyes, etc.

                // Render the current frame to the render texture.
                captureCamera.SetTargetBuffers(renderTexture.colorBuffer, renderTexture.depthBuffer);
                captureCamera.targetTexture = renderTexture;
                captureCamera.Render();

                // Do we want the texture to conatin an alpha channel or not.
                TextureFormat outputFormat = useAlphaChannel ? TextureFormat.RGBA32 : TextureFormat.RGB24;

                // Copy the contents of the render texture to a new Texture2D object so that we can further manipulate it.
                RenderTexture.active = renderTexture;
                screenShot = new Texture2D(renderTexture.width, renderTexture.height, outputFormat, false);
                screenShot.ReadPixels(new Rect(0, 0, screenShot.width, screenShot.height), 0, 0);

                return screenShot;
            }
            finally
            {
                // Ensure the previous render texture is restored to prevent any graphic anomalies that may occur
                // if the wrong render texture is set.
                RenderTexture.active = previous;

                // Clean up the tempory objects that were created.
                if (go != null)
                    Object.Destroy(go);

                if (renderTexture != null)
                    RenderTexture.ReleaseTemporary(renderTexture);
            }
        }
    }
}
