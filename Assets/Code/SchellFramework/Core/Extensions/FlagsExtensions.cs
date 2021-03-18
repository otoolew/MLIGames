//------------------------------------------------------------------------------
// Copyright © 2014 Schell Games, LLC. All Rights Reserved.
//
// Contact: Joe Pasek
//
// Created: October 2014
//------------------------------------------------------------------------------

using System;

namespace SG.Core
{
    /// <summary>
    /// Helper functions for dealing with enum flags
    /// From: From: http://stackoverflow.com/a/417217/1598965
    /// </summary>
    public static class FlagsExtensions
    {
        //checks if the value contains the provided type        
        /// <summary>
        /// Determines whether the enum contains the given flag value.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="type">Extension</param>
        /// <param name="value">Value to check</param>
        /// <returns><c>true </c>if the enum contains the flag value; 
        /// <c>false</c> otherwise</returns>
        public static bool Has<T>(this Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if this enum is only of the given flag value.
        /// </summary>
        /// <typeparam name="T">Enum flags type</typeparam>
        /// <param name="type">Extension</param>
        /// <param name="value">Value to check</param>
        /// <returns><c>true</c> if the this enum is exactly value; 
        /// <c>false</c> otherwise</returns>
        public static bool Is<T>(this Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Adds a flag to this enum.
        /// </summary>
        /// <typeparam name="T">Enum flags type</typeparam>
        /// <param name="type">Extension</param>
        /// <param name="value">Value to append</param>
        /// <returns>Enum with given value appended</returns>
        /// <exception cref="System.ArgumentException">If the value could not be appended</exception>
        public static T Add<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                string msg = string.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name);

                throw new ArgumentException(msg, ex);
            }
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <typeparam name="T">Enum flags type</typeparam>
        /// <param name="type">Extension</param>
        /// <param name="value">Value to be removed</param>
        /// <returns>Enum without the value</returns>
        /// <exception cref="System.ArgumentException">The value could not be removed</exception>
        public static T Remove<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                string msg =string.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name);

                throw new ArgumentException(msg, ex);
            }
        }
    }
}
