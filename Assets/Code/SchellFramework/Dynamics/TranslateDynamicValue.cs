// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 6/9/2016 4:22:37 PM
// ------------------------------------------------------------------------------

using System;
using SG.Core;
using UnityEngine;
using UnityEngine.Events;
using UObject = UnityEngine.Object;

namespace SG.Dynamics
{
    /// <summary>
    /// MonoBehaviour that extracts the value inside a DynamicValue (like from a Signal)
    /// and emits it as another basic UnityEvent with the inner type.
    /// </summary>
    public class TranslateDynamicValue : MonoBehaviour
    {
        [Serializable]
        public class UnityObjectEvent : UnityEvent<UObject> { }

        [Serializable]
        public class UnityVector2Event : UnityEvent<Vector2> { }

        [Serializable]
        public class UnityVector3Event : UnityEvent<Vector3> { }

        public UnityBoolEvent OnConvertBool;

        public UnityIntEvent OnConvertInt;

        public UnityFloatEvent OnConvertFloat;

        public UnityStringEvent OnConvertString;

        public UnityObjectEvent OnConvertUObject;

        public UnityVector2Event OnConvertVector2;

        public UnityVector3Event OnConvertVector3;

        public void ConvertDynamicValue(DynamicValue value)
        {
            UObject asUObject = value.Get() as UObject;
            if (asUObject != null)
            {
                OnConvertUObject.Invoke(value.Get() as UObject);
                return;
            }

            if (value.Type == typeof(bool))
                OnConvertBool.Invoke(value.Get<bool>());
            else if (value.Type == typeof(int))
                OnConvertInt.Invoke(value.Get<int>());
            else if (value.Type == typeof(float))
                OnConvertFloat.Invoke(value.Get<float>());
            else if (value.Type == typeof(string))
                OnConvertString.Invoke(value.Get<string>());
            else if (value.Type == typeof(Vector2))
                OnConvertVector2.Invoke(value.Get<Vector2>());
            else if (value.Type == typeof(Vector3))
                OnConvertVector2.Invoke(value.Get<Vector3>());
        }
    }
}
