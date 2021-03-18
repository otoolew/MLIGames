//------------------------------------------------------------------------------
// Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
// Contact: William Roberts
//
// Created: 01/13/2016
//------------------------------------------------------------------------------

using System;

namespace SG.Core
{
    /// <summary>
    /// Represents a file path that is associated with this object. 
    /// 
    /// Notes:
    /// + This attribute is utilized by the ScriptableSettings object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class FilePathAttribute : Attribute
    {
        public string Path { get; private set; }

        public FilePathAttribute(string path)
        {
            Path = path;
        }
    }
}
