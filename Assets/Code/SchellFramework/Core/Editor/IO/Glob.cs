//------------------------------------------------------------------------------
// Copyright © 2011 Schell Games, LLC. All Rights Reserved.
//
// Contact: William Roberts
//
// Created: 05/22/2011
//------------------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace SG.Core.IO
{
    /// <summary>
    /// Glob provides string matching based on Glob syntax 
    /// Reference: https://en.wikipedia.org/wiki/Glob_(programming)
    /// 
    /// This is a more user friendly syntax for basic pattern matching operations.
    /// Patterns are case sensitive.
    /// 
    /// <list type="table">
    /// <listheader>
    ///     <term>Character</term>
    /// </listheader>
    ///     <term>?</term>
    ///     <description>
    ///     Matches exactly one of any character. Example: 
    ///     '?at' matches 'cat' and 'bat', but not 'at' or 'ats'.
    ///     </description>
    /// 
    ///     <term>[]</term>
    ///     <description>Empty brackets match no characters. Example: 
    ///     'Letter[]' matches 'Letter' but not 'Letters'
    ///     </description>
    ///     
    ///     <term>[AB]</term>
    ///     <description>Match a set of characters only. Example: 
    ///     '[BC]at' matches 'Cat' and 'Bat', but not 'at' or 'Nat'
    ///     </description>
    /// 
    ///     <term>[0-9]</term>
    ///     <description>Match a range of characters. Example:
    ///     'Letter[0-9]' matches 'Letter0' or 'Letter5', but not 'Letter' or 'Letters'.
    ///     Other typical ranges are [a-z] (all lowercase) and [A-Z] all uppercase.
    ///     </description>
    /// 
    ///     <term>*</term>
    ///     <description>Match zero or more of any characters. Example:
    ///     '*law*' matches 'lawyer', 'unlawful', and 'outlaw'</description>
    /// </list>
    /// 
    /// </summary>
    public class Glob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Glob"/> class.
        /// </summary>
        /// <param name="pattern">Glob pattern to validate against.</param>
        public Glob(string pattern)
        {
            _globRegEx = ToRegExPattern(pattern);
            System.Diagnostics.Debug.WriteLine(_globRegEx);
        }

        private Regex ToRegExPattern(string globPattern)
        {
            string cleanedGlob = globPattern.Replace("[]", "");
            string regexPattern = Regex.Escape(cleanedGlob)
                                    .Replace(@"\[", "[")
                                    .Replace(@"\*", ".*")
                                    .Replace(@"\?", ".");

            return new Regex("^" + regexPattern + "$");
        }

        /// <summary>
        /// Determines if the specified string satifies the glob wildcard.
        /// pattern.
        /// </summary>
        /// <param name="input">String to test.</param>
        /// <returns>True if the test string matches the pattern.</returns>
        public bool IsMatch(string input)
        {
            return _globRegEx.IsMatch(input);
        }

        private readonly Regex _globRegEx;
    }
}
