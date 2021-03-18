// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 4/28/2016 9:20:25 AM
// ------------------------------------------------------------------------------

// Uncomment to test using the Unity JsonUtility
//#undef SG_Json

using System;
using SG.Core;
#if SG_Json
using Newtonsoft.Json;
using JsonSerializer = SG.Json.JsonSerializer;
#else
using UnityEngine;
#endif

namespace SG.Dynamics
{
    [ServiceOnDemand(ServiceType = typeof(IDynamicSerializer))]
    public class DynamicSerializer : IDynamicSerializer
    {
#if SG_Json
        private readonly JsonSerializer _serializer;

        public DynamicSerializer()
        {
            _serializer = new JsonSerializer();
        }

        public string Serialize(object value, bool prettyPrint)
        {
            return _serializer.SerializeObject(value, prettyPrint);
        }

        public object Deserialize(string json, Type expectedType)
        {
            try
            {
                return _serializer.DeserializeObject(json, expectedType);
            }
            catch (JsonSerializationException)
            {
                return Activator.CreateInstance(expectedType);
            }
        }
#else
        public string Serialize(object value, bool prettyPrint)
        {
            return JsonUtility.ToJson(value, prettyPrint);
        }

        public object Deserialize(string json, Type expectedType)
        {
            return JsonUtility.FromJson(json, expectedType);
        }
#endif
    }

    public interface IDynamicSerializer : IService
    {
        string Serialize(object value, bool prettyPrint);

        object Deserialize(string json, Type expectedType);
    }
}
