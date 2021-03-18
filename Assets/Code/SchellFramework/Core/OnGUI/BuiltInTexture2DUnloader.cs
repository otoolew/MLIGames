// ----------------------------------------------------------------------------
//  Copyright © 2017 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   01/25/2017
// ----------------------------------------------------------------------------

using UnityEngine;

namespace SG.Core.OnGUI
{
    /// <summary>
    /// BuiltInTexture2D allocates a texture in memory when it is created. When 
    /// code is recompiled, this texture remains in memory. It is not possible 
    /// for a destructor to unload the texture since Unity can not unload 
    /// objects at that time.
    /// 
    /// BuiltInTexture2DUnloader provides a solution to this problem. 
    /// ScriptableObjects are saved in memory before a code reload and then 
    /// deserialized once it is complete. BuiltInTexture2DUnloader is used to 
    /// store a reference to a texture to unload
    /// </summary>
    public class BuiltInTexture2DUnloader : ScriptableObject
    {
        public Texture2D Texture;

        /// <summary>
        /// If in the editor, this will create a BuiltInTexture2DUnloader in
        /// memory that stores a reference to the texture so that it may be
        /// unloaded the next time code is reloaded.
        /// </summary>
        /// <param name="t">Texture to unload on the next reload/</param>
        public static void Create(Texture2D t)
        {
#if UNITY_EDITOR
            BuiltInTexture2DUnloader unloader = CreateInstance<BuiltInTexture2DUnloader>();
            unloader.Texture = t;
#endif
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (Texture != null)
            {
                DestroyImmediate(Texture);
                DestroyImmediate(this);

            }
#endif
        }
    }
}