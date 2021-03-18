// -----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Max Golden
//
//  Created: 12/5/2016 1:55 PM
// -----------------------------------------------------------------------------

using System;

namespace SG.Core.Templating
{
    /// <summary>
    /// Attribute put over a string method/property/field to indicate that it should replace
    /// 'key' in a template
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class TemplateKeyAttribute : Attribute
    {
        public string key;

        public TemplateKeyAttribute(string key)
        {
            this.key = key;
        }
    }
}
