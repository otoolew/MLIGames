// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Eric Policaro
// 
//  Date: 03/09/2016
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SG.Core;
using SG.Vignettitor.VignetteData;

namespace SG.Vignettitor.VignettitorCore
{
    /// <summary>
    /// Keeps track of vignettitor utility instances for each available 
    /// type of vignette graph. This lets graphs of the same type share data 
    /// but prevents illegal data usage across graph types.
    /// </summary>
    /// <typeparam name="T">Utility type</typeparam>
    public class UtilityManager<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UtilityManager{T}"/> class.
        /// </summary>
        /// <param name="dataType">
        /// Type of the data. This type must be the same
        /// as or extend from the utility type ({T})
        /// </param>
        /// <exception cref="Exception">
        /// An exception is thrown if the provided type cannot be cast to
        /// the utility object's type.
        /// </exception>
        public UtilityManager(Type dataType)
        {
            if (!IsValidType(typeof(T), dataType))
                throw new Exception("Data error. Provided data type cannot be cast to " + typeof(T));

            _data = new Dictionary<Type, T>();
            Type baseGraphType = typeof(VignetteGraph);
            TypeSet types = AssemblyUtility.GetDerivedTypes(baseGraphType);
            types.Add(baseGraphType.Name, baseGraphType);
            for (int i = 0; i < types.types.Count; i++)
                _data.Add(types.types[i], Activator.CreateInstance(dataType) as T);
        }

        /// <summary>
        /// Gets the utility instance for the provided graph type.
        /// </summary>
        /// <param name="graphType">
        /// A type that must derive from VignetteGraph.
        /// </param>
        /// <returns>
        /// Utility instance of the type passed in to the constructor.
        /// </returns>
        public T GetInstance(Type graphType)
        {
            if (!IsValidType(typeof(VignetteGraph), graphType))
                throw new Exception("Provided type does not derive from VignetteGraph");

            if (_data.ContainsKey(graphType))
                return _data[graphType];
            return null;
        }

        private bool IsValidType(Type baseType, Type toCheck)
        {
            return baseType.IsAssignableFrom(toCheck);
        }

        private readonly Dictionary<Type, T> _data;
    }
}