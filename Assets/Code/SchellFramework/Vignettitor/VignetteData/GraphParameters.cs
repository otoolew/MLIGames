// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 3/14/2016 11:53:45 AM
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SG.Core;
using SG.Dynamics;
using SG.Vignettitor.Graph;
using UnityEngine;

namespace SG.Vignettitor.VignetteData
{
    [Serializable]
    public class GraphSignature
    {
        [SerializeField]
        private GraphParameter[] _parameters;

        [Serializable]
        public struct GraphParameter
        {
            public string Name;

            [DynamicType]
            public string ExpectedType;

            public GraphParameterUsage[] Usages;
        }

        [Serializable]
        public struct GraphParameterUsage
        {
            public int NodeId;
            public string PropertyName;
        }

        public T Resolve<T>(int nodeId, string propertyName, GraphInvocation invocation, T fallback)
        {
            int parameterCount = _parameters.SafeLength();
            for (int i = 0; i < parameterCount; i++)
            {
                int usageCount = _parameters[i].Usages.SafeLength();
                for (int j = 0; j < usageCount; j++)
                {
                    if (_parameters[i].Usages[j].NodeId != nodeId ||
                        !string.Equals(_parameters[i].Usages[j].PropertyName, propertyName))
                        continue;

                    return invocation.Resolve<T>(_parameters[i].Name, fallback);
                }
            }
            return fallback;
        }

        public ContentValidation Validate(VignetteGraph graph)
        {
            ContentValidation cv = new ContentValidation();
            int parameterCount = _parameters.SafeLength();
            for (int i = 0; i < parameterCount; i++)
            {
                string parameterName = _parameters[i].Name;
                if (string.IsNullOrEmpty(parameterName))
                {
                    cv.Error(this, "Graph Parameter {0} has no Name.", i);
                    parameterName = string.Concat("(null) [", i, "]");
                }

                string typeName = _parameters[i].ExpectedType;
                Type expectedType = Type.GetType(typeName);
                if (expectedType == null)
                    cv.Error(this, "Graph Parameter {0} has invalid ExpectedType {1}.", parameterName, typeName);

                int usageCount = _parameters[i].Usages.SafeLength();
                for (int j = 0; j < usageCount; j++)
                {
                    int nodeId = _parameters[i].Usages[j].NodeId;
                    if (graph[nodeId] == null)
                        cv.Error(this, "Graph Parameter {0} Usage {1} has invalid NodeId {2}.", parameterName, j, nodeId);

                    if (string.IsNullOrEmpty(_parameters[i].Usages[j].PropertyName))
                        cv.Error(this, "Graph Parameter {0} Usage {1} has blank PropertyName.", parameterName, j);
                }
            }
            return cv;
        }

        public ContentValidation ValidateInvocation(GraphInvocation invocation, [CanBeNull] HashSet<string> assumedParameters)
        {
            ContentValidation cv = new ContentValidation();
            int parameterCount = _parameters.SafeLength();
            for (int i = 0; i < parameterCount; i++)
            {
                string parameterName = _parameters[i].Name;

                if (assumedParameters != null && assumedParameters.Contains(parameterName))
                    continue;

                bool parameterFound = false;
                int invocationValueCount = invocation.Values.SafeLength();
                for (int j = 0; j < invocationValueCount; j++)
                {
                    if (!string.Equals(parameterName, invocation.Values[j].ParameterName))
                        continue;

                    parameterFound = true;

                    string typeName = _parameters[i].ExpectedType;
                    Type expectedType = Type.GetType(typeName);
                    if (!expectedType.IsAssignableFrom(invocation.Values[j].Value.Type))
                        cv.Error(this, "Graph Parameter {0} Invocation is type {1} but {2} is expected.", parameterName,
                            invocation.Values[j].Value.Type, typeName);
                }
                if (!parameterFound)
                    cv.Error(this, "Graph Parameter {0} not present in Invocation.", parameterName);
            }
            return cv;
        }
    }

    [Serializable]
    public class GraphInvocation
    {
        public static readonly GraphInvocation Empty = new GraphInvocation();

        [SerializeField]
        private List<GraphInvocationValue> _values;
        public List<GraphInvocationValue> Values { get { return _values; } }

        [Serializable]
        public struct GraphInvocationValue
        {
            public string ParameterName;
            public bool PassToSubgraphs; // should only be used for graph scope values
            public DynamicValue Value;
        }

        [Serializable]
        public struct GraphInvocationRemap
        {
            public string SuperParameterName;
            public string SubParameterName;
        }

        public T Resolve<T>(string parameterName, T fallback)
        {
            int valueCount = _values.SafeLength();
            for (int i = 0; i < valueCount; i++)
                if (string.Equals(_values[i].ParameterName, parameterName))
                    return _values[i].Value.Get<T>();
            return fallback;
        }

        public void AddGraphScopeValue(string parameterName, object value)
        {
            DynamicValue wrappedValue = new DynamicValue(value);
            AddGraphScopeValue(parameterName, wrappedValue);
        }

        public void AddGraphScopeValue(string parameterName, DynamicValue value)
        {
            GraphInvocationValue newInvocationValue = new GraphInvocationValue
            {
                ParameterName = parameterName,
                Value = value,
                PassToSubgraphs = true
            };

            int valueCount = _values.SafeLength();
            for (int i = 0; i < valueCount; i++)
            {
                if (!string.Equals(_values[i].ParameterName, parameterName))
                    continue;

                _values[i] = newInvocationValue;
                return;
            }

            if (valueCount < 1)
                _values = new List<GraphInvocationValue>();

            _values.Add(newInvocationValue);
        }

        public GraphInvocation CloneForSubGraph(GraphInvocation superGraphInvocation, GraphInvocationRemap[] remaps)
        {
            GraphInvocation dynamicInvocation = new GraphInvocation { _values = new List<GraphInvocationValue>(_values) };
            int remapCount = remaps.SafeLength();
            for (int i = 0; i < remapCount; i++)
                dynamicInvocation.AddRemap(superGraphInvocation, remaps[i]);
            int valueCount = superGraphInvocation._values.SafeLength();
            for (int i = 0; i < valueCount; i++)
            {
                if (!superGraphInvocation._values[i].PassToSubgraphs)
                    continue;

                dynamicInvocation.AddGraphScopeValue(superGraphInvocation._values[i].ParameterName, superGraphInvocation._values[i].Value);
            }
            return dynamicInvocation;
        }

        private void AddRemap(GraphInvocation otherInvocation, GraphInvocationRemap remap)
        {
            DynamicValue rawResolved = null;
            int otherValueCount = otherInvocation._values.SafeLength();
            for (int i = 0; i < otherValueCount; i++)
            {
                if (!string.Equals(otherInvocation._values[i].ParameterName, remap.SuperParameterName))
                    continue;
                rawResolved = otherInvocation._values[i].Value;
                break;
            }

            _values.Add(new GraphInvocationValue
            {
                ParameterName = remap.SubParameterName,
                Value = rawResolved
            });
        }
    }
}
