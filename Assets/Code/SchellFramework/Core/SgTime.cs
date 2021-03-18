// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Eric Policaro
//
//  Created: 8/26/2015 10:40:27 AM
// ------------------------------------------------------------------------------

using System;
using System.Text;
using UnityEngine;

namespace SG.Core
{
    /// <summary>
    /// Utility methods that offer TimeSpan functionality not present in .NET 3.5
    /// </summary>
    public static class SgTime
    {
        public enum UnitThreshold
        {
            Milliseconds=0, // set explicitly as order matters
            Seconds=1,
            Minutes=2,
            Hours=3,
            Days=4
        }

        /// <summary>
        /// Formats a <see cref="System.TimeSpan"/> in as a human-readable string.
        /// </summary>
        /// <param name="timeSpan"><see cref="System.TimeSpan"/> to format</param>
        /// <param name="fullWords">If true, use 'days' instead of 'd', 'hours'
        /// instead of 'h', 'minutes' instead of 'm', and 'seconds' instead of
        /// 's'.</param>
        /// <returns>
        /// String of the format "{days}d {hours}h {minutes}m {seconds}s"; 
        /// If less than 1 second, returns the string 'less than a second'
        /// </returns>
        public static string PrettyPrint(TimeSpan timeSpan,
                                         UnitThreshold minUnitThreshold = UnitThreshold.Seconds,
                                         string belowThresholdMsg = "less than a second",
                                         bool fullWords=false)
        {
            StringBuilder stringBuilder = new StringBuilder();
            PrettyPrintNonAlloc(stringBuilder, timeSpan, minUnitThreshold, belowThresholdMsg, fullWords);

            return stringBuilder.ToString();
        }

        public static void PrettyPrintNonAlloc(StringBuilder stringBuilder, TimeSpan timeSpan,
            UnitThreshold minUnitThreshold = UnitThreshold.Seconds,
            string belowThresholdMsg = "less than a second",
            bool fullWords=false)
        {
            switch (minUnitThreshold)
            {
                case UnitThreshold.Days:
                    if (timeSpan.TotalDays < 1.0)
                    {
                        stringBuilder.Append(belowThresholdMsg);
                        return;
                    }
                    break;
                case UnitThreshold.Hours:
                    if (timeSpan.TotalHours < 1.0)
                    {
                        stringBuilder.Append(belowThresholdMsg);
                        return;
                    }
                    break;
                case UnitThreshold.Minutes:
                    if (timeSpan.TotalMinutes < 1.0)
                    {
                        stringBuilder.Append(belowThresholdMsg);
                        return;
                    }
                    break;
                case UnitThreshold.Seconds:
                    if (timeSpan.TotalSeconds < 1.0)
                    {
                        stringBuilder.Append(belowThresholdMsg);
                        return;
                    }
                    break;
                case UnitThreshold.Milliseconds:
                    if (timeSpan.TotalMilliseconds < 1.0)
                    {
                        stringBuilder.Append(belowThresholdMsg);
                        return;
                    }
                    break;
            }

            bool initialDisplayed = false;
            if ((minUnitThreshold <= UnitThreshold.Days) && (timeSpan.Days > 0))
            {
                initialDisplayed = true;
                stringBuilder.Append(timeSpan.Days);
                if (fullWords)
                {
                    if (timeSpan.Days > 1)
                        stringBuilder.Append(" days");
                    else
                        stringBuilder.Append(" day");
                }
                else
                    stringBuilder.Append('d');
            }
            if ((minUnitThreshold <= UnitThreshold.Hours) && (timeSpan.Hours > 0))
            {
                if (initialDisplayed) stringBuilder.Append(' ');
                initialDisplayed = true;
                stringBuilder.Append(timeSpan.Hours);
                if (fullWords)
                {
                    if (timeSpan.Hours > 1)
                        stringBuilder.Append(" hours");
                    else
                        stringBuilder.Append(" hour");
                }
                else
                    stringBuilder.Append('h');
            }
            if ((minUnitThreshold <= UnitThreshold.Minutes) && (timeSpan.Minutes > 0))
            {
                if (initialDisplayed) stringBuilder.Append(' ');
                initialDisplayed = true;
                stringBuilder.Append(timeSpan.Minutes);
                if (fullWords)
                {
                    if (timeSpan.Minutes > 1)
                        stringBuilder.Append(" minutes");
                    else
                        stringBuilder.Append(" minute");
                }
                else
                    stringBuilder.Append('m');
            }
            if ((minUnitThreshold <= UnitThreshold.Seconds) && (timeSpan.Seconds > 0))
            {
                if (initialDisplayed) stringBuilder.Append(' ');
                initialDisplayed = true;
                stringBuilder.Append(timeSpan.Seconds);
                if (fullWords)
                {
                    if (timeSpan.Seconds > 1)
                        stringBuilder.Append(" seconds");
                    else
                        stringBuilder.Append(" second");
                }
                else
                    stringBuilder.Append('s');
            }
            if ((minUnitThreshold <= UnitThreshold.Milliseconds) && (timeSpan.Milliseconds > 0))
            {
                if (initialDisplayed) stringBuilder.Append(' ');
                initialDisplayed = true;
                stringBuilder.Append(timeSpan.Milliseconds.ToString("D3"));
                if (fullWords)
                {
                    if (timeSpan.Milliseconds > 1)
                        stringBuilder.Append(" milliseconds");
                    else
                        stringBuilder.Append(" millisecond");
                }
                else
                    stringBuilder.Append("ms");
            }

            if (stringBuilder.Length == 0)
                Debug.LogWarning("Output of PrettyPrint length is 0");
        }

        /// <summary>
        /// Formats a length of time in seconds as a human-readable string.
        /// </summary>
        /// <param name="timeInSeconds">The time in seconds.</param>
        /// <returns>
        /// String of the format "{days}d {hours}h {minutes}m {seconds}s"; 
        /// If less than 1 second, returns the string 'less than a second'
        /// </returns>
        public static string PrettyPrint(double timeInSeconds)
        {
            return PrettyPrint(TimeSpan.FromSeconds(timeInSeconds));
        }
    }
}
