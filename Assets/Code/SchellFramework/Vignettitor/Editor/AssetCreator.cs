using System.IO;
using SG.Core.OnGUI;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor
{
    public static class AssetCreator
    {
        public static readonly BuiltInTexture2D VignetteGraphIcon = new BuiltInTexture2D("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNvyMY98AAACQSURBVDhPpdLLEYAgDARQjx4tyDKogaaogV7owQIsIWYdmImZFfwc3iXjLmRwEpFf6PANOmRyzkmJk+jHXillRiDGeIEZDVgaXtX2qaCGdxVwXQScdLubDbPyBgX0ahrcRmEYFawsZPVWCAordEvosLElKHWHnAfRoFVLvj1jowXLrwLAdRFwnv3KPXT4nEwH4b7lodFkmfQAAAAASUVORK5CYII=");
        private const string GIZMOS_PATH = "Assets/Gizmos/";
        private const string ICON_PATH_END = " icon.png";

        public static void CreateGizmosDirectoryIfNeeded()
        {
            if (!Directory.Exists(GIZMOS_PATH))
                Directory.CreateDirectory(GIZMOS_PATH);
        }

        public static void CreateGraphIcon(string graphTypeName)
        {
            CreateGizmosDirectoryIfNeeded();
            string iconName = GIZMOS_PATH + graphTypeName + ICON_PATH_END;
            Texture2D t = VignetteGraphIcon;
            File.WriteAllBytes(iconName, t.EncodeToPNG());
            AssetDatabase.Refresh();
        }
    }
}
