// -----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Max Golden
//
//  Created: 12/2/2016 1:25 PM
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SG.Core.Templating
{
    /// <summary>
    /// Exception thrown when a user attempts to use a template key that could conflict with
    /// the template engine's intermediate hash (which is either a decimal or hexidecimal string)
    /// </summary>
    public class InvalidTemplateKeyException : Exception
    {
        public InvalidTemplateKeyException() { }
        public InvalidTemplateKeyException(string message) : base(message) { }
        public InvalidTemplateKeyException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Class that populates a template by applying a number of string substition transforms
    /// atomically. Each key is replaced with an intermediate hash, which is in turn replaced with
    /// the evaluated final value.
    /// </summary>
    public class TemplateEngine
    {
        private readonly Notify Log = NotifyManager.GetInstance<TemplateEngine>();

        // Function evaluated at Render time to replace a given key with a new string
        public delegate string TemplateTransform();

        private readonly string _template;  // Base template with no substitutions
        private readonly bool _useDecimalHash;  // 0-to-9-only hashes allow for keys like "A" and "B", which conflict with hex hashes
        private Dictionary<string, TemplateTransform> _transformDictionary; // Map of key -> transform

        /// <summary>
        /// Create a new TemplateEngine for the given template string
        /// </summary>
        /// <param name="template">Template to be transformed</param>
        /// <param name="useDecimalHash">If true, all hashes will be in decimal, which may reduce
        /// incidents of keys conflicting with the intermediate hash. If false, hashes will be in
        /// hexadecimal, which are faster and use less memory.</param>
        public TemplateEngine(string template, bool useDecimalHash = false)
        {
            _template = template;
            _useDecimalHash = useDecimalHash;
            _transformDictionary = new Dictionary<string, TemplateTransform>();
        }

        /// <summary>
        /// Bind the given key to a template transform
        /// </summary>
        /// <param name="key">Text to replace with the transform</param>
        /// <param name="transform">Transform to evaluate for the substitution</param>
        public void AddTransform(string key, TemplateTransform transform)
        {
            // Check for potential collisions against intermediate hash values (decimal/hexadecimal digits)
            string collisionRegex = _useDecimalHash ? @"^[0-9]*$" : @"^[0-9A-F]*$";
            bool matchedCollision = new System.Text.RegularExpressions.Regex(collisionRegex).IsMatch(key);

            if (matchedCollision)  // Reject any key that could potentially appear in toto in the intermediate hash
            {
                string message = string.Format(
                    "'{0}' is not a valid key for the template engine. Keys must contain at least one character that is not a digit{1}.",
                    key, _useDecimalHash ? string.Empty : " and not a capital letter A through F");
                throw new InvalidTemplateKeyException(message);
            }

            _transformDictionary[key] = transform;
        }

        /// <summary>
        /// Bind the given key to a string
        /// </summary>
        /// <param name="key">Text to replace with the string</param>
        /// <param name="transformString">String with which to replace the key</param>
        public void AddTransform(string key, string transformString)
        {
            AddTransform(key, () => transformString);
        }

        /// <summary>
        /// Apply all of the transforms registered with AddTransform and return the
        /// rendered template
        /// </summary>
        /// <returns>The template with all of the tranform keys replaced with evaluated
        /// TemplateTransform delegates</returns>
        public string Render()
        {
            string transformedTemplate = _template;

            // Replace the keys with hashes to avoid colliding with user text
            Dictionary<string, string> keyHashes = new Dictionary<string, string>();
            foreach (string key in _transformDictionary.Keys)
            {
                string hashedKey = GetStringHash(key);
                keyHashes[key] = hashedKey;
                transformedTemplate = transformedTemplate.Replace(key, hashedKey);
            }

            // Replace the intermediate hashes with the evaluated TemplateTransforms
            Dictionary<string, TemplateTransform>.Enumerator enumerator = _transformDictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                transformedTemplate = transformedTemplate.Replace(
                    keyHashes[enumerator.Current.Key],
                    enumerator.Current.Value.Invoke());
            }
            return transformedTemplate;
        }

        /// <summary>
        /// Gathers all TemplateKeyAttributes from the given object and maps their keys
        /// to template transforms
        /// </summary>
        /// <param name="obj">Instance of an object with one or more TemplateKeyAttributes</param>
        /// <returns>The total number of transforms added</returns>
        public int ApplyModel(object obj)
        {
            int numTransformsAdded = 0;
            Type objType = obj.GetType();
            Type attributeType = typeof(TemplateKeyAttribute);

            // Get method delegates
            foreach (MethodInfo method in objType.GetMethods())
            {
                if (!method.IsDefined(attributeType, true))
                    continue;

                if (!method.IsPublic)
                {
                    Log.Warning("Cannot create template binding for method {0}: Method must be public", method.Name);
                    continue;
                }

                if (method.ReturnType == typeof(void))
                {
                    Log.Warning("Cannot create template binding for method {0}: Method must not return void", method.Name);
                    continue;
                }

                foreach (TemplateKeyAttribute att in GetAllAttributes<TemplateKeyAttribute>(method))
                {
                    AddTransform(att.key, () => method.Invoke(obj, null).ToStringSafe());
                    numTransformsAdded++;
                }
            }

            // Get field delegates
            foreach (FieldInfo field in objType.GetFields())
            {
                if (!field.IsDefined(attributeType, true))
                    continue;

                if (!field.IsPublic)
                {
                    Log.Warning("Cannot create template binding for field {0}: Field must be public", field.Name);
                    continue;
                }

                foreach (TemplateKeyAttribute att in GetAllAttributes<TemplateKeyAttribute>(field))
                {
                    AddTransform(att.key, () => field.GetValue(obj).ToStringSafe());
                    numTransformsAdded++;
                }
            }

            // Get property delegates
            foreach (PropertyInfo property in objType.GetProperties())
            {
                if (!property.IsDefined(attributeType, true))
                    continue;

                MethodInfo getMethod = property.GetGetMethod();
                if (getMethod == null || !getMethod.IsPublic)
                {
                    Log.Warning("Cannot create template binding for property {0}: Get method must be public", property.Name);
                    continue;
                }

                foreach (TemplateKeyAttribute att in GetAllAttributes<TemplateKeyAttribute>(property))
                {
                    AddTransform(att.key, () => getMethod.Invoke(obj, null).ToStringSafe());
                    numTransformsAdded++;
                }
            }

            if (numTransformsAdded == 0)
            {
                Log.Warning("No valid TemplateKeyAttributes found on the class {0}", objType.Name);
            }
            return numTransformsAdded;
        }

        /// <summary>
        /// Hash the given text in a cryptographic fashion
        /// </summary>
        /// <param name="text">String to hash</param>
        /// <returns>String corresponding to the text's SHA256 hash in decimal or hexadecimal digits</returns>
        private string GetStringHash(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);

                if (_useDecimalHash)
                {
                    // If we want only numbers, we've got to deal with 6x more digits than hex. C'est la vie
                    return string.Join(string.Empty, hash.Select(x => x.ToString("000")).ToArray());
                }
                else
                {
                    // If we want the hex string, we use BitConverter which represents 4 bytes as 2 hex chars
                    return BitConverter.ToString(hash).Replace("-", string.Empty);  // but it adds these annoying '-'s so lets put a stop to that
                }
            }
        }

        /// <summary>
        /// Gets all custom attributes of type T on the given attributeProvider
        /// </summary>
        /// <typeparam name="T">Attribute type to get</typeparam>
        /// <param name="attributeProvider">ICusomAttributeProvider from which to gather the attribtues</param>
        /// <returns>IEnumerable of all of the attributes on the attribute provider casted to type T</returns>
        private IEnumerable<T> GetAllAttributes<T>(ICustomAttributeProvider attributeProvider) where T : Attribute
        {
            return attributeProvider.GetCustomAttributes(typeof(T), true).Cast<T>();
        }
    }

    public static class SafetyExtensions
    {
        /// <summary>
        /// If you call ToString() on a null string/object, the computer gets mad. This makes it less mad.
        /// </summary>
        public static string ToStringSafe(this object obj)
        {
            return obj == null ? string.Empty : obj.ToString();
        }
    }
}
