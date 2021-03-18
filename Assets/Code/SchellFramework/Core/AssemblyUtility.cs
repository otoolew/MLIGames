//------------------------------------------------------------------------------
// Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
// Contact: Ana Balliache
//
// Created: Jan 2015
//------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace SG.Core
{
    /// <summary>
    /// Parallel lists of type names and the type.
    /// </summary>
    public class TypeSet
    {
        public List<string> names = new List<string>();
        public List<Type> types = new List<Type>();

        public void Add(string name, Type t)
        {
            names.Add(name);
            types.Add(t);
        }
    }
    
    /// <summary>
    /// Parallel lists of type names and instances of that type.
    /// 
    /// WARNING: This can not be used on iOS due to the AOT compiler not 
    /// being able to trace the generic nesting.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InstanceSet<T> where T : class
    {
        public List<string> names = new List<string>();
        public List<T> instances = new List<T>();

        public void Add(string name, T instance)
        {
            names.Add(name);
            instances.Add(instance);
        }
    }

    /// <summary>
    ///  Utility functions to work with the assembly
    /// </summary>
    public static class AssemblyUtility
    {
        /// <summary>
        /// Creates an instance of all derived types for the given type.
        /// 
        /// WARNING: This can not be used on iOS due to the AOT compiler not 
        /// being able to trace the generic nesting.
        /// </summary>
        /// <typeparam name="T">Base type to use in the search.</typeparam>
        /// <returns>
        /// Parallel lists of the instantiated type names and the instances. 
        /// This format makes it convenient to feed the name list in to a GUI 
        /// dropdown.
        /// </returns>
        public static InstanceSet<T> InstanceAllDerivedTypes<T>() where T : class
        {
            // This does not return a dictionary because a dictionary is not 
            // sorted. The two independent lists are useful for using a GUI 
            // popup (dropdown).
            InstanceSet<T> result = new InstanceSet<T>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = GetTypes(assemblies[i]).Where(
                    type => type.IsSubclassOf(typeof(T))
                    ).ToArray<Type>();
                for (int t = 0; t < types.Length; t++)
                {
                    if (!types[t].IsAbstract)
                    {
                        result.Add(types[t].Name, 
                            Activator.CreateInstance(types[t]) as T);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets all types that derive from the given type.
        /// </summary>
        /// <param name="baseClass">Base class to use in search.</param>
        /// <returns>Parallel lists of type names and types.</returns>
        public static TypeSet GetDerivedTypes(Type baseClass)
        {
            TypeSet result = new TypeSet();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = GetTypes(assemblies[i]).Where(
                    type => type.IsSubclassOf(baseClass)
                    ).ToArray<Type>();
                for (int t = 0; t < types.Length; t++)
                    if (!types[t].IsAbstract)
                        result.Add(types[t].Name, types[t]);
            }
            return result;
        }

        /// <summary>
        /// Returns all the types in the assembly
        /// </summary>
        /// <param name="assembly">assembly to search</param>
        /// <returns>an array of non null types in the assembly</returns>
        public static Type[] GetTypes(Assembly assembly)
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }

            List<Type> validTypes = new List<Type>();
            
            // Remove null instances
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] != null)
                    validTypes.Add(types[i]);
            }

            return validTypes.ToArray();
        }
    }
}
