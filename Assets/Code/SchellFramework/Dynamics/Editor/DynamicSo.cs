// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 4/26/2016 9:49:16 AM
// ------------------------------------------------------------------------------

using System;
using UnityEngine;

namespace SG.Dynamics
{
    public class DynamicSo<T> : DynamicSo
    {
        public T Value;

        public override object RawValue
        {
            get { return Value; }
            set { Value = (T)value; }
        }

        public override Type Type
        {
            get { return typeof(T); }
        }
    }

    public abstract class DynamicSo : ScriptableObject
    {
        public abstract Type Type { get; }
        public abstract object RawValue { get; set; }
    }
}
