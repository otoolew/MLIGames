//------------------------------------------------------------------------------
// Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
// Contact: Eric Policaro
//
// Created: Sept 2015
//------------------------------------------------------------------------------

using System.IO;

namespace SG.Core
{
    /// <summary>
    /// Creates a p4ignore file in the Unity project that will
    /// keep the modules from being ignored by accident.
    /// </summary>
    public class IgnoreLoader
    {
        public static void CreateDefaultP4Ignore(bool usingPerforce)
        {
            string p4ignore = "Assets/.p4ignore";
            if (!File.Exists(p4ignore))
            {
                if (usingPerforce)
                {
                    Log.Debug("Generating default p4ignore file");
                    File.WriteAllText(p4ignore, "!Build");
                }
            }
            else
            {
                if (!usingPerforce)
                {
                    File.SetAttributes(p4ignore, ~FileAttributes.ReadOnly);
                    File.Delete(p4ignore);
                }
            }
        }

        private static readonly Notify Log = NotifyManager.GetInstance("Core");
    }
}