//------------------------------------------------------------------------------
// Copyright © 2016 Schell Games, LLC. All Rights Reserved.
// Contact: William Roberts
// Created: 06/13/2016 3:51:00 PM
//------------------------------------------------------------------------------

using System;
using UnityEngine;

namespace SG.Core
{
    /// <summary>
    /// Represents a single Unity layer.
    /// </summary>
    [Serializable]
    public struct Layer
    {
        [SerializeField]
        private int _layerIndex;


        public Layer(int layerIndex)
        {
            ValidateIndex(layerIndex);
            _layerIndex = layerIndex;
        }

        /// <summary>
        /// The index of the layer.
        /// </summary>
        public int Index
        {
            get { return _layerIndex; }
            set
            {
                ValidateIndex(value);
                _layerIndex = value;
            }
        }


        /// <summary>
        /// Gets the name of the layer that this layer represents.
        /// </summary>
        public string Name
        {
            get { return LayerMask.LayerToName(_layerIndex); }
        }


        /// <summary>
        /// Gets a bit mask for this layer.
        /// </summary>
        public int Mask
        {
            get { return 1 << _layerIndex;  }
        }


        public static implicit operator int (Layer layer)
        {
            return layer._layerIndex;
        }


        public static implicit operator Layer(int layerIndex)
        {
            return new Layer(layerIndex);
        }


        private static void ValidateIndex(int index)
        {
            if (index < 0 || index > 32)
            {
                throw new IndexOutOfRangeException(
                    string.Format("Invalid layer index '{0}' must be a value between 0 and 32!", index)
                );
            }
        }
    }
}
