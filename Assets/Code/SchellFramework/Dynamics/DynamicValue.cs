// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 4/27/2016 4:15:06 PM
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SG.Core;
using UnityEngine;
using UnityEngine.Events;
using UObject = UnityEngine.Object;

namespace SG.Dynamics
{
    [Serializable]
    public class DynamicValue
    {
        /// <summary>
        /// To reduce allocations from MonoType.get_AssemblyQualifiedName() ~ 200 bytes / call
        /// </summary>
        private static readonly Dictionary<Type, string> _aqnLookup = new Dictionary<Type, string>();

        [SerializeField]
        [DynamicType]
        private string _type;

        private Type _processedType;
        public Type Type
        {
            get { return _processedType ?? (_processedType = string.IsNullOrEmpty(_type) ? null : Type.GetType(_type)); }
        }

        [SerializeField]
        private string _string;

        [SerializeField]
        private bool _bool;

        [SerializeField]
        private int _int;

        [SerializeField]
        private float _float;

        [SerializeField]
        private UObject _object;

        /// <summary>
        /// Do not allow the type of this object to be set from the editor.
        /// </summary>
        [SerializeField]
#pragma warning disable 414 // Read by the DynamicPropertyDrawer
        private bool _lockType;
#pragma warning restore 414

        /// <summary>
        /// Temp holder for value to prevent the need to constantly deserialize.
        /// </summary>
        private object _deserializedObject;

        public DynamicValue() { }

        public DynamicValue(object value)
        {
            Set(value);
        }

        public void Set(object value)
        {
            Type typeOfValue = value != null ? value.GetType() : typeof(object);
            if (typeOfValue != _processedType)
            {
                _processedType = typeOfValue;
                if (!_aqnLookup.TryGetValue(typeOfValue, out _type))
                {
                    _type = typeOfValue.AssemblyQualifiedName;
                    _aqnLookup[typeOfValue] = _type; // cache name to avoid alloc
                }
            }

            _deserializedObject = value;

            // Maybe clear out the values of the other sections?
            TypeCode typeCodeOfValue = Type.GetTypeCode(_processedType);
            switch (typeCodeOfValue)
            {
                case TypeCode.Boolean:
                    _bool = Convert.ToBoolean(value);
                    return;
                case TypeCode.Int32:
                    _int = Convert.ToInt32(value);
                    return;
                case TypeCode.Single:
                    _float = Convert.ToSingle(value);
                    return;
                case TypeCode.String:
                    _string = value as string;
                    return;
                case TypeCode.Object:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // basic UObject field
            if (typeof (UObject).IsAssignableFrom(_processedType))
                _object = value as UObject;
            else // must use serializer to store in string field
                _string = Services.Locate<IDynamicSerializer>().Serialize(value, false);
        }

        public object Get()
        {
            return DeserializeValue();
        }

        public T Get<T>()
        {
            Type expectedType = typeof(T);
            if (!expectedType.IsAssignableFrom(Type))
                throw new DynamicException("Cannot convert from current Type {0} to {1}", Type.FullName, expectedType.FullName);
            
            return (T)DeserializeValue();
        }

        private object DeserializeValue()
        {
            if (_deserializedObject != null)
                return _deserializedObject;

            TypeCode typeCodeOfValue = Type.GetTypeCode(Type);
            switch (typeCodeOfValue)
            {
                case TypeCode.Boolean:
                    return _deserializedObject = _bool;
                case TypeCode.Int32:
                    return _deserializedObject = _int;
                case TypeCode.Single:
                    return _deserializedObject = _float;
                case TypeCode.String:
                    return _deserializedObject = _string;
                case TypeCode.Object:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // basic UObject field
            if (typeof(UObject).IsAssignableFrom(Type))
                return _deserializedObject = _object;

            // must use serializer to restore if not set 
            return _deserializedObject = Services.Locate<IDynamicSerializer>().Deserialize(_string, Type);
        }

        public void ClearDeserializedValue()
        {
            _processedType = null;
            _deserializedObject = null;
        }

        public void LockType(Type type)
        {
            _type = type.AssemblyQualifiedName;
            _processedType = type;
            _lockType = true;
        }

        public void UnLockType()
        {
            _lockType = false;
        }

        public override string ToString()
        {
            if (Type == null)
                return string.Concat("(Unknown Type) ", _processedType, " ", _string);

            TypeCode typeCodeOfValue = Type.GetTypeCode(Type);
            switch (typeCodeOfValue)
            {
                case TypeCode.Boolean:
                    return string.Concat("bool ", _bool);
                case TypeCode.Int32:
                    return string.Concat("int ", _int);
                case TypeCode.Single:
                    return string.Concat("float ", _float);
                case TypeCode.String:
                    return string.Concat("string ", _string);
                case TypeCode.Object:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return string.Concat(Type.FullName, " ", DeserializeValue());
        }
    }

    public class DynamicException : Exception
    {
        public DynamicException() { }
        public DynamicException(string message) : base(message) { }

        [StringFormatMethod("msg")]
        public DynamicException(string msg, params object[] args) :
            base(string.Format(msg, args)) { }
    }

    [Serializable]
    public class DynamicValueUnityEvent : UnityEvent<DynamicValue> { }
}
