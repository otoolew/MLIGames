//------------------------------------------------------------------------------
// Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
// Contact: Max Golden
//
// Created: 11/14/2016 4:06 PM
//------------------------------------------------------------------------------

using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace SG.Core.AssetIcons
{
    /// <summary>
    /// Utility for generating the Base64 encoding for an image from the right click menu
    /// </summary>
    /// <remarks>Perhaps this would be better as a menu bar option, but this is more convenient</remarks>
    public static class Base64StringGenerator
    {
        const int MAX_TEXTURE_WIDTH = 128;
        const int MAX_TEXTURE_HEIGHT = 128;
        const int CONSOLE_BUFFER_SIZE = 1 << 16;  // Size of the Unity Editor's console buffer

        private static readonly Notify Log = NotifyManager.GetInstance("Core.AssetIcons");

        /// <summary>
        /// If possible, generate the Base64 string from the selected image, save
        /// it to the clipboard, and output it into the console (if it can fit)
        /// </summary>
        [MenuItem("Assets/Generate Base64 String")]
        public static void GenerateBase64StringFromSprite()
        {
            Texture2D selectedTexture = Selection.activeObject as Texture2D;
            string objectName = Selection.activeObject.name;

            if (selectedTexture.width > MAX_TEXTURE_WIDTH || selectedTexture.height > MAX_TEXTURE_HEIGHT)
            {
                Log.Error("Cannot generate Base64 string for {0}: Image must be no larger than {1}x{2}",
                    objectName, MAX_TEXTURE_WIDTH, MAX_TEXTURE_HEIGHT);
                return;
            }

            // Duplicate the asset because we're going to be forcing some import settings
            string oldPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string fileName = Path.GetFileName(oldPath);
            string tempFileName = "tmp_" + fileName;
            string tempPath = oldPath.Replace(fileName, tempFileName);
            AssetDatabase.CopyAsset(oldPath, tempPath);

            TextureImporter importer = AssetImporter.GetAtPath(tempPath) as TextureImporter;
            importer.isReadable = true;
#if UNITY_5_5_OR_NEWER
            TextureImporterPlatformSettings platformSettings = importer.GetDefaultPlatformTextureSettings();
            platformSettings.format = TextureImporterFormat.RGBA32;
            importer.SetPlatformTextureSettings(platformSettings);
#else
            importer.textureFormat = TextureImporterFormat.RGBA32;
#endif
            importer.SaveAndReimport();

            Texture2D tempTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(tempPath);
            string base64Text = Texture2DToBase64(tempTexture);
            GUIUtility.systemCopyBuffer = base64Text;  // should be OS agnostic

            if (base64Text.Length > (CONSOLE_BUFFER_SIZE - objectName.Length - 89)) // 89 being the # of other chars in the message
            {
                Log.Debug("Base64 string for {0} copied to clipboard (too long to show in console)", objectName);
            }
            else
            {
                Log.Debug("Base64 string for {0} (copied to clipboard):\n--- BEGIN ---\n{1}\n---- END ----",
                    objectName, base64Text);
            }

            // Delete temporary file
            AssetDatabase.DeleteAsset(tempPath);
        }

        /// <summary>
        /// Validation function for GenerateBase64StringFromSprite
        /// </summary>
        /// <returns>True if the sected asset is a Texture2D and false otherwise</returns>
        [MenuItem("Assets/Generate Base64 String", true)]
        public static bool GenerateBase64StringFromSpriteValidation()
        {
            if (!Selection.activeObject)
                return false;

            return Selection.activeObject.GetType() == typeof(Texture2D);
        }

        private static string Texture2DToBase64(Texture2D texture)
        {
            return Convert.ToBase64String(texture.EncodeToPNG());
        }
    }
}