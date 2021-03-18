//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   10/29/2014
//-----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace SG.Core.OnGUI
{
    /// <summary>
    /// A texture that is built in to a project's code as a base 64 encoded 
    /// string. This texture will be available for use in the game or editor 
    /// without requiring any additional assets.
    /// </summary>
    public class BuiltInTexture2D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuiltInTexture2D"/> class.
        /// </summary>
        /// <param name="base64EncodedTexture">The base64 encoded texture.</param>
        public BuiltInTexture2D(string base64EncodedTexture)
        {
            _base64EncodedTexture = base64EncodedTexture;
            CreateTexture();
        }

        private void CreateTexture()
        {
            _texture = new Texture2D(1, 1);
            _texture.LoadImage(Convert.FromBase64String(_base64EncodedTexture));
            BuiltInTexture2DUnloader.Create(_texture);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="BuiltInTexture2D"/> to <see cref="Texture2D"/>.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Texture2D(BuiltInTexture2D t)
        {
            if (t == null)
                return null;

            if (t._texture == null)
                t.CreateTexture();
            
            return t._texture; 
        }

        private Texture2D _texture;
        private readonly string _base64EncodedTexture;
    }
}
