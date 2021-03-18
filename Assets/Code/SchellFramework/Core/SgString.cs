// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Eric Policaro
//
//  Created: 8/26/2015 12:01:36 PM
// ------------------------------------------------------------------------------

using System;

namespace SG.Core
{
    /// <summary>
    /// String utilities and extension methods that offer functionality not present in .NET 3.5
    /// </summary>
    public static class SgString
    {
        /// <summary>
        /// Returns a value indicating whether a given <see cref="System.String"/> is
        /// contained within this string. Allows case to be ignored. 
        /// For most cases, <see cref="StringComparison.Ordinal"/> or <see cref="StringComparison.OrdinalIgnoreCase"/>
        /// should be used
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/444798/case-insensitive-containsstring
        /// </remarks>
        /// <param name="sourceData">Source string, extension.</param>
        /// <param name="value">Value to search for</param>
        /// <param name="comparison">Comparison option</param>
        /// <returns>true if value is contained with the string or value is the empty string(""); false otherwise</returns>
        /// <exception cref="System.ArgumentNullException">
        /// sourceData;Cannot run contains on a null string
        /// or
        /// value;Cannot search for a null string
        /// </exception>
        public static bool Contains(this string sourceData, string value, StringComparison comparison)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "Cannot search for a null string");
            }

            return sourceData.IndexOf(value, comparison) >= 0;
        }

        /// <summary>
        /// Returns a substring before the last occurance of character.
        /// </summary>
        /// <example>
        /// "This.is.a.test".TrimBeforeLast('.') returns "this.is.a"
        /// </example>
        /// <param name="source">The source string, extension.</param>
        /// <param name="character">Character to trim to</param>
        /// <returns>Substring before the last occurrence of the character (excluding that character).</returns>
        public static string TrimAfterLast(this string source, char character)
        {
            int lastIndex = source.LastIndexOf(character);
            if (lastIndex == -1)
                return source;

            return source.Substring(0, lastIndex);
        }

        /// <summary>
        /// Returns a substring after the last occurance of character.
        /// </summary>
        /// <example>
        /// "This.is.a.test".TrimBeforeLast('.') returns "test"
        /// </example>
        /// <param name="source">The source string, extension.</param>
        /// <param name="character">Character to trim from</param>
        /// <returns>Substring after the last occurrence of the character (excluding that character).</returns>
        public static string TrimBeforeLast(this string source, char character)
        {
            int lastIndex = source.LastIndexOf(character);
            if (lastIndex == -1)
                return source;

            return source.Substring(lastIndex + 1, source.Length - lastIndex - 1);
        }

        /// <summary>
        /// Checks if a string matches a pattern with wildcard characters, 
        /// denoted by "*". <b>This is a recursive function and should not be called every 
        /// frame in game code.</b>
        /// </summary>
        /// <param name="pattern">
        /// The pattern that the input will be matched to, where * denotes any 
        /// set of characters.
        /// </param>
        /// <param name="input">The string to compare to the pattern.</param>
        /// <returns>
        /// True if the input string matches the pattern taking into account 
        /// the wildcard character usages.
        /// </returns>
        public static bool MatchWildcardString(string pattern, string input)
        {
            if (string.CompareOrdinal(pattern, input) == 0)
                return true;

            if (string.IsNullOrEmpty(input))
                return string.IsNullOrEmpty(pattern.Trim('*'));

            if (pattern.Length == 0)
                return false;

            if (pattern[0] == '*')
            {
                if (MatchWildcardString(pattern.Substring(1), input))
                    return true;
                
                return MatchWildcardString(pattern, input.Substring(1));
            }

            if (pattern[pattern.Length - 1] == '*')
            {
                if (MatchWildcardString(pattern.Substring(0, pattern.Length - 1), input))
                    return true;

                return MatchWildcardString(pattern, input.Substring(0, input.Length - 1));
            }

            if (pattern[0] == input[0])
            {
                return MatchWildcardString(pattern.Substring(1), input.Substring(1));
            }

            return false;
        }

        /// <summary>
        /// Verifies if the given string is null or only whitespace characters.
        /// </summary>
        /// <remarks>
        /// This functionality is present in .NET 4.0, but not in 3.5.
        /// </remarks>
        /// <param name="value">The string to examine.</param>
        /// <returns>
        /// True if the string is null, empty, or contains only whitespace characters.
        /// </returns>
        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a string of code points representing the content of the passed string.
        /// </summary>
        public static string GetCodePoints(string input)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i=0; i < input.Length; i += char.IsSurrogatePair(input, i) ? 2 : 1)
            {
                int codepoint = char.ConvertToUtf32(input, i);
                sb.AppendFormat("U+{0:X4} ", codepoint);
            }
            return sb.ToString();
        }
    }
}