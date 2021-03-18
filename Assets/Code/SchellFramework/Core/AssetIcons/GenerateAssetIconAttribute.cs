//------------------------------------------------------------------------------
// Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
// Contact: Max Golden
//
// Created: 11/11/2016 10:14 AM
//------------------------------------------------------------------------------

using System;
using UnityEngine;
using SG.Core.OnGUI;

namespace SG.Core.AssetIcons
{
    /// <summary>
    /// Attribute you can attach to a class to create an icon in the Gizmos folder
    /// which will be used automatically by all assets in the project of the associated type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class GenerateAssetIconAttribute : Attribute
    {
        private static readonly Notify Log = NotifyManager.GetInstance("Core.AssetIcons");

        public Texture2D IconTexture { get; private set; }

        /// <summary>
        /// Deserialize the base64 image string to use as the icon and fill a given color with a new color
        /// </summary>
        /// <param name="newColorString">New color to use in the image</param>
        /// <param name="oldColorString">Old color to replace with the new color</param>
        /// <param name="base64Image">Base64 image string to use as the icon</param>
        public GenerateAssetIconAttribute(string newColorString, string oldColorString, string base64Image)
        {
            IconTexture = new BuiltInTexture2D(StripWhitespace(base64Image));

            Color newIconColor, oldIconColor;

            // The ColorUtility's parser expects hex strings to include a #, so we add one if the first parse fails
            bool hasNewIconColor = ColorUtility.TryParseHtmlString(newColorString, out newIconColor);
            if (!hasNewIconColor)
                hasNewIconColor = ColorUtility.TryParseHtmlString("#" + newColorString, out newIconColor);

            bool hasOldIconColor = ColorUtility.TryParseHtmlString(oldColorString, out oldIconColor);
            if (!hasOldIconColor)
                hasOldIconColor = ColorUtility.TryParseHtmlString("#" + oldColorString, out oldIconColor);

            if (hasNewIconColor && hasOldIconColor)
            {
                for (int y = 0; y < IconTexture.height; y++)
                {
                    for (int x = 0; x < IconTexture.width; x++)
                    {
                        Color currentPixelColor = IconTexture.GetPixel(x, y);
                        float dif = ((Vector4) (currentPixelColor - oldIconColor)).magnitude;
                        Color newPixelColor = dif > 0.1f ? 
                            currentPixelColor : newIconColor;
                        IconTexture.SetPixel(x, y, newPixelColor);
                    }
                }
                IconTexture.Apply();
            }
            else
            {
                if (!hasNewIconColor)
                    Log.Error("Could not parse new icon color: {0}", newColorString);

                if (!hasOldIconColor)
                    Log.Error("Could not parse old icon color: {1}", oldColorString);
            }
        }

        /// <summary>
        /// Deserialize the base64 image string to use as the icon
        /// </summary>
        /// <param name="base64Image">Base64 image string to use as the icon</param>
        public GenerateAssetIconAttribute(string base64Image)
        {
            IconTexture = new BuiltInTexture2D(StripWhitespace(base64Image));
        }

        /// <summary>
        /// Create the SG default asset icon for this type
        /// </summary>
        public GenerateAssetIconAttribute()
        {
            IconTexture = new BuiltInTexture2D(CoreIcons.DEFAULT_ICON_STRING);
        }

        /// <summary>
        /// Remove newline, tab, and space characters from the given string
        /// </summary>
        /// <param name="str">String from which to strip all whitespace</param>
        /// <returns>The string passed in without any newline, tab, or space characters</returns>
        public static string StripWhitespace(string str)
        {
            // Remove whitespace just in case you don't want to have a file that scrolls to the right ~forever~
            return str.Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\t", string.Empty)
                .Replace(" ", string.Empty);
        }
    }
}